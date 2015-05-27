using meloveService.Models;
using meloveService.DataObjects;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using meloveService.Security;

namespace meloveService.WebAPIs
{
    public class RegisterRequestController : ApiController
    {
        [AuthorizeLevel(AuthorizationLevel.Anonymous)]
        //Completed: Create the class RegisterRequest
        //Completed: Implement the Post function
        public HttpResponseMessage Post(RegisterRequest pRegisterRequest)
        {
            //Check if the Username is valid
            if (!Regex.IsMatch(pRegisterRequest.mUserName, "^[a-zA-Z0-9]{4,}$"))
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Username");
            }
            //Check if the Password is valid
            else if (pRegisterRequest.mPassword.Length < 8)
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Password");
            }
            //Check if the user exists already
            meloveContext context = new meloveContext();
            User user = context.Users.Where(a => a.mName == pRegisterRequest.mUserName).SingleOrDefault();
            if (user != null)
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, "User already exists");
            }
            //Register the user
            else
            {
                byte[] salt = PasswordUtility.generateSalt();

                User newUser = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    mName = pRegisterRequest.mUserName,
                    mSalt = salt,
                    mSaltedAndHashedPd = PasswordUtility.hash(pRegisterRequest.mPassword, salt)
                };

                context.Users.Add(newUser);
                context.SaveChanges();
                //Return the success code
                return this.Request.CreateResponse(HttpStatusCode.Created);
            }
        }
    }
}
