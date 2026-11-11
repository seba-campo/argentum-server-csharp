using Argentum.Network.Serialization;

namespace Argentum.Network.Packets.Incoming;

/// <summary>
/// Paquete enviado por el cliente para solicitar la actualización forzada de su posición en caso de desincronización (ClientPacketId.eRequestPositionUpdate).
/// En el protocolo clásico de VB6 no contiene payload adicional (tamaño 0 después del PacketId).
/// </summary>
public sealed record RequestPositionUpdatePacket : IIncomingPacket
{
    public ClientPacketId Id => ClientPacketId.eRequestPositionUpdate;

    public static RequestPositionUpdatePacket Deserialize(NetReader reader)
    {
        // No tiene campos adicionales tras el Header (Int16)
        return new RequestPositionUpdatePacket();
    }
}
