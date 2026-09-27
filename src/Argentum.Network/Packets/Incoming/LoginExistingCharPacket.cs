using Argentum.Network.Serialization;

namespace Argentum.Network.Packets.Incoming;

/// <summary>
/// Paquete enviado por el cliente para iniciar sesión con un personaje existente (eLoginExistingChar).
/// </summary>
public sealed record LoginExistingCharPacket(
    int CharacterId,
    string Username
) : IIncomingPacket
{
    public ClientPacketId Id => ClientPacketId.eLoginExistingChar;

    public static LoginExistingCharPacket Deserialize(NetReader reader)
    {
        int characterId = reader.ReadInt32();
        string username = reader.ReadString();

        return new LoginExistingCharPacket(characterId, username);
    }
}
