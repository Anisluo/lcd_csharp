using System;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace VisionCore
{
    [Serializable]
    public class Proj
    {
        /// <summary>通信列表</summary>
        public List<ECom> mECom;
        /// <summary>相机列表</summary>
        public List<CamerasBase> mCamera;
        /// <summary>系统变量</summary>
        public List<DataCell> mSysVar;
        /// <summary>标定变量 </summary>
        public List<DataCell> mCalVar;
        /// <summary>流程变量</summary>
        private List<DataCell> mSloVar = new List<DataCell>();
        /// <summary>模块基类列表</summary>
        public List<ObjBase> mObjBase = new List<ObjBase>();
        /// <summary>模块信息列表</summary>
        public List<ModInfo> mModInfo { get; private set; } = new List<ModInfo>();
        /// <summary>流程信息</summary>
        public ProjInfo mProjInfo { set; get; } = new ProjInfo();
        /// <summary>运行模式 </summary>
        public RunMode mRunMode { set; get; } = RunMode.停止执行;
        /// <summary>运行方式 </summary>
        public RunType mRunType { set; get; } = RunType.主动执行;
        /// <summary>显示启用 </summary>
        public bool mIsShow { set; get; } = true;
        /// <summary>线程控制</summary>
        [NonSerialized]
        public Thread mThread;
        /// <summary>线程运行条件 </summary>
        [NonSerialized]
        public bool mThreadStatus = false;
        /// <summary>流程控制</summary>
        [NonSerialized]
        public AutoResetEvent mAutoResetEvent = new AutoResetEvent(false);
        /// <summary>流程建立</summary>
        public Proj()
        {
            mThread = new Thread(Process);
            mThread.IsBackground = true;
            mThread.Start();
        }
        public void Start()
        {
            mThreadStatus = true;
            mAutoResetEvent.Set();
        }
        public void Stop()
        {
            mThreadStatus = false;
            Thread.Sleep(10);
            if (mECom == null) { return; }
            foreach (ECom com in mECom)//也许没必要停止所有仪器
            {
                try
                {
                    if (com.IsConnected) { com.StopRecStrSignal(); }
                }
                catch { }
            }
        }
        /// <summary>TODO:获取线程状态</summary>
        public bool GetThreadStatus()
        {
            return mThreadStatus;
        }
        /// <summary>TODO:更新流程名</summary>
        public void ShowName()
        {
            foreach (ObjBase item in mObjBase)
            {
                item.Name = mProjInfo.Name;
            }
        }
        /// <summary>TODO:初始化-反序列化未初始的值</summary>
        public void SetInfo()
        {
            mAutoResetEvent = new AutoResetEvent(false);
            mThread = new Thread(Process);
            mThread.IsBackground = true;
            mThread.Start();
            foreach (ObjBase Mod in mObjBase)//这个是否应该放在线程前面
            {
                //初始化未序列化的值
                Mod.SetInfo();
                Mod.mHTimer = new HTimer();
                Mod.ModInfo.CostTime = 0;
                Mod.mCamera = mCamera;
                Mod.mECom = mECom;
                Mod.mSysVar = mSysVar;
                Mod.mSloVar = mSloVar;
            }
            GerInfoList();
        }
        /// <summary>
        /// 运行日志-代表了当前需要执行的模块名称
        /// </summary>
        string GoStr = "None";
        bool IsRun = true, IsFor = true, IsAdd = false;
        /// <summary>
        /// 循环模块
        /// </summary>
        List<ObjBase> ForObj = new List<ObjBase>();
        /// <summary>
        /// 循环模块是否已识别
        /// </summary>
        Dictionary<string, bool> ForBoolDic = new Dictionary<string, bool>();

        /// <summary>
        /// 每个流程执行的子模块
        /// </summary>
        Dictionary<string, List<ObjBase>> ForBaseDic = new Dictionary<string, List<ObjBase>>();
        /// <summary>TODO:流程运行</summary>
        public void Process()
        {
            mThreadStatus = false;
            while (true)
            {
                if (mThreadStatus == false)
                {
                    mAutoResetEvent.WaitOne();//阻塞等待--应该是减少资源消耗
                }
                else
                {
                    Log.Info(mProjInfo.Name + ":运行开始*:↓");
                    HTimer mHTimer = new HTimer(true);// 启动计时器 
                    GoStr = "None";//运行日志清空
                    IsRun = true;//正在运行
                    IsFor = true;//正在循环
                    IsAdd = false;//是否添加模块--避免添加循环模块

                    //已执行的模块清空
                    if (ForBaseDic != null)
                    {
                        ForBaseDic.Clear();
                    }

                    //已执行变量清空
                    if (ForBoolDic != null)
                    {
                        ForBoolDic.Clear();
                    }

                    //模块循环拆分-这一步暂时不需要
                    foreach (ObjBase item in mObjBase)
                    {
                        if (item.ModInfo.Name.StartsWith("循环开始"))//循环模块分解
                        {
                            IsAdd = true;
                            ForObj = new List<ObjBase>();
                            ForBoolDic.Add(item.ModInfo.Name, false);
                        }

                        if (IsAdd)//是否添加到执行过的模块
                        {
                            ForObj.Add(item);
                        }

                        if (item.ModInfo.Name.StartsWith("循环结束"))
                        {
                            IsAdd = false;
                            ForBaseDic.Add(item.ModInfo.Name, ForObj);
                        }
                    }

                    ///模块执行
                    for (int i = 0; i < mObjBase.Count; ++i)
                    {
                        GoStr = "None";
                        ObjBase obj = mObjBase[i];
                        obj.ModInfo.Result = obj.Enable;
                        obj.ModInfo.State = ModState.None;
                        if (obj.Enable & !Sol.IsStop)
                        {//如果流程勾选
                            obj.mHTimer.Start(); // 启动计时器
                            obj.ModInfo.State = ModState.None;
                            GoStr = obj.ModInfo.Name.Contains(GoStr) ? "None" : GoStr;

                            //流程模块判断
                            if (obj.ModInfo.Name.StartsWith("循环开始"))
                            {//循环模块
                                if ((ForBoolDic.ContainsKey(obj.ModInfo.Name)))
                                {
                                    ForRun(obj.ModInfo.Name);//直接运行循环模块
                                    i = mObjBase.FindIndex(c => c.ModInfo.Name == obj.ModInfo.Name.Replace("开始", "结束"));
                                }
                            }
                            else if (IsRun & obj.ModInfo.Name.StartsWith("如果") || IsRun & obj.ModInfo.Name.StartsWith("否则") || IsRun & obj.ModInfo.Name.StartsWith("跳转"))
                            {//条件模块
                                if (GoStr == "None")
                                {
                                    obj.ModInfo.State = ModState.Start;
                                    obj.RunObj();
                                    IsRun = obj.ModInfo.Name.StartsWith("跳转") ? true : obj.ModInfo.Result;
                                    GoStr = (obj.ModInfo.Result & obj.ModInfo.Name.StartsWith("跳转")) ? obj.ModInfo.GoLable : "None";
                                }
                            }
                            else if (IsRun & (GoStr == "None" & !obj.ModInfo.Name.StartsWith("如果") || GoStr == "None" & !obj.ModInfo.Name.StartsWith("否则")))
                            {//条件模块
                                obj.ModInfo.State = ModState.Start;
                                obj.RunObj();
                                IsRun = obj.ModInfo.Name.StartsWith("循环开始") ? true : obj.ModInfo.Result;
                            }
                            else if (!IsRun & obj.ModInfo.Name.StartsWith("否则"))
                            {//条件模块
                                if (GoStr == "None")
                                {
                                    obj.ModInfo.State = ModState.Start;
                                    obj.RunObj();
                                    IsRun = obj.ModInfo.Name.StartsWith("跳转") ? true : obj.ModInfo.Result;
                                    GoStr = (obj.ModInfo.Result & obj.ModInfo.Name.StartsWith("跳转")) ? obj.ModInfo.GoLable : "None";
                                }
                            }
                            else if (obj.ModInfo.Name == GoStr)
                            {//如果模块名称等于GoStr-用于跳转的
                                obj.ModInfo.State = ModState.Start;
                                obj.RunObj();
                            }
                            else
                            {//是否模块结束
                                IsRun = obj.ModInfo.Name.StartsWith("结束") ? true : false;
                            }

                            if (IsRun & GoStr == "None")
                            {
                                switch (obj.ModInfo.State)
                                {
                                    case ModState.OK:
                                        break;
                                    case ModState.NG:
                                        Log.Info(mProjInfo.Name + ":" + obj.ModInfo.Name + ":运行失败! " + obj.ModInfo.CostTime.ToString("F0") + "ms");
                                        break;
                                    case ModState.NoImage:
                                        Log.Info(mProjInfo.Name + ":" + obj.ModInfo.Name + ":未选图像! " + obj.ModInfo.CostTime.ToString("F0") + "ms");
                                        break;
                                }
                            }

                            obj.mHTimer.Stop();  // 停止当前模块计时器
                            obj.ModInfo.CostTime = obj.mHTimer.GetMilliSecond;

                            if (GoStr != "None")
                            {
                                //如果流程控制不为空则找到该模块的位置-重新执行？
                                int index = mObjBase.FindIndex(c => c.ModInfo.Name == GoStr);
                                for (int j = 0; j < (index - i); ++j)
                                {
                                    mObjBase[i + j + 1].ModInfo.State = ModState.None;
                                }
                                mObjBase[index].ModInfo.State = ModState.OK;//而循环模块变为已经运行
                                i = index;//运行位置变为循环模块的位置
                            }
                        }
                    }

                    //显示所有图像-目前大可不必
                    List<DataCell> DispalyImage = mSloVar.FindAll(e => e.mDataMode == DataMode.图像);
                    if (DispalyImage.Count >= 0)
                    {
                        foreach (DataCell cell in DispalyImage)
                        {
                            if (cell.mDataValue != null)
                            {
                                ShowMsg.SendImage(mProjInfo.Name, (byte[])cell.mDataValue);
                            }
                        }
                    }
                    else
                    {
                        Log.Info(mProjInfo.Name + ":获取图像*:失败! " + mProjInfo.CostTime.ToString("F0") + "ms");
                    }

                    mHTimer.Stop();  // 停止计时器
                    mIsShow = false;
                    Thread.Sleep(30);
                    ChangeEvent();//模块已修改
                    Application.DoEvents();
                    if (mRunMode == RunMode.单次执行)
                    {
                        Stop();
                    }
                    Log.Info(mProjInfo.Name + ":运行结束*:" + mHTimer.GetMilliSecond.ToString("F0") + "ms" + "");
                }
            }
        }
        private void ForRun(string objName)
        {
            IsFor = true;
            List<ObjBase> mForObjBase = ForBaseDic.First(c => c.Key == objName.Replace("开始", "结束")).Value;
            while (IsFor)
            {
                IsRun = true;
                for (int i = 0; i < mForObjBase.Count; ++i)
                {
                    ObjBase obj = mForObjBase[i];

                    if (obj.Enable & !Sol.IsStop)
                    {
                        obj.mHTimer.Start(); // 启动计时器
                        obj.ModInfo.State = ModState.None;
                        GoStr = obj.ModInfo.Name.Contains(GoStr) ? "None" : GoStr;
                        if (IsRun & obj.ModInfo.Name.StartsWith("如果") || IsRun & obj.ModInfo.Name.StartsWith("否则") || IsRun & obj.ModInfo.Name.StartsWith("跳转"))
                        {
                            if (GoStr == "None")
                            {
                                obj.ModInfo.State = ModState.Start;
                                obj.RunObj();
                                IsRun = obj.ModInfo.Name.StartsWith("跳转") ? true : obj.ModInfo.Result;
                                GoStr = (obj.ModInfo.Result & obj.ModInfo.Name.StartsWith("跳转")) ? obj.ModInfo.GoLable : "None";
                            }
                        }
                        else if (IsRun & (GoStr == "None" & !obj.ModInfo.Name.StartsWith("如果") || GoStr == "None" & !obj.ModInfo.Name.StartsWith("否则")))
                        {
                            obj.ModInfo.State = ModState.Start;
                            obj.RunObj();
                            IsRun = obj.ModInfo.Name.StartsWith("循环开始") ? true : obj.ModInfo.Result;
                        }
                        else if (!IsRun & obj.ModInfo.Name.StartsWith("否则"))
                        {
                            if (GoStr == "None")
                            {
                                obj.ModInfo.State = ModState.Start;
                                obj.RunObj();
                                IsRun = obj.ModInfo.Name.StartsWith("跳转") ? true : obj.ModInfo.Result;
                                GoStr = (obj.ModInfo.Result & obj.ModInfo.Name.StartsWith("跳转")) ? obj.ModInfo.GoLable : "None";
                            }
                        }
                        else if (obj.ModInfo.Name == GoStr)
                        {
                            obj.ModInfo.State = ModState.Start;
                            obj.RunObj();
                        }
                        else
                        {
                            IsRun = obj.ModInfo.Name.StartsWith("结束") ? true : false;
                        }


                        if (IsRun & GoStr == "None")//异常-一般不会出现在这
                        {
                            switch (obj.ModInfo.State)
                            {
                                case ModState.OK:
                                    break;
                                case ModState.NG:
                                    Log.Info(mProjInfo.Name + ":" + obj.ModInfo.Name + ":运行失败! " + obj.ModInfo.CostTime.ToString("F0") + "ms");
                                    break;
                                case ModState.NoImage:
                                    Log.Info(mProjInfo.Name + ":" + obj.ModInfo.Name + ":未选图像! " + obj.ModInfo.CostTime.ToString("F0") + "ms");
                                    break;
                            }
                        }

                        if (obj.ModInfo.Name.StartsWith("循环开始"))
                        {
                            if (obj.ModInfo.Result)
                            {
                                IsFor = !obj.ModInfo.Result;
                            }
                        }
                        if (obj.ModInfo.Name.StartsWith("循环结束"))
                        {
                            if (!IsFor)
                            {

                            }
                        }
                        obj.mHTimer.Stop();  // 停止计时器
                        obj.ModInfo.CostTime = obj.mHTimer.GetMilliSecond;
                        if (GoStr != "None")//运行完成
                        {
                            GoStr = "None";
                            int index = mObjBase.FindIndex(c => c.ModInfo.Name == GoStr);
                            mObjBase[index].ModInfo.State = ModState.OK;
                            for (int j = 0; j < (index - i); ++j)
                            {
                                mObjBase[i + j + 1].ModInfo.State = ModState.None;
                            }
                            i = index;
                        }
                    }
                }
            }
        }
        /// <summary>TODO:增加新的模块到流程列表</summary>
        public void AddModObj(ObjBase objBase, int index)
        {
            int EncodeMax = 0; //确定新模块的不重名名称
            List<string> nameList = mObjBase.Select(c => c.ModInfo.Name).ToList();
            while (true)
            {
                if (!nameList.Contains(objBase.ModInfo.Plugin + EncodeMax))
                {
                    break;//没有重名就跳出循环
                }
                EncodeMax++;
            }
            objBase.ModInfo.Encode = EncodeMax;
            objBase.ModInfo.Name = objBase.ModInfo.Plugin + objBase.ModInfo.Encode;
            objBase.ModInfo.Index = mProjInfo.Index;
            objBase.mECom = mECom;
            objBase.mSysVar = mSysVar;
            objBase.mSloVar = mSloVar;
            objBase.mCalVar = mCalVar;
            objBase.mCamera = mCamera;
            objBase.Name = mProjInfo.Name;
            objBase.ModInfo.State = ModState.None;
            if (index > -1)
            {
                mObjBase.Insert(index, objBase);
            }
            GerInfoList();
        }

        /// <summary>添加一个任务</summary>
        /// <param name="ChangMod">要追加的模块目标位置模块名称</param>
        /// <param name="mModParam">模块信息</param>
        /// <param name="isNext">是否在后方追加</param>
        /// TODO:添加一个模块
        public void AddModObj(string ChangName, ModInfo mModParam, bool isNext)
        {
            try
            {
                //Sol.mProjSave = false;//解决方案未保存
                //PluginsInfo mPluginsInfo = PluginToolService.mPluginDic[mModParam.Plugin];
                //ObjBase NewObjBase = (ObjBase)mPluginsInfo.ModObjType.Assembly.CreateInstance(mPluginsInfo.ModObjType.FullName);
                //NewObjBase.ModInfo.Plugin = mPluginsInfo.Name;
                //int index = mObjBase.FindIndex(x => x.ModInfo.Name == ChangName) + 1;
                //if (index >= 0 & isNext)
                //{
                //    AddModObj(NewObjBase, index);
                //}
                //else if (index >= 0 & !isNext)
                //{
                //    AddModObj(NewObjBase, index - 1);
                //}
                //else
                //{
                //    AddModObj(NewObjBase, -1);
                //}
                //if (mModParam.Plugin == "文件夹")
                //{
                //    mPluginsInfo = PluginToolService.mPluginDic["文件夹结束"];
                //    NewObjBase = (ObjBase)mPluginsInfo.ModObjType.Assembly.CreateInstance(mPluginsInfo.ModObjType.FullName);
                //    NewObjBase.ModInfo.Plugin = mPluginsInfo.Name;
                //    AddModObj(NewObjBase, index + 1);
                //}
                //else if (mModParam.Plugin == "循环开始")
                //{
                //    mPluginsInfo = PluginToolService.mPluginDic["循环结束"];
                //    NewObjBase = (ObjBase)mPluginsInfo.ModObjType.Assembly.CreateInstance(mPluginsInfo.ModObjType.FullName);
                //    NewObjBase.ModInfo.Plugin = mPluginsInfo.Name;
                //    AddModObj(NewObjBase, index + 1);
                //}
                //else if (mModParam.Plugin == "如果")
                //{
                //    mPluginsInfo = PluginToolService.mPluginDic["结束"];
                //    NewObjBase = (ObjBase)mPluginsInfo.ModObjType.Assembly.CreateInstance(mPluginsInfo.ModObjType.FullName);
                //    NewObjBase.ModInfo.Plugin = mPluginsInfo.Name;
                //    NewObjBase.ModInfo.Remarks = "双击添加条件";
                //    AddModObj(NewObjBase, index + 1);
                //}
                //if (mModParam.Plugin == "标签")
                //{
                //}
                ChangeEvent();
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
            GerInfoList();
        }

        /// <summary>TODO:删除模块-模块名称</summary>
        public void RemovMod(string RemovName)
        {
            try
            {
                bool falg = true;
                Sol.mProjSave = false;//解决方案未保存
                ObjBase ObjBaseInfo = null;
                if (RemovName.Contains("文件夹"))
                {
                    ObjBaseInfo = mObjBase.First(x => x.ModInfo.Name == RemovName);
                    mObjBase.Remove(ObjBaseInfo);
                    ObjBaseInfo = mObjBase.First(x => x.ModInfo.Name == "文件夹结束" + RemovName.Replace("文件夹", ""));
                    mObjBase.Remove(ObjBaseInfo);
                }
                else if (RemovName.Contains("循环开始"))
                {
                    ObjBaseInfo = mObjBase.First(x => x.ModInfo.Name == RemovName);
                    mObjBase.Remove(ObjBaseInfo);
                    ObjBaseInfo = mObjBase.First(x => x.ModInfo.Name == "循环结束" + RemovName.Replace("循环开始", ""));
                    mObjBase.Remove(ObjBaseInfo);
                }
                else if (RemovName.Contains("如果"))
                {
                    int index = mObjBase.FindIndex(x => x.ModInfo.Name == RemovName);
                    ObjBaseInfo = mObjBase.First(x => x.ModInfo.Name == RemovName);
                    mObjBase.Remove(ObjBaseInfo);
                    while (falg)
                    {
                        ObjBase RemovObj = mObjBase[index];
                        if (RemovObj.ModInfo.Name.Contains("否则"))
                        {
                            mObjBase.Remove(RemovObj);
                        }
                        else if (RemovObj.ModInfo.Name.Contains("结束"))
                        {
                            mObjBase.Remove(RemovObj);
                            falg = false;
                        }
                        else
                        {
                            RemovObj = mObjBase[++index];
                        }
                    }
                }
                else
                {
                    mObjBase.Remove(mObjBase.First(x => x.ModInfo.Name == RemovName));
                }
                GerInfoList();
                ChangeEvent();
            }
            catch (Exception ex) { Log.Debug(ex.Message); }
        }

        /// <summary>TODO:更新模块</summary>
        public void ChangeMod(ModInfo mModParam)
        {
            Sol.mProjSave = false;//解决方案未保存
            ObjBase data = mObjBase.FirstOrDefault(c => (c.ModInfo.Name == mModParam.Name));
            data.Enable = mModParam.Enable;
            GerInfoList();
            ChangeEvent();
        }
        /// <summary>TODO:修改位置</summary>
        /// <param name="ChangName">模块名称</param>
        /// <param name="GoalName">目标位置的模块名称</param>
        /// <param name="isNext">是否在目标下方追加</param>
        public void ChangePos(string ChangName, string GoalName, bool isNext, ModFlowItem itemA, ModFlowItem itemB)
        {
            try
            {
                Sol.mProjSave = false;//解决方案未保存
                ObjBase ModInfo = mObjBase.First(x => x.ModInfo.Name == ChangName);
                if (ModInfo.ModInfo.Plugin == "条件分支" && !ModInfo.ModInfo.Name.StartsWith("如果"))// 
                {
                    Log.Warn("条件分支工具若要改动位置,请拖动对应的 <如果> 模块");
                    return;
                }
                if (ModInfo.ModInfo.Name.StartsWith("结束"))
                {
                    Log.Warn("条件结束不允许改变位置,若要改动位置,请拖动对应的 <否则>模块");
                    return;
                }
                if (ModInfo.ModInfo.Name.StartsWith("文件夹结束"))
                {
                    Log.Warn("文件夹结束不允许改变位置,若要改动位置,请拖动对应的 <文件夹>模块");
                    return;
                }
                int NewPos = mObjBase.FindIndex(x => x.ModInfo.Name == GoalName);
                int OldPos = mObjBase.FindIndex(x => x.ModInfo.Name == ChangName);
                int OrgPos = mObjBase.FindIndex(x => x.ModInfo.Name == "文件夹结束" + ChangName.Replace("文件夹", ""));
                if (ChangName.Contains("文件夹"))
                {
                    if (NewPos >= OrgPos)
                    {
                        Log.Warn("文件夹只能在文件夹结束前面,请拖动至对应位置 <文件夹>模块");
                        return;
                    }
                }
                NewPos = NewPos < OldPos ? ++NewPos : NewPos;
                mObjBase.Remove(ModInfo); //ToDo:模块位置变更-原位删除
                mObjBase.Insert(NewPos, ModInfo);//ToDo:模块位置变更-新位插入
                GerInfoList();
                ChangeEvent();
            }
            catch { }
        }
        /// <summary>TODO:获取所有模块名称</summary>
        public List<string> GetObjBaseList()
        {
            return mObjBase.Select(c => c.ModInfo.Name).ToList();
        }
        /// <summary>TODO:根据名称获取模块</summary>
        public ObjBase GetObjBase(string Name)
        {
            return mObjBase.FirstOrDefault(c => c.ModInfo.Name == Name);
        }
        /// <summary>TODO:根据索引获取模块</summary>
        public ObjBase GetObjBase(int index)
        {
            return mObjBase.FirstOrDefault(c => c.ModInfo.Encode == index);
        }
        /// <summary>TODO:还原模块数据</summary>
        public void RecoverObjBase(ObjBase backModObjBase)
        {
            if (mObjBase.Count > 0)
            {
                int index = mObjBase.FindIndex(c => c.ModInfo.Encode == backModObjBase.ModInfo.Encode);
                mObjBase[index] = backModObjBase;
            }
        }
        /// <summary>TODO:模块修改事件 </summary>
        [field: NonSerialized]
        public event EventHandler<EventArgs> mChangeEvent;
        /// <summary>TODO:触发模块事件 </summary>
        public void ChangeEvent()
        {
            mChangeEvent?.Invoke(this, new EventArgs());
        }
        /// <summary>获取模块信息列表</summary>
        public void GerInfoList()
        {
            if (mModInfo == null)
            {
                mModInfo = new List<ModInfo>();
            }
            mModInfo.Clear();
            foreach (ObjBase mbase in mObjBase)
            {
                mModInfo.Add(mbase.ModInfo);
            }
        }
        ~Proj()
        {
            mAutoResetEvent.Dispose();
            mThread.Abort();
        }
    }
}
