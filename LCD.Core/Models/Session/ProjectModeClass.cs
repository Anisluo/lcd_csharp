using System.Collections.Generic;

namespace LCD.dataBase
{
    public class ProjectModeClass
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string projectName { get; set; }
        public int ModeType { get; set; }
        public List<TestDataMode> TestDataModes { get; set; } = new List<TestDataMode>();
        public List<SpectrumDataMode> SpectrumDataModes { get; set; } = new List<SpectrumDataMode>();
    }
}
