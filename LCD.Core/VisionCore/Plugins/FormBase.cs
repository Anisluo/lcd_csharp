using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionCore
{
    /// <summary>
    /// UI基类
    /// </summary>
    public partial class FormBase
    {
        /// <summary>界面-对应模块 </summary>
        public ObjBase ObjBase { get; set; }

        /// <summary>备份-取消还原</summary>
        private ObjBase ObjBaseBack { get; set; }

        public FormBase(ObjBase mObj)
        {
            ObjBase = mObj;
        }

        public void Init()
        {
            ObjToForm();
            FormToObj();
        }
        /// <summary>模块数据加载至界面-先模块后界面</summary>
        public virtual void ObjToForm() { }
        /// <summary>界面数据加载至模块-先模块后界面</summary>
        public virtual void FormToObj() { }

        private void btn_Run_Click(object sender, EventArgs e) { }
        private void btn_Save_Click(object sender, EventArgs e) { }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            ObjBase = ObjBaseBack;
        }
    }
}
