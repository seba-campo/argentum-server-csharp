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

// 2. Configurar el Enrutador de Paquetes (Dispatcher)
var dispatcher = new PacketDispatcher();

// Registrar qué hacer cuando llega un paquete no reconocido
dispatcher.UnhandledPacket += (session, packetId) =>
{
    logger.LogWarning("[ID: {ConnectionId}] Paquete no implementado: {PacketId} ({(int)packetId})",
        session.Id, packetId, (short)packetId);
};

// Registrar el controlador para eLoginExistingChar
dispatcher.RegisterHandler(
    ClientPacketId.eLoginExistingChar,
    deserializer: LoginExistingCharPacket.Deserialize,
    handler: async (session, packet) =>
    {
        logger.LogInformation("[ID: {ConnectionId}] -> Recibido Login: Usuario='{Username}', CharId={CharId}",
            session.Id, packet.Username, packet.CharacterId);

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

// 3. Iniciar el Servidor TCP
using var server = new TcpGameServer(
    serverPort,
    dispatcher,
    loggerFactory.CreateLogger<TcpGameServer>()
);

server.Start();

// 4. Mantener la aplicación viva hasta que el usuario presione Ctrl+C
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
