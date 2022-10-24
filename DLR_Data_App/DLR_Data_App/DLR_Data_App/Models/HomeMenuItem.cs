using System;
using Xamarin.Forms;

namespace DLR_Data_App.Models
{
    public enum MenuItemType
    {
        CurrentProject,
        Projects,
        Sensortest,
        Settings,
        About,
        Logout,
        ProfilingList,
        CurrentProfiling,
        DistanceMeasuringDemo,
        VoiceRecognitionDemo,
        DrivingEasy,
        DrivingHard
    }
    public class HomeMenuItem
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public NavigationPage NavigationPage { get; set; }
    }
}
