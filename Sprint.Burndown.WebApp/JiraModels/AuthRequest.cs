using System;
using Newtonsoft.Json;

namespace Sprint.Burndown.WebApp.JiraModels 
{
    public class AuthRequest
    {
        [JsonProperty("username")]
        public String UserName { get; set; }

        [JsonProperty("password")]
        public String Password { get; set; }
    }
}
