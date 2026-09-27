using System.Net.Sockets;
using Argentum.Network.Packets;
using Argentum.Network.Packets.Incoming;
using Argentum.Network.Packets.Outgoing;
using Argentum.Network.Routing;
using Argentum.Network.Serialization;
using Argentum.Server.Networking;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Argentum.Tests.Network;

public class TcpServerIntegrationTests
{
    [Fact]
    public async Task EndToEnd_ClientConnectsAndSendsLogin_ReceivesLoggedAndConsoleMessage()
    {
        const int testPort = 7699;
        var dispatcher = new PacketDispatcher();

        // 1. Configurar el handler en el Dispatcher
        dispatcher.RegisterHandler(
            ClientPacketId.eLoginExistingChar,
            deserializer: LoginExistingCharPacket.Deserialize,
            handler: async (session, packet) =>
            {
                // Responder primero con Logged
                await session.SendAsync(new LoggedPacket(IsNewUser: false));

                // Responder luego con Mensaje de consola
                await session.SendAsync(new ConsoleMessagePacket(
                    Message: $"Bienvenido {packet.Username}",
                    FontIndex: 1,
                    Channel: 0
                ));
            }
        );

        // 2. Levantar el servidor TCP de prueba
        using var server = new TcpGameServer(
            testPort,
            dispatcher,
            NullLogger<TcpGameServer>.Instance
        );
        server.Start();

        // 3. Simular un cliente TCP conectándose
        using var client = new TcpClient();
        await client.ConnectAsync("127.0.0.1", testPort);
        Assert.True(client.Connected);

        NetworkStream stream = client.GetStream();

        // 4. El cliente envía el paquete eLoginExistingChar
        byte[] loginPayload;
        using (var writer = new NetWriter())
        {
            writer.WriteInt16((short)ClientPacketId.eLoginExistingChar); // Header
            writer.WriteInt32(101);                                     // CharId
            writer.WriteString("Gandalf");                              // Nombre
            loginPayload = writer.ToByteArray();
        }

        await stream.WriteAsync(loginPayload);
        await stream.FlushAsync();

        // 5. El cliente lee la primera respuesta: LoggedPacket (3 bytes: [2, 0, 0])
        byte[] loggedBuffer = new byte[3];
        await stream.ReadExactlyAsync(loggedBuffer);

        using (var reader = new NetReader(loggedBuffer))
        {
            short packetId = reader.ReadInt16();
            Assert.Equal((short)ServerPacketId.elogged, packetId);
            bool isNewUser = reader.ReadBoolean();
            Assert.False(isNewUser);
            Assert.False(reader.HasBytes);
        }

        // 6. El cliente lee la segunda respuesta: ConsoleMessagePacket
        byte[] consoleBuffer = new byte[1024];
        int consoleBytesRead = await stream.ReadAsync(consoleBuffer);
        Assert.True(consoleBytesRead > 0);

        using (var reader = new NetReader(consoleBuffer, 0, consoleBytesRead))
        {
            short packetId = reader.ReadInt16();
            Assert.Equal((short)ServerPacketId.eConsoleMsg, packetId);
            string message = reader.ReadString();
            Assert.Equal("Bienvenido Gandalf", message);
            byte font = reader.ReadByte();
            Assert.Equal(1, font);
            byte channel = reader.ReadByte();
            Assert.Equal(0, channel);
            Assert.False(reader.HasBytes);
        }

        // 7. Cerrar limpiamente
        server.Stop();
    }
}
