﻿namespace DLR_Data_App.Models
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
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
