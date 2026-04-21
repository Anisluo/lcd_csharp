using System.Collections.Generic;

namespace LCD.dataBase
{
    public class UserIdMode
    {
        public int ID { get; set; }
        public string BarCode { get; set; }
        public string CreationTime { get; set; }

        public List<ProjectModeClass> ProjectModes { get; set; } = new List<ProjectModeClass>();
    }
}
