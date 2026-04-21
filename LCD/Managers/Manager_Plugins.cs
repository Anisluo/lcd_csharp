using LCD.Ctrl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using VisionCore;

namespace LCD.Managers
{
    public class Manager_Plugins
    {
        public static List<MachinePluginInfo> MachineInfos { get; set; } = new List<MachinePluginInfo>();
        /// <summary>插件 路径</summary>
        public static readonly string PlugInsDir = Path.Combine(Environment.CurrentDirectory, "Plugins\\");

        public static void InitPlugin()
        {
            MachineInfos = new List<MachinePluginInfo>();
            InitPluginLocal();
            if (Directory.Exists(PlugInsDir) == false) return;//判断是否存在
            //判断是否是UI.dll
            foreach (var dllFile in Directory.GetFiles(PlugInsDir, "*.dll"))
            {
                try
                {
                    FileInfo fi = new FileInfo(dllFile);
                    //判断是否是Plugin.xxxxxxx.dll
                    if (!fi.Name.StartsWith("Plugin.") || !fi.Name.EndsWith(".dll")) continue;
                    Assembly assemPlugIn = Assembly.Load(File.ReadAllBytes(fi.FullName));//该方法不占用文件，不知道能不能调试
                    //Assembly assemPlugIn = AppDomain.CurrentDomain.Load(Assembly.LoadFile(fi.FullName).GetName());// 该方法会占用文件 但可以调试
                    //判断是否包含ObjBase
                    foreach (Type type in assemPlugIn.GetTypes())
                    {
                        //是ObjBase的子类
                        if (typeof(TestMachine).IsAssignableFrom(type))
                        {
                            MachinePluginInfo info = new MachinePluginInfo();
                            //获取插件名称
                            if (GetPluginInfo(assemPlugIn, type, ref info))
                            {
                                MachineInfos.Add(info);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(dllFile + ":" + ex.ToString());
                }
            }
        }

        private static bool GetPluginInfo(Assembly assemPlugIn, Type type, ref MachinePluginInfo info)
        {
            try
            {
                object[] categoryObjs = type.GetCustomAttributes(typeof(CategoryAttribute), true);
                object[] dispNameObjs = type.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                info.Category = ((CategoryAttribute)categoryObjs[0]).Category;
                info.Name = ((DisplayNameAttribute)dispNameObjs[0]).DisplayName;
                info.MachineObjType = type;
                return true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
            return false;
        }

        private static void InitPluginLocal()
        {
            Assembly ass = Assembly.GetExecutingAssembly();
            foreach (Type type in ass.GetTypes())
            {
                //是ObjBase的子类
                if (typeof(TestMachine).IsAssignableFrom(type))
                {
                    MachinePluginInfo info = new MachinePluginInfo();
                    //获取插件名称
                    if (GetPluginInfo(ass, type, ref info))
                    {
                        MachineInfos.Add(info);
                    }
                }
            }
        }
    }
}
