using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using MahApps.Metro.Controls.Dialogs;

namespace gLhbViewer
{
	public class ViewModelLocator
	{
		public MainViewModel MainViewModel
		{
			get => SimpleIoc.Default.GetInstance<MainViewModel>();
		}

		public ViewModelLocator()
		{
			if (!ViewModelBase.IsInDesignModeStatic)
			{
				SimpleIoc.Default.Register<IDialogCoordinator, DialogCoordinator>();
			}
			SimpleIoc.Default.Register<MainViewModel>();
		}
	}
}