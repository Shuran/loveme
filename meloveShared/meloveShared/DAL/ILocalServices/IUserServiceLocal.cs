using System;
using System.Threading.Tasks;
using meloveShared.BL;

namespace meloveShared.DAL
{
	public interface IUserServiceLocal
	{
		void SaveUserLocal(LoggedInUser pLoggedInUser, string pPassword);
	}
}

