using System.Text;

namespace Argentum.Network.Serialization;

/// <summary>
/// Búfer de escritura binaria de alto rendimiento para paquetes de red de Argentum Online.
/// Compatible con el protocolo de tipos y codificación del cliente oficial (Little-Endian, Windows-1252).
/// </summary>
public sealed class NetWriter : IDisposable
{
    private static readonly Encoding Windows1252;

    static NetWriter()
    {
        // Registrar soporte para codificación Windows-1252 si no estuviera disponible
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Windows1252 = Encoding.GetEncoding(1252);
    }

    private MemoryStream _stream;
    private BinaryWriter _writer;

    public NetWriter(int initialCapacity = 256)
    {
        _stream = new MemoryStream(initialCapacity);
        _writer = new BinaryWriter(_stream, Windows1252, leaveOpen: true);
    }

    /// <summary>
    /// Longitud actual en bytes escritos en el búfer.
    /// </summary>
    public int Length => (int)_stream.Length;

    /// <summary>
    /// Escribe 1 byte sin signo (0 a 255).
    /// </summary>
    public void WriteByte(byte value) => _writer.Write(value);

    /// <summary>
    /// Escribe 1 byte booleano (0 = false, 1 = true).
    /// </summary>
    public void WriteBoolean(bool value) => _writer.Write(value ? (byte)1 : (byte)0);

    /// <summary>
    /// Escribe un entero de 16 bits con signo (equivalente a Integer en VB6, short en C#).
    /// </summary>
    public void WriteInt16(short value) => _writer.Write(value);

    /// <summary>
    /// Escribe un entero de 32 bits con signo (equivalente a Long en VB6, int en C#).
    /// </summary>
    public void WriteInt32(int value) => _writer.Write(value);

    /// <summary>
    /// Escribe un número de punto flotante de 32 bits (Single en VB6, float en C#).
    /// </summary>
    public void WriteSingle(float value) => _writer.Write(value);

    /// <summary>
    /// Escribe un bloque de bytes crudos.
    /// </summary>
    public void WriteBytes(ReadOnlySpan<byte> bytes) => _writer.Write(bytes);

    /// <summary>
    /// Escribe una cadena de texto precedida por su longitud de 16 bits (Int16) en formato Windows-1252.
    /// </summary>
    public void WriteString(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            _writer.Write((short)0);
            return;
        }

        byte[] rawBytes = Windows1252.GetBytes(value);
        _writer.Write((short)rawBytes.Length);
        _writer.Write(rawBytes);
    }

    /// <summary>
    /// Obtiene una copia de los bytes escritos hasta el momento.
    /// </summary>
    public byte[] ToByteArray() => _stream.ToArray();

    /// <summary>
    /// Limpia el búfer para ser reutilizado sin reasignar memoria.
    /// </summary>
    public void Reset()
    {
        _stream.SetLength(0);
        _stream.Position = 0;
    }

    public void Dispose()
    {
        _writer.Dispose();
        _stream.Dispose();
    }
}
