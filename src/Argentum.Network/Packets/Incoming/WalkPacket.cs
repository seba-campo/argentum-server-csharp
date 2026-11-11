using Argentum.Core.Enums;
using Argentum.Network.Serialization;

namespace Argentum.Network.Packets.Incoming;

/// <summary>
/// Paquete enviado por el cliente para dar un paso o caminar en una dirección (ClientPacketId.eWalk).
/// Protocolo VB6: Heading (Int8) + PacketCount (Int32).
/// </summary>
public sealed record WalkPacket(
    Heading Heading,
    int PacketCount
) : IIncomingPacket
{
    public ClientPacketId Id => ClientPacketId.eWalk;

    public static WalkPacket Deserialize(NetReader reader)
    {
        byte headingByte = reader.ReadByte();
        int packetCount = reader.ReadInt32();

        var heading = (Heading)headingByte;
        return new WalkPacket(heading, packetCount);
    }
}