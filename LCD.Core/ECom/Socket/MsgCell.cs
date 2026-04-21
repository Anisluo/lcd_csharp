using System;

namespace DMSkin.Socket
{
    /// <summary>
    /// 消息单元-支持序列化
    /// </summary>
    [Serializable]
    public class MsgCell : IDataCell
    {
        public MsgCell()
        {
        }
        public MsgCell(int messageId, object data)
        {
            this._MessageId = messageId;
            this._Data = data;
        }

        public int MessageId
        {
            get
            {
                return this._MessageId;
            }
            set
            {
                this._MessageId = value;
            }
        }
        public object Data
        {
            get
            {
                return this._Data;
            }
            set
            {
                this._Data = value;
            }
        }
        /// <summary>
        /// string类型的数据转换-姑且认为它是高效的
        /// </summary>
        /// <returns></returns>
        public byte[] ToBuffer()
        {
            byte[] array = SerHelper.Serialize(this._Data);
            byte[] bytes = BitConverter.GetBytes(this.MessageId);
            byte[] array2 = new byte[array.Length + bytes.Length];
            Buffer.BlockCopy(bytes, 0, array2, 0, bytes.Length);
            Buffer.BlockCopy(array, 0, array2, bytes.Length, array.Length);
            return array2;
        }
        /// <summary>
        /// string类型的数据转换-姑且认为它是高效的
        /// </summary>
        /// <param name="buffer"></param>
        public void FromBuffer(byte[] buffer)
        {
            this._MessageId = BitConverter.ToInt32(buffer, 0);
            this._Data = SerHelper.Deserialize(buffer, 4);
        }

        private int _MessageId;
        private object _Data;
    }
}
