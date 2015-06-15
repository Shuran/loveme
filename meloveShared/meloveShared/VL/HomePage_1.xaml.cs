using Xamarin.Forms;

namespace meloveShared.VL
{
	public partial class HomePage_1 : ContentPage
	{
		public HomePage_1 ()
		{
			InitializeComponent ();
		}

		protected override void OnAppearing()
		{
			VLGlobalInfoManager.mInstance.mCurrentPage = PageNameEnum.HomePage_1;
		}
	}
}

