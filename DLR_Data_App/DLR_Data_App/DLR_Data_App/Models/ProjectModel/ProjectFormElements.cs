using SQLite;

namespace DLR_Data_App.Models.ProjectModel
{
  public class ProjectFormElements
  {
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Label { get; set; }
    //public List<string> Label { get; set; }
    public string Hint { get; set; }
    public string DefaultValue { get; set; }
    public bool ReadOnly { get; set; }
    public bool Required { get; set; }
    public string RequiredText { get; set; }
    //public List<string> RequiredText { get; set; }
    public string Relevance { get; set; }
    public string Constraint { get; set; }
    public string InvalidText { get; set; }
    //public List<string> InvalidText { get; set; }
    public string Calculate { get; set; }

    public string Options { get; set; }
    public bool Cascading { get; set; }
    public string Other { get; set; }
    //public List<string> Other { get; set; }
    public string Appearance { get; set; }
    public string Metadata { get; set; }
    //public List<string> Metadata { get; set; }
    public string Type { get; set; }
  }
}
