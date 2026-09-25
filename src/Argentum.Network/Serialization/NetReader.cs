using System.Text;

namespace Argentum.Network.Serialization;

/// <summary>
/// Búfer de lectura binaria para paquetes de red entrantes desde el cliente de Argentum Online.
/// </summary>
public sealed class NetReader : IDisposable
{
    private static readonly Encoding Windows1252;

    static NetReader()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Windows1252 = Encoding.GetEncoding(1252);
    }

    private MemoryStream _stream;
    private BinaryReader _reader;

    public NetReader(byte[] buffer) : this(buffer, 0, buffer.Length)
    {
    }

    public NetReader(byte[] buffer, int index, int count)
    {
        _stream = new MemoryStream(buffer, index, count, writable: false);
        _reader = new BinaryReader(_stream, Windows1252, leaveOpen: true);
    }

    /// <summary>
    /// Cantidad de bytes restantes por leer en el paquete.
    /// </summary>
    public int Remaining => (int)(_stream.Length - _stream.Position);

    /// <summary>
    /// Indica si todavía quedan bytes por leer.
    /// </summary>
    public bool HasBytes => Remaining > 0;

    /// <summary>
    /// Lee 1 byte sin signo (0 a 255).
    /// </summary>
    public byte ReadByte() => _reader.ReadByte();

    /// <summary>
    /// Lee 1 byte como booleano (0 = false, distinto de 0 = true).
    /// </summary>
    public bool ReadBoolean() => _reader.ReadByte() != 0;

    /// <summary>
    /// Lee un entero de 16 bits con signo (Integer en VB6, short en C#).
    /// </summary>
    public short ReadInt16() => _reader.ReadInt16();

    /// <summary>
    /// Lee un entero de 32 bits con signo (Long en VB6, int en C#).
    /// </summary>
    public int ReadInt32() => _reader.ReadInt32();

    /// <summary>
    /// Lee un número de punto flotante de 32 bits (Single en VB6, float en C#).
    /// </summary>
    public float ReadSingle() => _reader.ReadSingle();

    /// <summary>
    /// Lee una cantidad específica de bytes crudos.
    /// </summary>
    public byte[] ReadBytes(int count) => _reader.ReadBytes(count);

    /// <summary>
    /// Lee una cadena de texto precedida por su longitud de 16 bits (Int16) en formato Windows-1252.
    /// </summary>
    public string ReadString()
    {
        short length = _reader.ReadInt16();
        if (length <= 0)
        {
            return string.Empty;
        }

        byte[] rawBytes = _reader.ReadBytes(length);
        return Windows1252.GetString(rawBytes);
    }

    public void Dispose()
    {
        _reader.Dispose();
        _stream.Dispose();
    }
}
