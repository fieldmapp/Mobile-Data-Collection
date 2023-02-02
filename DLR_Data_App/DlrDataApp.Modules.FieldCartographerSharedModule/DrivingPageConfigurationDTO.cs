using DlrDataApp.Modules.Base.Shared;

namespace DlrDataApp.Modules.FieldCartographer.Shared
{
    public class DrivingPageConfigurationDTO
    {
        public DrivingPageConfigurationDTO()
        {

        }
        public DrivingPageConfigurationDTO(DrivingPageConfiguration configuration)
        {
            Configuration = JsonTranslator.GetJson(configuration);
            Id = configuration.Id;
        }
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int? Id { get; set; }
        public string Configuration { get; set; }
        [SQLite.Ignore]
        public DrivingPageConfiguration DrivingPageConfiguration
        {
            get
            {
                var conf = JsonTranslator.GetFromJson<DrivingPageConfiguration>(Configuration);
                conf.Id = Id;
                return conf;
            }
        }
    }
}