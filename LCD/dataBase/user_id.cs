using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LCD.dataBase
{
    public class user_id
    {
        /// <summary>
        /// 存在
        /// </summary>
        public bool Exist=false;
        public user_id()
        {
            Project.listBarCode.Clear();
            RederList();
        }
        private Database Database { get; set; }=new Database();

        public int Insert(UserIdMode userIdMode)
        {
            for (int i = 0; i < Project.listBarCode.Count; i++)
            {
                if (userIdMode.BarCode==Project.listBarCode[i].BarCode)
                {
                    Exist = true;
                    //DeleteAll(Project.listBarCode[i].ID, Project.listBarCode[i].BarCode);
                    //ProjectMode.Delete(Project.listBarCode[i].ID);
                    //Delete(Project.listBarCode[i].BarCode);
                    //return Project.listBarCode[i].ID;
                }
            }
            string Sql = "";
            if (!Exist)
            {
                Sql = $"insert into user_id (BarCode, CreationTime) values ('{userIdMode.BarCode}', '{userIdMode.CreationTime}')";
                Database.Command(Sql);
            }
            else
            {

            }

            

            Sql = $"select * from user_id where BarCode= '{userIdMode.BarCode}'";
            SQLiteDataReader sqLiteData = Database.Reader(Sql);
            if (sqLiteData.Read())
            {
                UserIdMode sMode = new UserIdMode();
                sMode.BarCode = sqLiteData["BarCode"].ToString();
                sMode.ID = Int32.Parse(sqLiteData["ID"].ToString());
                return sMode.ID;
            }
            else
            {
                return -1;
            }
        }

        public void DeleteAll(int ID,string BarCode)
        {
            ProjectMode.Delete(ID);
            Delete(BarCode);
        }
        public int Delete(string BarCode)
        {
            string Sql = "";
            Sql = $"delete from user_id where BarCode='{BarCode}'";
            //if (userIdMode.BarCode!=null&&userIdMode.CreationTime!=null)
            //{
            //    Sql = $"delete from user_id where BarCode={userIdMode.BarCode} AND CreationTime={userIdMode.CreationTime}";
            //}
            //if (userIdMode.BarCode!=null && userIdMode.CreationTime == null)
            //{
            //    Sql = $"delete from user_id where BarCode={userIdMode.BarCode}";
            //}
            //else if (userIdMode.CreationTime != null && userIdMode.BarCode == null)
            //{
            //    Sql = $"delete from user_id where CreationTime={userIdMode.CreationTime}";
            //}

            return Database.Command(Sql);
        }

        public void RederList()
        {
            Project.listBarCode.Clear();
            string SQL = "select *   from user_id order by ID desc";
            SQLiteDataReader sqLiteData= Database.Reader(SQL);
            while (sqLiteData.Read())
            {
                UserIdMode sMode=new UserIdMode();
                sMode.ID = Int32.Parse(sqLiteData["ID"].ToString());
                sMode.BarCode = sqLiteData["BarCode"].ToString();
                
                Project.listBarCode.Add(sMode);

                ProjectMode.RederList(sMode.ID);
            }

            
        }
       
    }

    public class UserIdMode
    {
        public int ID { get; set; }
        public string BarCode { get; set; }
        public string CreationTime { get; set; }

        public List<ProjectModeClass> ProjectModes { get; set; }=new List<ProjectModeClass>();
    }
}
