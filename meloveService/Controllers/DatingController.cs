using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using meloveService.DataObjects;
using meloveService.Models;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service.Security;

namespace meloveService.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.Admin)]
    public class DatingController : TableController<Dating>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            meloveContext context = new meloveContext();
            DomainManager = new EntityDomainManager<Dating>(context, Request, Services);
        }

        // GET tables/Dating
        public IQueryable<Dating> GetAllDating()
        {
            return Query(); 
        }

        // GET tables/Dating/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Dating> GetDating(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Dating/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Dating> PatchDating(string id, Delta<Dating> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Dating
        public async Task<IHttpActionResult> PostDating(Dating item)
        {
            Dating current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Dating/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteDating(string id)
        {
             return DeleteAsync(id);
        }

    }
}