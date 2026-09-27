using Argentum.Network.Packets;
using Argentum.Network.Packets.Incoming;
using Argentum.Network.Packets.Outgoing;
using Argentum.Network.Serialization;
using Xunit;

namespace Argentum.Tests.Network;

public class PacketTests
{
    [Fact]
    public void LoggedPacket_SerializesCorrectly()
    {
        var packet = new LoggedPacket(IsNewUser: false);

        using var writer = new NetWriter();
        packet.Serialize(writer);

        byte[] bytes = writer.ToByteArray();

        // ServerPacketId.elogged es 2 en short (Int16) -> [0x02, 0x00]
        // IsNewUser (false) -> [0x00]
        byte[] expected = [(byte)ServerPacketId.elogged, 0, 0];
        Assert.Equal(expected, bytes);
    }

    [Fact]
    public void ConsoleMessagePacket_SerializesExpectedPayload()
    {
        const string message = "¡Bienvenido a Argentum Online!";
        const byte font = 1;
        const byte channel = 0;

        var packet = new ConsoleMessagePacket(message, font, channel);

        using var writer = new NetWriter();
        packet.Serialize(writer);

        using var reader = new NetReader(writer.ToByteArray());
        short packetId = reader.ReadInt16();
        string readMessage = reader.ReadString();
        byte readFont = reader.ReadByte();
        byte readChannel = reader.ReadByte();

        Assert.Equal((short)ServerPacketId.eConsoleMsg, packetId);
        Assert.Equal(message, readMessage);
        Assert.Equal(font, readFont);
        Assert.Equal(channel, readChannel);
        Assert.False(reader.HasBytes);
    }

    [Fact]
    public void LoginExistingCharPacket_DeserializesCorrectly()
    {
        const int expectedCharId = 42;
        const string expectedUsername = "PaladinHeroico";

        byte[] rawPayload;
        using (var writer = new NetWriter())
        {
            writer.WriteInt32(expectedCharId);
            writer.WriteString(expectedUsername);
            rawPayload = writer.ToByteArray();
        }

        using var reader = new NetReader(rawPayload);
        var packet = LoginExistingCharPacket.Deserialize(reader);

        Assert.Equal(ClientPacketId.eLoginExistingChar, packet.Id);
        Assert.Equal(expectedCharId, packet.CharacterId);
        Assert.Equal(expectedUsername, packet.Username);
        Assert.False(reader.HasBytes);
    }
}
