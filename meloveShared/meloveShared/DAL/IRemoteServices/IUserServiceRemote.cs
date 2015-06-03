using System;
using System.Threading.Tasks;
using meloveShared.BL;

namespace meloveShared.DAL
{
	public interface IUserServiceRemote : IUserService
	{
		Task<User> GetUserRemote(string pName, string pPassword);
	}
}

