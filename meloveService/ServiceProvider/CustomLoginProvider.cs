using Microsoft.WindowsAzure.Mobile.Service.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using meloveService.ServiceProvider;

namespace meloveService.WebAPIs
{
    public class CustomLoginProvider : LoginProvider
    {
        public const string ProviderName = "melove";
        public CustomLoginProvider(IServiceTokenHandler tokenHandler):base(tokenHandler)
        {
            this.TokenLifetime = new TimeSpan(30, 0, 0, 0);
        }

        //Completed: Comment out the CreateLoginResult function
        /*
        public LoginResult CreateLoginResult(LoginRequest loginRequest)
        {
            return new LoginResult();
        }
         * */

        //Completed: Add in the override functions to inherit from the abstract class
        //Bug: Error	2	'meloveService.WebAPIs.CustomLoginProvider' does not implement inherited abstract member 'Microsoft.WindowsAzure.Mobile.Service.Security.LoginProvider.ParseCredentials(Newtonsoft.Json.Linq.JObject)'	C:\Users\Shuran\Desktop\Projects\Melove\meloveService\ServiceProvider\CustomLoginProvider.cs	9	18	meloveService
        public override string Name
        {
            get { return ProviderName; }
        }
        
        public override ProviderCredentials CreateCredentials(ClaimsIdentity claimsIdentity)
        {
            //TODO-working-on: debug
            //Bug: NotImplementedException() is thrown. (These functions would be called from CreateLoginResult(claimsId,MasterKey) in LoginRequestController)
            //throw new NotImplementedException();
            
            if (claimsIdentity == null)
            {
                throw new ArgumentNullException("claimsIdentity");
            }
            return new CustomLoginProviderCredentials 
            {
                UserId = this.TokenHandler.CreateUserId(this.Name,claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value)
            };
        }

        public override void ConfigureMiddleware(Owin.IAppBuilder appBuilder, Microsoft.WindowsAzure.Mobile.Service.ServiceSettingsDictionary settings)
        {
            return;
        }

        public override ProviderCredentials ParseCredentials(Newtonsoft.Json.Linq.JObject serialized)
        {
            if (serialized == null)
            {
                throw new ArgumentNullException("serialized");
            }

            return serialized.ToObject<CustomLoginProviderCredentials>();
        }
    }
}
