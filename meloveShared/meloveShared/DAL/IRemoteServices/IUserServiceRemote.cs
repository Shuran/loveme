using System;
using System.Threading.Tasks;
using meloveShared.BL;
using Newtonsoft.Json.Linq;

namespace meloveShared.DAL
{
	public interface IUserServiceRemote : IUserService
	{
		Task<JObject> GetUserRemote(string pName, string pPassword);
	}
}

