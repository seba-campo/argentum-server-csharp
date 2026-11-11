using Argentum.Network.Serialization;

namespace Argentum.Network.Packets.Outgoing;

/// <summary>
/// Paquete enviado por el servidor para notificar a los clientes en el área que un personaje se ha movido (ServerPacketId.eCharacterMove).
/// Formato de red VB6: PacketId (Int16) + CharacterIndex (Int16) + X (Byte/Int8) + Y (Byte/Int8).
/// </summary>
public sealed record CharacterMovePacket(
    short CharacterIndex,
    byte X,
    byte Y
) : IOutgoingPacket
{
    public ServerPacketId Id => ServerPacketId.eCharacterMove;

    public void Serialize(NetWriter writer)
    {
        writer.WriteInt16((short)Id);
        writer.WriteInt16(CharacterIndex);
        writer.WriteByte(X);
        writer.WriteByte(Y);
    }
}
