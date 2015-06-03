using System;
using meloveShared.DAL;
using System.Threading.Tasks;
using meloveShared.BL;

namespace meloveShared.DL
{
	public class DatabaseServiceLocal : DatabaseService, IDatabaseServiceLocal
	{
		//Singleton Set-up
		private static DatabaseServiceLocal mDatabaseServiceLocal;
		public static DatabaseServiceLocal mInstance
		{
			get
			{
				if (mDatabaseServiceLocal == null)
				{
					mDatabaseServiceLocal = new DatabaseServiceLocal ();
				}

				return mDatabaseServiceLocal;
			}
		}
		private DatabaseServiceLocal()
		{
			
		}

		//Implementation of the interface
	}
}

