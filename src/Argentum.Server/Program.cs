using Argentum.Core.Maps;
using Argentum.Core.Spatial;
using Argentum.Core.World;
using Argentum.Network.Packets;
using Argentum.Network.Packets.Incoming;
using Argentum.Network.Packets.Outgoing;
using Argentum.Network.Routing;
using Argentum.Server.Networking;
using Microsoft.Extensions.Logging;

// 1. Configurar Logging por consola
using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddSimpleConsole(options =>
    {
        options.IncludeScopes = false;
        options.SingleLine = true;
        options.TimestampFormat = "[HH:mm:ss] ";
    });
    builder.SetMinimumLevel(LogLevel.Information);
});

ILogger logger = loggerFactory.CreateLogger("ArgentumServer");

Console.Title = "Argentum Online Server - .NET 9";
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine(@"
   ___                        _                     ____         _ _            
  / _ \                      | |                   / __ \       | (_)           
 / /_\ \_ __ __ _  ___ _ __  | |_ _   _ _ __ ___  | |  | |_ __  | |_ _ __   ___ 
 |  _  | '__/ _` |/ _ \ '_ \ | __| | | | '_ ` _ \ | |  | | '_ \ | | | '_ \ / _ \
 | | | | | | (_| |  __/ | | || |_| |_| | | | | | || |__| | | | || | | | | |  __/
 \_| |_/_|  \__, |\___|_| |_| \__|\__,_|_| |_| |_| \____/|_| |_||_|_|_| |_|\___|
             __/ |                                                              
            |___/                     C# / .NET 9 Game Server
");
Console.ResetColor();

const int serverPort = 7666;

// 2. Inicializar el Administrador del Mundo (WorldManager)
var worldManager = new WorldManager();

// Cargamos un mapa inicial (Mapa 1: Ullathorpe)
var ullathorpe = new Map(1, "Ullathorpe");
worldManager.AddMap(ullathorpe);
logger.LogInformation("Mundo inicializado con {MapCount} mapa(s).", worldManager.MapCount);

// 3. Configurar el Enrutador de Paquetes (Dispatcher)
var dispatcher = new PacketDispatcher();

// Registrar qué hacer cuando llega un paquete no reconocido
dispatcher.UnhandledPacket += (session, packetId) =>
{
    logger.LogWarning("[ID: {ConnectionId}] Paquete no implementado: {PacketId} ({(int)packetId})",
        session.Id, packetId, (short)packetId);
};

// 4. Instanciar el Servidor TCP
TcpGameServer? server = null;

// Registrar el controlador para eLoginExistingChar
dispatcher.RegisterHandler(
    ClientPacketId.eLoginExistingChar,
    deserializer: LoginExistingCharPacket.Deserialize,
    handler: async (session, packet) =>
    {
        logger.LogInformation("[ID: {ConnectionId}] -> Recibido Login: Usuario='{Username}', CharId={CharId}",
            session.Id, packet.Username, packet.CharacterId);

        // Ubicar al jugador en el mapa (por defecto mapa 1, posición 50, 50)
        var spawnPos = new Position(1, 50, 50);
        bool spawned = worldManager.SpawnPlayer(session.Id, spawnPos);

        if (spawned)
        {
            logger.LogInformation("[ID: {ConnectionId}] Jugador '{Username}' posicionado en Mapa {Map} ({X}, {Y}).",
                session.Id, packet.Username, spawnPos.Map, spawnPos.X, spawnPos.Y);
        }
        else
        {
            logger.LogWarning("[ID: {ConnectionId}] No se pudo ubicar al jugador en la posición inicial {Pos}.",
                session.Id, spawnPos);
        }

        // Responder con la confirmación de Login (eLogged)
        logger.LogInformation("[ID: {ConnectionId}] <- Enviando confirmación de Login (eLogged)...", session.Id);
        await session.SendAsync(new LoggedPacket(IsNewUser: false));

        // Enviar mensaje de bienvenida a la consola del juego (eConsoleMsg)
        logger.LogInformation("[ID: {ConnectionId}] <- Enviando mensaje de consola...", session.Id);
        await session.SendAsync(new ConsoleMessagePacket(
            Message: $"¡Hola {packet.Username}! Te has conectado exitosamente al servidor C# .NET 9.",
            FontIndex: 1,
            Channel: 0
        ));
    }
);

// Registrar el controlador para eWalk (Caminata)
dispatcher.RegisterHandler(
    ClientPacketId.eWalk,
    deserializer: WalkPacket.Deserialize,
    handler: async (session, packet) =>
    {
        var newPos = worldManager.MovePlayer(session.Id, packet.Heading);

        if (newPos.HasValue)
        {
            logger.LogInformation("[ID: {ConnectionId}] Avanzó hacia {Heading} -> Nueva posición: Mapa {Map} ({X}, {Y})",
                session.Id, packet.Heading, newPos.Value.Map, newPos.Value.X, newPos.Value.Y);

            // Notificar a los demás jugadores cercanos en el área para que vean la animación de caminar
            var nearbyPlayerIds = worldManager.GetNearbyPlayers(newPos.Value, excludePlayerId: session.Id);
            if (nearbyPlayerIds.Count > 0 && server != null)
            {
                var movePacket = new CharacterMovePacket(
                    CharacterIndex: (short)session.Id,
                    X: newPos.Value.X,
                    Y: newPos.Value.Y
                );

                foreach (int nearbyId in nearbyPlayerIds)
                {
                    if (server.TryGetSession(nearbyId, out var nearbySession) && nearbySession != null)
                    {
                        await nearbySession.SendAsync(movePacket);
                    }
                }
            }
        }
        else
        {
            var currentPos = worldManager.GetPlayerPosition(session.Id);
            logger.LogWarning("[ID: {ConnectionId}] Intento de paso inválido hacia {Heading}. Se mantiene en {Pos}",
                session.Id, packet.Heading, currentPos);

            // Sincronización forzada en caso de paso bloqueado / desfasaje
            if (currentPos.HasValue)
            {
                await session.SendAsync(new PosUpdatePacket(currentPos.Value.X, currentPos.Value.Y));
            }
        }
    }
);

// Registrar el controlador para eRequestPositionUpdate (Solicitud de posición por lag/desync)
dispatcher.RegisterHandler(
    ClientPacketId.eRequestPositionUpdate,
    deserializer: RequestPositionUpdatePacket.Deserialize,
    handler: async (session, packet) =>
    {
        var currentPos = worldManager.GetPlayerPosition(session.Id);
        if (currentPos.HasValue)
        {
            logger.LogInformation("[ID: {ConnectionId}] Cliente solicitó actualización de posición -> Enviando {X}, {Y}",
                session.Id, currentPos.Value.X, currentPos.Value.Y);

            await session.SendAsync(new PosUpdatePacket(currentPos.Value.X, currentPos.Value.Y));
        }
    }
);

// 5. Iniciar el Servidor TCP
server = new TcpGameServer(
    serverPort,
    dispatcher,
    loggerFactory.CreateLogger<TcpGameServer>()
);

// Limpiar del mundo al jugador cuando se desconecta
server.ClientDisconnected += session =>
{
    worldManager.DespawnPlayer(session.Id);
    logger.LogInformation("[ID: {ConnectionId}] Jugador removido del mundo al desconectarse.", session.Id);
};

server.Start();

// 6. Mantener la aplicación viva hasta que el usuario presione Ctrl+C
var cts = new CancellationTokenSource();
Console.CancelKeyPress += (sender, eventArgs) =>
{
    eventArgs.Cancel = true;
    logger.LogInformation("Deteniendo el servidor...");
    cts.Cancel();
};

logger.LogInformation("Servidor listo. Presioná Ctrl+C para salir.");

try
{
    await Task.Delay(Timeout.Infinite, cts.Token);
}
catch (OperationCanceledException)
{
    // Cierre ordenado
}

server.Stop();
logger.LogInformation("Servidor finalizado limpiamente.");
