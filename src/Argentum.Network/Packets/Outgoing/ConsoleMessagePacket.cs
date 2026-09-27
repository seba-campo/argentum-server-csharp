using Argentum.Network.Serialization;

namespace Argentum.Network.Packets.Outgoing;

/// <summary>
/// Paquete para imprimir un mensaje de texto en la consola del cliente (eConsoleMsg).
/// </summary>
public sealed record ConsoleMessagePacket(
    string Message,
    byte FontIndex,
    byte Channel
) : IOutgoingPacket
{
    public ServerPacketId Id => ServerPacketId.eConsoleMsg;

    public void Serialize(NetWriter writer)
    {
        writer.WriteInt16((short)Id);
        writer.WriteString(Message);
        writer.WriteByte(FontIndex);
        writer.WriteByte(Channel);
    }
}
