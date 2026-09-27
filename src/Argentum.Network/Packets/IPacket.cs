using Argentum.Network.Serialization;

namespace Argentum.Network.Packets;

/// <summary>
/// Contrato base para cualquier paquete del protocolo de Argentum Online.
/// </summary>
public interface IPacket
{
    short PacketId { get; }
}

/// <summary>
/// Paquete enviado desde el servidor hacia el cliente.
/// </summary>
public interface IOutgoingPacket : IPacket
{
    ServerPacketId Id { get; }
    short IPacket.PacketId => (short)Id;

    void Serialize(NetWriter writer);
}

/// <summary>
/// Paquete recibido desde el cliente hacia el servidor.
/// </summary>
public interface IIncomingPacket : IPacket
{
    ClientPacketId Id { get; }
    short IPacket.PacketId => (short)Id;
}
