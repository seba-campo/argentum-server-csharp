using Argentum.Network.Serialization;

namespace Argentum.Network.Packets.Outgoing;

/// <summary>
/// Paquete de confirmación de inicio de sesión exitoso enviado al cliente (eLogged).
/// </summary>
public sealed record LoggedPacket(bool IsNewUser = false) : IOutgoingPacket
{
    public ServerPacketId Id => ServerPacketId.elogged;

    public void Serialize(NetWriter writer)
    {
        writer.WriteInt16((short)Id);
        writer.WriteBoolean(IsNewUser);
    }
}
