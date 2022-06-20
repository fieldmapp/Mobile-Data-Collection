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
        VoiceRecognitionDemo,
        DrivingEasy,
        LowYieldCartograph
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
