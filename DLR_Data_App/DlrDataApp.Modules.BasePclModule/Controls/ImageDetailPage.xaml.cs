//Main contributors: Maximilian Enderling
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DlrDataApp.Modules.Base.Shared.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ImageDetailPage : ContentPage
	{
		public ImageDetailPage(ImageSource imageSource)
        {
            InitializeComponent();
            Image.Source = imageSource;
		}
    }
}