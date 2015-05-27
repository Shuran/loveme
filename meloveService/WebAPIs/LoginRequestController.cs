using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace meloveService.WebAPIs
{
    [AuthorizeLevel(AuthorizationLevel.Anonymous)]
    public class LoginRequestController : ApiController
    {
        //Completed: Create the apiservices to obtain the masterkey
        public ApiServices Services { get; set; }

        //Completed: Debug
        //Bug: mHandler is null (Change private to public, and mHandler to handler)
        public IServiceTokenHandler handler { get; set; }
        //Completed: Create class LoginRequest
        public HttpResponseMessage Post(LoginRequest pLoginRequest)
        {
            //TODO: Check if the pLoginRequest provides the correct user information

            //Completed: Create LoginResult from service provider
            //Completed: Match the loginRequest with login information
            //Completed: Produce meaningful response in loginResult
            //Completed: Pass in the claims identity to function CreateLoginResult as new parameter
            ClaimsIdentity claimsId = new ClaimsIdentity();
            claimsId.AddClaim(new Claim(ClaimTypes.NameIdentifier, pLoginRequest.mUserName));
            LoginResult loginResult = new CustomLoginProvider(handler).CreateLoginResult(claimsId,Services.Settings.MasterKey);
            return this.Request.CreateResponse(HttpStatusCode.OK, loginResult);
        }
    }
}
