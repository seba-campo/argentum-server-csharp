using Argentum.Network.Connections;
using Argentum.Network.Packets;
using Argentum.Network.Serialization;

namespace Argentum.Network.Routing;

/// <summary>
/// Enrutador responsable de leer la cabecera (packetId) e invocar el controlador correspondiente.
/// </summary>
public sealed class PacketDispatcher
{
    private readonly Dictionary<ClientPacketId, Func<IClientSession, NetReader, Task>> _handlers = new();

    /// <summary>
    /// Evento disparado cuando se recibe un paquete para el cual no hay controlador registrado.
    /// </summary>
    public event Action<IClientSession, ClientPacketId>? UnhandledPacket;

    /// <summary>
    /// Registra un controlador tipado para un ID de paquete entrante.
    /// </summary>
    public void RegisterHandler<TPacket>(
        ClientPacketId packetId,
        Func<NetReader, TPacket> deserializer,
        Func<IClientSession, TPacket, Task> handler)
        where TPacket : IIncomingPacket
    {
        _handlers[packetId] = async (session, reader) =>
        {
            TPacket packet = deserializer(reader);
            await handler(session, packet);
        };
    }

    /// <summary>
    /// Despacha un búfer de bytes entrante leyendo los 2 bytes de cabecera (ClientPacketId).
    /// </summary>
    public async Task DispatchAsync(IClientSession session, byte[] rawBytes)
    {
        if (rawBytes.Length < sizeof(short))
        {
            // Búfer incompleto: ni siquiera tiene los 2 bytes del identificador
            return;
        }

        using var reader = new NetReader(rawBytes);
        short packetIdRaw = reader.ReadInt16();
        var packetId = (ClientPacketId)packetIdRaw;

        if (_handlers.TryGetValue(packetId, out var handler))
        {
            await handler(session, reader);
        }
        else
        {
            UnhandledPacket?.Invoke(session, packetId);
        }
    }
}
