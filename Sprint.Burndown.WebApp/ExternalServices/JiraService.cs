using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Newtonsoft.Json.NetCore;

using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.Const;
using Sprint.Burndown.WebApp.Core;
using Sprint.Burndown.WebApp.Extensions;
using Sprint.Burndown.WebApp.JiraModels;

using RestRequest = RestSharp.RestRequest;

namespace Sprint.Burndown.WebApp.ExternalServices
{
    public class JiraService : IJiraService
    {
        private const string MAX_RESULTS_LIMIT = "250";

        private static readonly string AuthService = "/auth/1/session/";

        private static readonly string AgileBoardService = "/agile/1.0/board";

        private static readonly string AgileOneBoardService = "/agile/1.0/board/{0}";

        private static readonly string AgileBoardSprintsService = "/agile/1.0/board/{0}/sprint";

        private static readonly string AgileSprintService = "/agile/1.0/sprint/{0}";

        private static readonly string AgileSprintIssuesService = "/agile/1.0/sprint/{0}/issue";

        private static readonly string AgileSprintOneIssueService = "/agile/1.0/issue/{0}";

        private static readonly string AgileIssueWorklogService = "/api/2/issue/{0}/worklog";

        private static readonly string MyselfService = "/api/2/myself/";

        private static readonly NLog.ILogger Log = LoggerType.Integration;

        private JiraOptions Options { get; }

        private ICredentialsStorage CredentialsStorage { get; }

        public JiraService(IOptionsMonitor<JiraOptions> jiraOptionsAccessor, ICredentialsStorage credentialsStorage)
        {
            Options = jiraOptionsAccessor.CurrentValue;
            CredentialsStorage = credentialsStorage;
        }

        public Session Login(string username, string password)
        {
            if (Options.UseBasicAuthentication)
            {
                return null;
            }

            var client = new RestClient(Options.JiraApiUrl) { CookieContainer = new CookieContainer() };

            var request = CreateRequest(AuthService, Method.POST);
            request.AddBody(new { username = username, password = password });

            var response = client.Execute<AuthResponse>(request);

            return response.Data.Session;
        }

        public UserModel GetUserInformation(string email, string password)
        {
            var client = new RestClient(Options.JiraApiUrl)
            {
                Authenticator = new HttpBasicAuthenticator(email, password)
            };

            var username = email.ExtractUsername();
            var request = CreateRequest(MyselfService, Method.GET);

            var response = client.Execute<UserModel>(request);
            var isOk = response.StatusCode == HttpStatusCode.OK;

            if (!isOk)
            {
                Log.Warn($"Authentication for [{username}] failed. Jira response: [{response.StatusCode}].");
                LogErrorResponse(response);
                return null;
            }

            return response.Data;
        }

        private void LogErrorResponse(IRestResponse<UserModel> response)
        {
            if (response.ErrorException != null)
            {
                Log.Warn(response.ErrorException, $"[Connection error]: '{response.ErrorMessage}'.");
            }

            // try to get additional errors
            try
            {
                var errors = JsonConvert.DeserializeObject<JiraErrorsModel>(response.Content);
                if (errors != null)
                {
                    if ((errors?.ErrorMessages?.Length ?? 0) > 0)
                    {
                        foreach (var error in errors.ErrorMessages)
                        {
                            Log.Warn($"[JIRA Error]: '{error}'.");
                        }
                    }
                }
            }
            catch
            {
                Log.Warn($"No additional errors on [{response}]");
            }
        }

        public bool Logout(Session session)
        {
            if (Options.UseBasicAuthentication)
            {
                return true;
            }

            var client = CreateRestClient(session);
            var request = CreateRequest(AuthService, Method.DELETE);
            var response = client.Execute(request);

            return response.StatusCode == HttpStatusCode.NoContent;
        }

        public IDictionary<string, DateTime> GetIssueUpdatesInternal(Session session, string sprintId)
        {
            var client = CreateRestClient(session);
            var url = string.Format(AgileSprintIssuesService, sprintId);
            var request = CreateRequest(url, Method.GET);
            request.AddQueryParameter("fields", "updated");
            request.AddQueryParameter("startAt", "0");
            request.AddQueryParameter("maxResults", MAX_RESULTS_LIMIT);

            var response = client.Execute(request);
            var responseData = JsonConvert.DeserializeObject<PagingIssues<IssueModel>>(response.Content);
            var result = responseData.Issues
                .Where(x => x.Fields.Updated.HasValue)
                .ToDictionary(k => k.Id, v => v.Fields.Updated.Value);

            return result;
        }
        
