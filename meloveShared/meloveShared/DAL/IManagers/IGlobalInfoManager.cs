using System;
using meloveShared.BL;

namespace meloveShared.DAL
{
	// This is the manager class that stores and interacts with data related to the whole app
	public interface IGlobalInfoManager
	{
		User mCurrentUser { get; set; }
	}
}

