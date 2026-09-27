using Argentum.Network.Packets;

namespace Argentum.Network.Connections;

/// <summary>
/// Representa la sesión de red activa de un cliente conectado.
/// </summary>
public interface IClientSession
{
    /// <summary>
    /// Identificador único numérico de la conexión (ConnectionId).
    /// </summary>
    int Id { get; }

    /// <summary>
    /// Dirección IP y puerto remoto del cliente.
    /// </summary>
    string RemoteEndPoint { get; }

    /// <summary>
    /// Indica si el socket TCP sigue abierto.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Serializa y envía un paquete hacia el cliente de forma asíncrona.
    /// </summary>
    Task SendAsync(IOutgoingPacket packet);

    /// <summary>
    /// Cierra la conexión de red con el cliente.
    /// </summary>
    void Disconnect();
}
