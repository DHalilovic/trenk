
public interface INetSerializer
{
    /// <summary>
    /// Handles received data over network
    /// </summary>
    bool Receive(byte type, byte[] data);
}
