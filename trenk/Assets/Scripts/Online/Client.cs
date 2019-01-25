using System;
using System.Net.Sockets;

public class Client
{
    private readonly Net net;
    private readonly Socket clientSocket;
    private readonly NetworkStream stream;
    private readonly byte[] readBuffer = new byte[5000];

    public Client(Net net, bool host)
    {
        this.net = net;
        //this.clientSocket = net.ClientSocket;
        this.clientSocket.NoDelay = true;
        this.stream = new NetworkStream(this.clientSocket);

        if (host)
            // Client awaiting EndConnect
            stream.BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);
    }

    public void close()
    {
        stream.Close();
        clientSocket.Close();
    }

    private void OnRead(IAsyncResult ar)
    {
        int length = stream.EndRead(ar);

        //if (length <= 0)
        //    net.OnDisconnect(ar);
        //else
        //{
        //    // Process message
        //    // Prepare to read more
        //    stream.BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);
        //}
    }
}
