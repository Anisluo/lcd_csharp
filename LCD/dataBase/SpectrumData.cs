using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCD.dataBase
{
    public static class SpectrumData
    {
        private static Database Database { get; set; } = new Database();
        public static void Insert(SpectrumDataMode userIdMode)
        {

            //string Sql = $"select * from SpectrumData where Project_id={userIdMode.Project_id}";
            //System.Data.SQLite.SQLiteDataReader sqLiteData = Database.Reader(Sql);
            //if (sqLiteData.Read())
            //{
            //    Sql = $"delete from SpectrumData where UserID={userIdMode.Project_id} ";
            //    Database.Command(Sql);
            //}
            string Sql = $"insert into SpectrumData (Project_id, DataName,dataValue) values ({userIdMode.Project_id},'{userIdMode.DataName}','{userIdMode.dataValue}')";
            Database.Command(Sql);

            
        }

        public static int Delete(int ID)
        {
            string Sql = "";
            if (ID != null)
            {
                Sql = $"delete from SpectrumData where Project_id={ID}";
            }
            return Database.Command(Sql);
        }

        public static List<SpectrumDataMode> RederList(int ID)
        {
            List<SpectrumDataMode> spectrum=new List<SpectrumDataMode>();
            Project.ListSpectrumData.Clear();
            string SQL = $"select * from SpectrumData where Project_id={ID}";
            System.Data.SQLite.SQLiteDataReader sqLiteData = Database.Reader(SQL);
            while (sqLiteData.Read())
            {
                SpectrumDataMode sMode = new SpectrumDataMode();
                sMode.ID = Int32.Parse(sqLiteData["ID"].ToString());
                sMode.DataName = sqLiteData["DataName"].ToString();
                sMode.dataValue = sqLiteData["dataValue"].ToString();
                Project.ListSpectrumData.Add(sMode);
                spectrum.Add(sMode);
            }

            return spectrum;
        }
    }

    public class SpectrumDataMode
    {
        public int ID { get; set; }
        public int Project_id { get; set; }
        public string DataName { get; set; }
        public string dataValue { get; set; }
    }
}
