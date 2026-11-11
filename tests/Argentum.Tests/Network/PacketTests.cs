using Argentum.Core.Enums;
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
        const string message = "Bienvenido a Argentum Online!";
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
    public void PosUpdatePacket_SerializesCorrectly()
    {
        const byte x = 45;
        const byte y = 80;

        var packet = new PosUpdatePacket(x, y);

        using var writer = new NetWriter();
        packet.Serialize(writer);

        using var reader = new NetReader(writer.ToByteArray());
        short packetId = reader.ReadInt16();
        byte readX = reader.ReadByte();
        byte readY = reader.ReadByte();

        Assert.Equal((short)ServerPacketId.ePosUpdate, packetId);
        Assert.Equal(x, readX);
        Assert.Equal(y, readY);
        Assert.False(reader.HasBytes);
    }

    [Fact]
    public void CharacterMovePacket_SerializesCorrectly()
    {
        const short characterIndex = 12;
        const byte x = 50;
        const byte y = 51;

        var packet = new CharacterMovePacket(characterIndex, x, y);

        using var writer = new NetWriter();
        packet.Serialize(writer);

        using var reader = new NetReader(writer.ToByteArray());
        short packetId = reader.ReadInt16();
        short readCharIndex = reader.ReadInt16();
        byte readX = reader.ReadByte();
        byte readY = reader.ReadByte();

        Assert.Equal((short)ServerPacketId.eCharacterMove, packetId);
        Assert.Equal(characterIndex, readCharIndex);
        Assert.Equal(x, readX);
        Assert.Equal(y, readY);
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

    [Fact]
    public void WalkPacket_DeserializesCorrectly()
    {
        const Heading expectedHeading = Heading.East;
        const int expectedPacketCount = 12345;

        byte[] rawPayload;
        using (var writer = new NetWriter())
        {
            writer.WriteByte((byte)expectedHeading);
            writer.WriteInt32(expectedPacketCount);
            rawPayload = writer.ToByteArray();
        }

        using var reader = new NetReader(rawPayload);
        var packet = WalkPacket.Deserialize(reader);

        Assert.Equal(ClientPacketId.eWalk, packet.Id);
        Assert.Equal(expectedHeading, packet.Heading);
        Assert.Equal(expectedPacketCount, packet.PacketCount);
        Assert.False(reader.HasBytes);
    }

    [Fact]
    public void RequestPositionUpdatePacket_DeserializesCorrectly()
    {
        byte[] rawPayload = [];
        using var reader = new NetReader(rawPayload);
        var packet = RequestPositionUpdatePacket.Deserialize(reader);

        Assert.Equal(ClientPacketId.eRequestPositionUpdate, packet.Id);
        Assert.False(reader.HasBytes);
    }
}
