using Argentum.Network.Serialization;
using Xunit;

namespace Argentum.Tests.Network;

public class NetBufferTests
{
    [Fact]
    public void WriteAndRead_UserExamplePacket_MatchesExactBytesAndValues()
    {
        // Ejemplo del usuario: { "packetId": 12, "nombre": "Mago", "vida": 150 }
        const short expectedPacketId = 12;
        const string expectedNombre = "Mago";
        const short expectedVida = 150;

        byte[] rawBytes;

        // 1. Serializar con NetWriter
        using (var writer = new NetWriter())
        {
            writer.WriteInt16(expectedPacketId);
            writer.WriteString(expectedNombre);
            writer.WriteInt16(expectedVida);

            rawBytes = writer.ToByteArray();
        }

        // 2. Verificar que la tira de bytes exacta en la red sea de 10 bytes:
        // [12, 0] (id=12) + [4, 0] (len=4) + [77, 97, 103, 111] ("Mago") + [150, 0] (vida=150)
        byte[] expectedBytes = [12, 0, 4, 0, 77, 97, 103, 111, 150, 0];
        Assert.Equal(expectedBytes, rawBytes);

        // 3. Deserializar con NetReader
        using (var reader = new NetReader(rawBytes))
        {
            short actualPacketId = reader.ReadInt16();
            string actualNombre = reader.ReadString();
            short actualVida = reader.ReadInt16();

            Assert.Equal(expectedPacketId, actualPacketId);
            Assert.Equal(expectedNombre, actualNombre);
            Assert.Equal(expectedVida, actualVida);
            Assert.False(reader.HasBytes); // No deben sobrar bytes
        }
    }

    [Fact]
    public void WriteAndRead_MultipleDataTypesWithSpecialCharacters_RoundTripsCorrectly()
    {
        byte expectedByte = 255;
        bool expectedBool = true;
        short expectedInt16 = -32000;
        int expectedInt32 = 123456789;
        float expectedSingle = 3.14159f;
        string expectedTextWithAccents = "¡El Señor de las Sombras ha caído!";

        byte[] buffer;

        using (var writer = new NetWriter())
        {
            writer.WriteByte(expectedByte);
            writer.WriteBoolean(expectedBool);
            writer.WriteInt16(expectedInt16);
            writer.WriteInt32(expectedInt32);
            writer.WriteSingle(expectedSingle);
            writer.WriteString(expectedTextWithAccents);

            buffer = writer.ToByteArray();
        }

        using (var reader = new NetReader(buffer))
        {
            Assert.Equal(expectedByte, reader.ReadByte());
            Assert.Equal(expectedBool, reader.ReadBoolean());
            Assert.Equal(expectedInt16, reader.ReadInt16());
            Assert.Equal(expectedInt32, reader.ReadInt32());
            Assert.Equal(expectedSingle, reader.ReadSingle(), precision: 4);
            Assert.Equal(expectedTextWithAccents, reader.ReadString());
            Assert.Equal(0, reader.Remaining);
        }
    }
}
