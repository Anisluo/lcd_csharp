using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCD.dataBase
{
    public static class ProjectMode
    {
        private static Database Database { get; set; } = new Database();

        public static int Insert(ProjectModeClass userIdMode)
        {
            //$"select * from project where UserID={id}";
            string
            Sql = $"select * from project where UserID={userIdMode.UserID} AND projectName='{userIdMode.projectName}' AND ModeType={userIdMode.ModeType}";
            System.Data.SQLite.SQLiteDataReader sqLiteData = Database.Reader(Sql);
            if (sqLiteData.Read())
            {
                ProjectModeClass sMode = new ProjectModeClass();
                sMode.ID = Int32.Parse(sqLiteData["ID"].ToString());
                sMode.projectName = sqLiteData["projectName"].ToString();
                sMode.ModeType = Int32.Parse(sqLiteData["ModeType"].ToString());
                Delete1(sMode.ID);

            }
            else
            {

            }

            //string Sql = $"select * from project where UserID={userIdMode.UserID}";
            //System.Data.SQLite.SQLiteDataReader sqLiteData = Database.Reader(Sql);
            //if (sqLiteData.Read())
            //{
            //    Sql = $"delete from project where UserID={userIdMode.UserID} ";
            //    Database.Command(Sql);
            //}
            Sql = $"insert into project (UserID, projectName,ModeType) values ({userIdMode.UserID},'{userIdMode.projectName}',{userIdMode.ModeType})";
            Database.Command(Sql);

            Sql = $"select * from project where UserID={userIdMode.UserID} AND projectName='{userIdMode.projectName}' AND ModeType={userIdMode.ModeType}";
            sqLiteData = Database.Reader(Sql);
            if (sqLiteData.Read())
            {
                ProjectModeClass sMode = new ProjectModeClass();
                sMode.ID = Int32.Parse(sqLiteData["ID"].ToString());
                sMode.projectName = sqLiteData["projectName"].ToString();
                sMode.ModeType = Int32.Parse(sqLiteData["ModeType"].ToString());
                return sMode.ID;
            }
            else
            {
                return -1;
            }
        }
        public static int Delete1(int id)
        {
            int ID = -1;
            string Sql = $"select * from project where ID={id}";
            System.Data.SQLite.SQLiteDataReader sqLiteData = Database.Reader(Sql);
            if (sqLiteData.Read())
            {
                ID = Int32.Parse(sqLiteData["ID"].ToString());
                TestData.Delete(ID);
                SpectrumData.Delete(ID);

            }

            Sql = $"delete from project where ID={id}";
            return Database.Command(Sql);
            //Delete(id);

        }
        public static  int Delete(int id)
        {
            int ID = -1;
            string Sql = $"select * from project where UserID={id}";
            System.Data.SQLite.SQLiteDataReader sqLiteData = Database.Reader(Sql);
            if (sqLiteData.Read())
            {
                ID = Int32.Parse(sqLiteData["ID"].ToString());
                TestData.Delete(ID);
                SpectrumData.Delete(ID);
                
            }

            Sql = $"delete from project where UserID={id}";
            return Database.Command(Sql);
            //Delete(id);
          
        }

        public static void RederList(int ID)
        {
            
            string SQL = $"select * from project where UserID ={ID}";// $"select *   from project where UserID={ID}";
            System.Data.SQLite.SQLiteDataReader sqLiteData = Database.Reader(SQL);
            while (sqLiteData.Read())
            {
                //Project.listBarCode[Project.listBarCode.Count-1].ProjectModes.Clear();
                ProjectModeClass sMode = new ProjectModeClass();
                sMode.ID = Int32.Parse(sqLiteData["ID"].ToString());

                sMode.projectName = sqLiteData["projectName"].ToString();
                sMode.ModeType = Int32.Parse(sqLiteData["ModeType"].ToString());
                Project.listBarCode[Project.listBarCode.Count-1].ProjectModes.Add(sMode);
            }


        }
    }

    public class ProjectModeClass 
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string projectName { get; set; }
        public int ModeType { get; set; }
        public List<TestDataMode> TestDataModes { get; set; }    = new List<TestDataMode>();
        public List<SpectrumDataMode> SpectrumDataModes { get; set; }=new List<SpectrumDataMode>();
    }
}
