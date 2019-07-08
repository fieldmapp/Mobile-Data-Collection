using DLR_Data_App.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProjectPage : TabbedPage
	{
    ProjectViewModel viewModel;

    public ProjectPage ()
		{
			InitializeComponent ();
      viewModel = new ProjectViewModel();

      BindingContext = viewModel;
		}

    /**
     * Refresh view
     */
    protected override void OnAppearing()
    {
      base.OnAppearing();
      
      viewModel.UpdateView();
    }
  }
}