namespace DMSkin.Socket
{
    /// <summary>
    /// 数据单元--提供Tcp收发的数据转换实现
    /// </summary>
    public interface IDataCell
    {
        byte[] ToBuffer();
        void FromBuffer(byte[] buffer);
    }
}
