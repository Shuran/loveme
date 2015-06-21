using System;
using System.Threading.Tasks;
using meloveShared.BL;
using Newtonsoft.Json.Linq;
using meloveShared.DL;

namespace meloveShared.DAL
{
	public interface IUserServiceRemote : IUserService
	{
		void GetUserRemote(string pName, string pPassword);
		void SetUserRemoteCallback(WebCallback pCallback);
	}
}

