using System.Net.Sockets;
using Argentum.Network.Packets;
using Argentum.Network.Serialization;

namespace Argentum.Network.Connections;

/// <summary>
/// Implementación de sesión de cliente basada en TcpClient.
/// </summary>
public sealed class ClientSession : IClientSession, IDisposable
{
    private readonly TcpClient _tcpClient;
    private readonly NetworkStream _stream;
    private readonly SemaphoreSlim _sendLock = new(1, 1);
    private int _disconnected;

    public int Id { get; }
    public string RemoteEndPoint { get; }
    public bool IsConnected => _disconnected == 0 && _tcpClient.Connected;

    public event Action<ClientSession>? Disconnected;

    public ClientSession(int id, TcpClient tcpClient)
    {
        Id = id;
        _tcpClient = tcpClient;
        _stream = tcpClient.GetStream();
        RemoteEndPoint = tcpClient.Client.RemoteEndPoint?.ToString() ?? "Unknown";
    }

    /// <summary>
    /// Ciclo de lectura asíncrona continuo de datos provenientes del cliente.
    /// </summary>
    public async Task StartReadingAsync(Func<IClientSession, byte[], Task> onDataReceived, CancellationToken ct)
    {
        byte[] buffer = new byte[4096];

        try
        {
            while (!ct.IsCancellationRequested && IsConnected)
            {
                int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, ct);
                if (bytesRead == 0)
                {
                    // Desconexión limpia del cliente (EOF)
                    break;
                }

                // Extraemos exactamente el segmento de bytes recibidos
                byte[] receivedData = new byte[bytesRead];
                Buffer.BlockCopy(buffer, 0, receivedData, 0, bytesRead);

                await onDataReceived(this, receivedData);
            }
        }
        catch (Exception) when (!IsConnected || ct.IsCancellationRequested)
        {
            // Cierre normal de socket o cancelación
        }
        finally
        {
            Disconnect();
        }
    }

    /// <summary>
    /// Serializa y despacha un paquete binario al cliente.
    /// </summary>
    public async Task SendAsync(IOutgoingPacket packet)
    {
        if (!IsConnected)
        {
            return;
        }

        byte[] payload;
        using (var writer = new NetWriter())
        {
            packet.Serialize(writer);
            payload = writer.ToByteArray();
        }

        await _sendLock.WaitAsync();
        try
        {
            if (IsConnected)
            {
                await _stream.WriteAsync(payload);
                await _stream.FlushAsync();
            }
        }
        finally
        {
            _sendLock.Release();
        }
    }

    public void Disconnect()
    {
        if (Interlocked.Exchange(ref _disconnected, 1) == 0)
        {
            try
            {
                _stream.Dispose();
                _tcpClient.Close();
            }
            catch
            {
                // Ignorar excepciones al cerrar sockets ya caídos
            }

            Disconnected?.Invoke(this);
        }
    }

    public void Dispose()
    {
        Disconnect();
        _sendLock.Dispose();
    }
}
