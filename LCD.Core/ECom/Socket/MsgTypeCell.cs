using System;

namespace DMSkin.Socket
{
    [Serializable]
    public class MsgTypeCell
    {
        public MsgTypeCell(MsgType msgType, byte[] buffer)
        {
            this._ImageSuffix = "";
            this.Msgtype = msgType;
            this.BufferBytes = buffer;
        }

        public MsgType Msgtype
        {
            get
            {
                return this._Msgtype;
            }
            set
            {
                this._Msgtype = value;
            }
        }

        /// <summary>
        /// 图片后缀
        /// </summary>
        public string ImageSuffix
        {
            get
            {
                return this._ImageSuffix;
            }
            set
            {
                this._ImageSuffix = value;
            }
        }

        /// <summary>
        /// 图片数据
        /// </summary>
        public byte[] BufferBytes
        {
            get
            {
                return this._BufferBytes;
            }
            set
            {
                this._BufferBytes = value;
            }
        }

        private MsgType _Msgtype;
        private string _ImageSuffix;
        private byte[] _BufferBytes;
    }
}
