using SQLite;

namespace DLR_Data_App.Models.ProjectModel
{
  public class ProjectFormMetadata
  {
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int Version { get; set; }
    public string Languages { get; set; }
    //public List<string> Languages { get; set; }
    public string OptionsPresets { get; set; }
    //public List<string> OptionsPresets { get; set; }
    public string Htitle { get; set; }
    public string InstanceName { get; set; }
    public string PublicKey { get; set; }
    public string SubmissionUrl { get; set; }
  }
}
