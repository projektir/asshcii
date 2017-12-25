using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SSH.Packets;

namespace SSH
{
    public class ByteWriter : IDisposable
    {
        private MemoryStream stream = new MemoryStream();

        public void WritePacketType(PacketType packetType)
        {
            WriteByte((byte)packetType);
        }

        public void WriteBytes(byte[] value)
        {
            WriteUInt32((uint)value.Length);
            WriteRawBytes(value);
        }

        public void WriteString(string value)
        {
            WriteString(value, Encoding.ASCII);
        }

        public void WriteString(string value, Encoding encoding)
        {
            WriteBytes(encoding.GetBytes(value));
        }

        public void WriteStringList(IEnumerable<String> list)
        {
            WriteString(string.Join(",", list));
        }

        public void WriteUInt32(uint value)
        {
            byte[] data = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                data = data.Reverse().ToArray();
            WriteRawBytes(data);
        }

        public void WriteMPInt(byte[] value)
        {
            if ((value.Length == 1) && (value[0] == 0))
            {
                WriteUInt32(0);
                return;
            }

            uint length = (uint)value.Length;
            if (((value[0] & 0x80) != 0))
            {
                WriteUInt32((uint)(length + 1));
                WriteByte(0x00);
            }
            else
            {
                WriteUInt32((uint)length);
            }

            WriteRawBytes(value);
        }

        public void WriteRawBytes(byte[] value)
        {
            if (disposedValue)
                throw new ObjectDisposedException("ByteWriter");
            stream.Write(value, 0, value.Count());
        }

        public void WriteByte(byte value)
        {
            if (disposedValue)
                throw new ObjectDisposedException("ByteWriter");
            stream.WriteByte(value);
        }

        public byte[] ToByteArray()
        {
            if (disposedValue)
                throw new ObjectDisposedException("ByteWriter");
            return stream.ToArray();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    stream.Dispose();
                    stream = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}