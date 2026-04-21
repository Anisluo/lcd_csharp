using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace LCD.dataBase
{
    public class Database
    {
        public static string DBPath { get; set; } = "data source=" + AppDomain.CurrentDomain.BaseDirectory + @"database\MyData.db";
        private static SQLiteConnection DBConnection = null;
        public static void Open()
        {
            DBConnection = new SQLiteConnection(DBPath);
            DBConnection?.Open();
        }
        /// <summary>
        /// 增删改
        /// </summary>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public int Command(string SQL)
        {
            int ret = -1;
            SQLiteCommand cmd=new SQLiteCommand(SQL, DBConnection);
            ret = cmd.ExecuteNonQuery();
            return ret;
        }

        public SQLiteDataReader Reader(string SQL)
        {
            SQLiteCommand cmd = new SQLiteCommand(SQL, DBConnection);
            return cmd.ExecuteReader();
           
        }
        //public
    }
}
