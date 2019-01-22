using System;
using System.Net;
using System.Net.Sockets;

public class Net
{
    private INetSerializer serializer; // Parses bytestreams.
    private Socket listenerSocket; // Listens for connection requests
    private Socket clientSocket; // Opponent endpoint
    private NetworkStream stream;
    private bool server; // Is this user a server?
    private string clientIp; // Opponent's address
    private readonly byte[] readBuffer = new byte[5000];

    public Net(INetSerializer serializer, string clientIp)
    {
        this.serializer = serializer;
    }

    public void Listen(int port)
    {
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

        if (server)
        {
            listenerSocket = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            listenerSocket.Bind(localEndPoint);
            listenerSocket.Listen(100);
            listenerSocket.BeginAccept(OnListenerConnect, null);
        }
        else
        {

        }
    }

    protected void OnListenerConnect(IAsyncResult ar)
    {
        Socket temp = listenerSocket.EndAccept(ar);

        // Accept only client provided by matchmaking
        if (temp.RemoteEndPoint.ToString() == clientIp)
        {
            clientSocket = temp;
            stream = new NetworkStream(clientSocket);
        }
    }

    public void OnDisconnect(IAsyncResult ar)
    {
        clientSocket.Close();
        clientSocket = null;
    }

    public void Send(short type, int length, byte[] data)
    {

    }


}
