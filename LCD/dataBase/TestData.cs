using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCD.dataBase
{
    public static class TestData
    {
        private static Database Database { get; set; } = new Database();
        public static void Insert(TestDataMode userIdMode)
        {

            //string Sql = $"select * from TestData where Project_id={userIdMode.Project_id}";
            //System.Data.SQLite.SQLiteDataReader sqLiteData = Database.Reader(Sql);
            //if (sqLiteData.Read())
            //{
            //    Sql = $"delete from TestData where UserID={userIdMode.Project_id} ";
            //    Database.Command(Sql);
            //}
            string Sql = $"insert into TestData (Project_id, Num,L,X,Y,Z,Cx,Cy,u,v,CCT,Time,Voltage,ElectricCurrent,Power,Remark)" +
                  $"values ({userIdMode.Project_id}, '{userIdMode.Num}','{userIdMode.L}','{userIdMode.X}','{userIdMode.Y}','{userIdMode.Z}','" +
                  $"{userIdMode.Cx}','{userIdMode.Cy}','{userIdMode.u}','{userIdMode.v}','{userIdMode.CCT}','{userIdMode.Time}','{userIdMode.Voltage}','{userIdMode.ElectricCurrent}','{userIdMode.Power}','{userIdMode.Remark}')";
            Console.WriteLine(Sql);
            Database.Command(Sql);


        }

        public static int Delete(int ID)
        {
            string Sql = "";
            if (ID != null)
            {
                Sql = $"delete from TestData where Project_id={ID}";
            }
            return Database.Command(Sql);
        }

        public static List<TestDataMode> RederList(int ID)
        {
            List<TestDataMode> testData=new List<TestDataMode>();
            Project.TestDataModes.Clear();
            string SQL = $"select * from TestData where Project_id={ID}";
            System.Data.SQLite.SQLiteDataReader sqLiteData = Database.Reader(SQL);
            while (sqLiteData.Read())
            {
                TestDataMode sMode = new TestDataMode();
                sMode.ID = Int32.Parse(sqLiteData["ID"].ToString());

                sMode.Num = sqLiteData["Num"].ToString();
                sMode.L = sqLiteData["L"].ToString();
                sMode.X = sqLiteData["X"].ToString();
                sMode.Y = sqLiteData["Y"].ToString();
                sMode.Z = sqLiteData["Z"].ToString();
                sMode.Cx = sqLiteData["Cx"].ToString();
                sMode.Cy = sqLiteData["Cy"].ToString();
                sMode.u = sqLiteData["u"].ToString();
                sMode.v = sqLiteData["v"].ToString();
                sMode.CCT = sqLiteData["CCT"].ToString();
                sMode.Time = sqLiteData["Time"].ToString();
                sMode.Voltage = sqLiteData["Voltage"].ToString();
                sMode.ElectricCurrent = sqLiteData["ElectricCurrent"].ToString();
                sMode.Power = sqLiteData["Power"].ToString();
                sMode.Remark = sqLiteData["Remark"].ToString();
                Project.TestDataModes.Add(sMode);
                testData.Add(sMode);
            }

            return testData;
        }
    }

    public class TestDataMode
    {
        public string Remark { get; set; }
        public int ID { get; set; }
        public int Project_id { get; set; }
        public string Num { get; set; }
        public string L { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
        public string Z { get; set; }
        public string Cx { get; set; }
        public string Cy { get; set; }
        public string u { get; set; }
        public string v { get; set; }
        public string CCT { get; set; }
        public string Time { get; set; }
        public string Voltage { get; set; }
        public string ElectricCurrent { get; set; }
        public string Power { get; set; }

    }
}
