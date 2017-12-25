using System;

namespace SSH.Packets
{
    public class KexDHReply : Packet
    {
        public override PacketType PacketType => PacketType.SSH_MSG_KEXDH_REPLY;

        public byte[] ServerHostKey { get; set; }
        public byte[] ServerValue { get; set; }
        public byte[] Signature { get; set; }

        protected override void InternalGetBytes(ByteWriter writer)
        {
            // string server public host key and certificates(K_S)
            // mpint f
            // string signature of H
            writer.WriteBytes(ServerHostKey);
            writer.WriteMPInt(ServerValue);
            writer.WriteBytes(Signature);
        }

        protected override void Load(ByteReader reader)
        {
            // Client never sends this!
            throw new InvalidOperationException("SSH Client should never send a SSH_MSG_KEXDH_REPLY message");
        }
    }
}