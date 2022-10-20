using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using RestSharp;
using RestSharp.Newtonsoft.Json.NetCore;

using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.Extensions;
using Sprint.Burndown.WebApp.Models;

using RestRequest = RestSharp.RestRequest;

namespace Sprint.Burndown.WebApp.ExternalServices
{
    public class ProductionScheduleService : IProductionScheduleService
    {
        private const string ApiKey = "21d0c1e3f4805932a10a5c4cab9b0e37";

        private const string GovServerUrl = "https://data.gov.ru";

        private const string WorkDaysApi = "/api/json/dataset/7708660670-proizvcalendar/version/{0}/content/";

        private const string ApiVersion = "20151123T183036";

        // sample: +https://data.gov.ru/api/json/dataset/7708660670-proizvcalendar/version/20151123T183036/content/?access_token=21d0c1e3f4805932a10a5c4cab9b0e37

        public Lazy<IList<HolidayModel>> GetSchedule()
        {
            return new Lazy<IList<HolidayModel>>(GetWorkDaysInternal);
        }

        private IList<HolidayModel> GetWorkDaysInternal()
        {
            var client = CreateRestClient();
            var request = CreateRequest(WorkDaysApi, Method.GET);
            var response = client.Execute<List<HolidayModel>>(request);

            return response.IsSuccessful()
                    ? JsonConvert.DeserializeObject<List<HolidayModel>>(response.Content)
                    : null;
        }

        private RestClient CreateRestClient()
        {
            return new RestClient(GovServerUrl);
        }

        private RestRequest CreateRequest(string servicePathFormat, Method method)
        {
            var servicePath = string.Format(servicePathFormat, ApiVersion);
            var request = new RestRequest(servicePath, method)
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = new NewtonsoftJsonSerializer()
            };

            request.AddQueryParameter("access_token", ApiKey);

            return request;
        }
    }
}
