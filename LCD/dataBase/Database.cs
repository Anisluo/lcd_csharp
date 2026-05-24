using System;
using System.Collections.Generic;
using System.IO;
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
            // 确保 database 目录存在（SQLite 不会自动创建上层目录，path 不存在时 Open 会抛异常）
            string dbDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database");
            if (!Directory.Exists(dbDir)) Directory.CreateDirectory(dbDir);

            DBConnection = new SQLiteConnection(DBPath);
            DBConnection?.Open();
            EnsureSchema();
        }

        // 老部署里 MyData.db 由人手工初始化；空文件部署时点测试会因 "no such table" 抛
        // SQLiteException → 后台 tesThread 未捕获 → AppDomain.UnhandledException → 闪退。
        // 这里在 Open 后幂等建表，已有 schema 的旧库不受影响。
        private static void EnsureSchema()
        {
            if (DBConnection == null) return;
            string[] ddl = new[]
            {
                @"CREATE TABLE IF NOT EXISTS user_id (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    BarCode TEXT,
                    CreationTime TEXT)",
                @"CREATE TABLE IF NOT EXISTS project (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserID INTEGER,
                    projectName TEXT,
                    ModeType INTEGER)",
                @"CREATE TABLE IF NOT EXISTS TestData (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Project_id INTEGER,
                    Num TEXT, L TEXT, X TEXT, Y TEXT, Z TEXT,
                    Cx TEXT, Cy TEXT, u TEXT, v TEXT, CCT TEXT,
                    Time TEXT, Voltage TEXT, ElectricCurrent TEXT, Power TEXT, Remark TEXT,
                    Low TEXT, High TEXT, RiseTime TEXT, FallTime TEXT,
                    CoordX TEXT, CoordY TEXT, CoordZ TEXT, CoordU TEXT, CoordV TEXT,
                    Lcolor TEXT, Acolor TEXT, Bcolor TEXT, La TEXT, Lb TEXT, CT TEXT)",
                @"CREATE TABLE IF NOT EXISTS SpectrumData (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Project_id INTEGER,
                    DataName TEXT,
                    dataValue TEXT)"
            };
            foreach (var sql in ddl)
            {
                using (var cmd = new SQLiteCommand(sql, DBConnection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
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
