using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.Const;
using Sprint.Burndown.WebApp.Core;
using Sprint.Burndown.WebApp.Extensions;
using Sprint.Burndown.WebApp.ViewModels;

using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Sprint.Burndown.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/token")]
    public class TokenController : Controller
    {
        private IGlobalContext Context { get; }

        private IJiraFacade JiraFacade { get; }

        private ICredentialsStorage CredentialsStorage { get; }

        public TokenController(IGlobalContext context, IJiraFacade jiraFacade, ICredentialsStorage credentialsStorage)
        {
            Context = context;
            JiraFacade = jiraFacade;
            CredentialsStorage = credentialsStorage;
        }

        [HttpPost]
        public ActionResult<string> Create([FromBody]UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userData = ValidateCredentials(model.Username, model.Password);
            if (userData.HasValue)
            {
                CredentialsStorage.Add(userData.Value.token, userData.Value.credentials);

                return userData.Value.token;
            }
            
            return BadRequest();
        }

        private (string token, UserCredentials credentials)? ValidateCredentials(string username, string password)
        {
            var user = JiraFacade.GetUserInformation(username, password);

            if (user != null)
            {
                string avatarUrl = null;
                if (user.AvatarUrls != null)
                {
                    user.AvatarUrls.TryGetValue("32x32", out avatarUrl);
                }

                var name = user.Name ?? username.ExtractUsername() ?? user.DisplayName;
                var displayName = user.DisplayName ?? user.Name ?? username.ExtractUsername();

                var userToken = GenerateToken(user.Id, name, displayName, avatarUrl ?? Url.Content("~/images/user-account.png"));
                var userCredentials = CreateCredentials(user.Id, username, password, user.DisplayName);

                return (userToken, userCredentials);
            }

            return null;
        }

        private string GenerateToken(string userId, string userName, string dislayName, string avatarUrl)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(CustomClaimTypes.DisplayName, dislayName),
                new Claim(CustomClaimTypes.AvatarUrl, avatarUrl),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString()),
            };

            var simmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("77135F4BEC914BB3A76320FBD9D8B856"));
            var credentials = new SigningCredentials(simmetricKey, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken(new JwtHeader(credentials), new JwtPayload(claims));
            var userToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return userToken;
        }
        private UserCredentials CreateCredentials(string userId, string userName, string password, string displayName)
        {
            return new UserCredentials
            {
                UserName = userName,
                Password = password,
                DisplayName = displayName
            };
        }
    }
}