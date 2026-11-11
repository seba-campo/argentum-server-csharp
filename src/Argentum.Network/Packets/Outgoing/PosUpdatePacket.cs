using Argentum.Network.Serialization;

namespace Argentum.Network.Packets.Outgoing;

/// <summary>
/// Paquete enviado por el servidor para forzar o actualizar la posición del cliente (ServerPacketId.ePosUpdate).
/// Formato de red VB6: PacketId (Int16) + X (Byte/Int8) + Y (Byte/Int8).
/// </summary>
public sealed record PosUpdatePacket(byte X, byte Y) : IOutgoingPacket
{
    public ServerPacketId Id => ServerPacketId.ePosUpdate;

    public void Serialize(NetWriter writer)
    {
        writer.WriteInt16((short)Id);
        writer.WriteByte(X);
        writer.WriteByte(Y);
    }
}
