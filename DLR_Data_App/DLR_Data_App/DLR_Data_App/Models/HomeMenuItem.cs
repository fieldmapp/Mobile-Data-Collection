using System;
using Xamarin.Forms;

namespace DLR_Data_App.Models
{
    public class HomeMenuItem
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public NavigationPage NavigationPage { get; set; }
    }
}
