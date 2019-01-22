using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

public class Net
{
    public readonly short localPort = 8080;
    public readonly short targetPort = 8080;

    private INetSerializer serializer; // Parses bytestreams
    private readonly bool server; // Is this user a server?
    private readonly string targetIp; // Opponent's address
    private Socket listenerSocket; // Listens for connection requests
    private Socket clientSocket; // Opponent endpoint
    private BufferedStream stream; // Wraps socket for data retrieval
    private readonly byte[] readBuffer = new byte[5000];

    public Net(INetSerializer serializer, bool server, string targetIp, short targetPort)
    {
        this.serializer = serializer;
        this.server = server;
        this.targetIp = targetIp;
        this.targetPort = targetPort;
    }

    // Testing without serializer
    public Net(bool server, string targetIp, short targetPort)
    {
        this.server = server;
        this.targetIp = targetIp;
        this.targetPort = targetPort;
    }

    public void Listen(int port)
    {
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress localIp = ipHostInfo.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(localIp, localPort);

        if (server)
        {
            listenerSocket = new Socket(localIp.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            listenerSocket.Bind(localEndPoint);
            listenerSocket.Listen(100);
            listenerSocket.BeginAccept(OnListenerConnect, null);
        }
        else
        {
            clientSocket = new Socket(localIp.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            clientSocket.BeginConnect(new IPEndPoint(IPAddress.Parse(targetIp), targetPort), OnEndConnect, null);
        }
    }

    protected void OnListenerConnect(IAsyncResult ar)
    {
        Socket temp = listenerSocket.EndAccept(ar);

        // Accept only client provided by matchmaking
        if (temp.RemoteEndPoint.ToString() == targetIp)
        {
            clientSocket = temp;
            clientSocket.NoDelay = true; // Improve performance
            stream = new BufferedStream(new NetworkStream(clientSocket));
        }
        else
        {
            temp.Close();
        }
    }

    private void OnEndConnect(IAsyncResult ar)
    {
        clientSocket.EndConnect(ar);
    }

    public void OnDisconnect(IAsyncResult ar)
    {
        stream.Dispose();
        clientSocket = null;
    }

    public void Send(short type, int length, byte[] data)
    {
        // Must send type, length first but still asynchronously
        stream.WriteAsync(BitConverter.GetBytes(type), 0, 2);
        stream.WriteAsync(BitConverter.GetBytes(length), 0, 4);
        stream.WriteAsync(data, 0, 4);
    }
}
