using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BoardGamesApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DemoTokenController : Controller
    {
        private readonly IConfiguration _configuration;

        public DemoTokenController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // NOT FOR PRODUCTION USE!!!
        // you will need a robust auth implementation for production
        // i.e. use ASP.NET Identity with manual token generation or try IdentityServer
        [AllowAnonymous]
        [HttpGet("/get-token")]
        public IActionResult GenerateToken(
            string name = "aspnetcore-api-demo", bool admin = false)
        {
            var jwt = JwtTokenGenerator.Generate(
                name, admin,
                _configuration["Tokens:Issuer"],
                _configuration["Tokens:Key"]);

            return Ok(jwt);
        }
    }
}