        public IPagingList<BoardModel> GetBoardsInternal(Session session)
        {
            var client = CreateRestClient(session);
            var request = CreateRequest(AgileBoardService, Method.GET);
            request.AddQueryParameter("startAt", "0");
            request.AddQueryParameter("maxResults", MAX_RESULTS_LIMIT);
            request.AddQueryParameter("projectKeyOrId", ProjectKeys.Main);
            request.AddQueryParameter("type", BoardTypes.Scrum);

            var response = client.Execute<PagingList<BoardModel>>(request);

            return response.Data;
        }

        public BoardModel GetBoardInternal(Session session, string boardId)
        {
            var client = CreateRestClient(session);
            var url = string.Format(AgileOneBoardService, boardId);
            var request = CreateRequest(url, Method.GET);
            var response = client.Execute<BoardModel>(request);

            return response.Data;
        }

        public IPagingList<SprintModel> GetSprintsInternal(Session session, string boardId)
        {
            var client = CreateRestClient(session);
            var url = string.Format(AgileBoardSprintsService, boardId);
            var request = CreateRequest(url, Method.GET);
            request.AddQueryParameter("startAt", "0");
            request.AddQueryParameter("maxResults", MAX_RESULTS_LIMIT);
            request.AddQueryParameter("state", SprintTypes.ActiveAndFuture);

            var response = client.Execute<PagingList<SprintModel>>(request);
            var sprints = response.Data;

            return sprints;
        }

        public SprintModel GetSprintInternal(Session session, string sprintId)
        {
            var client = CreateRestClient(session);
            var url = string.Format(AgileSprintService, sprintId);
            var request = CreateRequest(url, Method.GET);
            var response = client.Execute<SprintModel>(request);

            return response.Data;
        }

        public IPagingList<IssueModel> GetIssuesInternal(Session session, string sprintId)
        {
            var client = CreateRestClient(session);
            var url = string.Format(AgileSprintIssuesService, sprintId);
            var request = CreateRequest(url, Method.GET);
            request.AddQueryParameter("fields", $"{Options.ActualTestTimeField},fixVersions,priority,labels,assignee,status,summary,description,components,reporter,issuetype,description,timetracking,summary,created,updated,parent");
            request.AddQueryParameter("expand", $"{Options.ActualTestTimeField},fixVersions,priority,labels,assignee,status,summary,description,components,reporter,issuetype,description,timetracking,summary,created,updated,parent");
            request.AddQueryParameter("startAt", "0");
            request.AddQueryParameter("maxResults", MAX_RESULTS_LIMIT);

            var response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var resultData = JsonConvert.DeserializeObject<PagingIssues<IssueModel>>(response.Content);
                return resultData;
            }

            return null;
        }

        public IssueChangelogModel GetIssueChangelogInternal(Session session, string issueId)
        {
            var client = CreateRestClient(session);
            var url = string.Format(AgileSprintOneIssueService, issueId);

            var request = CreateRequest(url, Method.GET);
            request.AddQueryParameter("expand", "changelog");
            request.AddQueryParameter("startAt", "0");
            request.AddQueryParameter("maxResults", "150");

            var response = client.Execute(request);
            var issueHistory = JsonConvert.DeserializeObject<IssueChangelogModel>(response.Content);

            return issueHistory;
        }

        public IssueWorklogModel GetIssueWorklogInternal(Session session, string issueId)
        {
            var client = CreateRestClient(session);
            var url = string.Format(AgileIssueWorklogService, issueId);

            var request = CreateRequest(url, Method.GET);
            request.AddQueryParameter("expand", "properties");
            request.AddQueryParameter("startAt", "0");
            request.AddQueryParameter("maxResults", "150");

            var response = client.Execute(request);
            var issueWorklog = JsonConvert.DeserializeObject<IssueWorklogModel>(response.Content);

            return issueWorklog;
        }

        private RestClient CreateRestClient(Session session)
        {
            var client = new RestClient(Options.JiraApiUrl)
            {
                CookieContainer = new CookieContainer()
            };

            if (Options.UseBasicAuthentication)
            {
                var credentials = CredentialsStorage.GetCurrentUserCredentials();
                client.Authenticator = new HttpBasicAuthenticator(credentials.UserName, credentials.Password);
            }
            else
            {
                var sessionCookie = new Cookie()
                {
                    Domain = Credentials.JiraHostName,
                    Name = session.Name,
                    Value = session.Value,
                    HttpOnly = true
                };

                client.CookieContainer.Add(sessionCookie);
            }

            return client;
        }

        private RestRequest CreateRequest(string servicePath, Method method)
        {
            var request = new RestRequest(servicePath, method)
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = new NewtonsoftJsonSerializer()
            };

            return request;
        }
    }
}
