using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Argentum.Network.Connections;
using Argentum.Network.Routing;
using Microsoft.Extensions.Logging;

namespace Argentum.Server.Networking;

/// <summary>
/// Servidor TCP que escucha en el puerto del juego, acepta conexiones y las deriva al enrutador.
/// </summary>
public sealed class TcpGameServer : IDisposable
{
    private readonly int _port;
    private readonly PacketDispatcher _dispatcher;
    private readonly ILogger<TcpGameServer> _logger;
    private readonly ConcurrentDictionary<int, ClientSession> _sessions = new();
    private TcpListener? _listener;
    private CancellationTokenSource? _cts;
    private int _nextConnectionId;

    public int Port => _port;
    public int ActiveConnectionsCount => _sessions.Count;

    public TcpGameServer(int port, PacketDispatcher dispatcher, ILogger<TcpGameServer> logger)
    {
        _port = port;
        _dispatcher = dispatcher;
        _logger = logger;
    }

    /// <summary>
    /// Inicia el listener TCP y el bucle de aceptación de clientes.
    /// </summary>
    public void Start()
    {
        _cts = new CancellationTokenSource();
        _listener = new TcpListener(IPAddress.Any, _port);
        _listener.Start();

        _logger.LogInformation("Servidor TCP iniciado en el puerto {Port}. Esperando conexiones...", _port);

        _ = Task.Run(() => AcceptConnectionsAsync(_cts.Token));
    }

    private async Task AcceptConnectionsAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested && _listener != null)
        {
            try
            {
                TcpClient tcpClient = await _listener.AcceptTcpClientAsync(ct);

                // Desactivar el algoritmo de Nagle para enviar paquetes inmediatamente sin latencia extra
                tcpClient.NoDelay = true;

                int connectionId = Interlocked.Increment(ref _nextConnectionId);
                var session = new ClientSession(connectionId, tcpClient);

                if (_sessions.TryAdd(connectionId, session))
                {
                    _logger.LogInformation("Cliente conectado [ID: {ConnectionId}] desde {RemoteEndPoint}. Conexiones activas: {Total}",
                        connectionId, session.RemoteEndPoint, _sessions.Count);

                    session.Disconnected += OnClientDisconnected;

                    // Iniciar la lectura de datos de este cliente en segundo plano
                    _ = Task.Run(() => session.StartReadingAsync(_dispatcher.DispatchAsync, ct), ct);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al aceptar una nueva conexión TCP");
            }
        }
    }

    private void OnClientDisconnected(ClientSession session)
    {
        if (_sessions.TryRemove(session.Id, out _))
        {
            _logger.LogInformation("Cliente desconectado [ID: {ConnectionId}]. Conexiones activas: {Total}",
                session.Id, _sessions.Count);
        }
    }

    /// <summary>
    /// Detiene el servidor y desconecta a todos los clientes.
    /// </summary>
    public void Stop()
    {
        _cts?.Cancel();
        _listener?.Stop();

        foreach (var session in _sessions.Values)
        {
            session.Disconnect();
        }

        _sessions.Clear();
        _logger.LogInformation("Servidor TCP detenido.");
    }

    public void Dispose()
    {
        Stop();
        _cts?.Dispose();
    }
}
