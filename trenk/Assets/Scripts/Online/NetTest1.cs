using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class NetTest1 : MonoBehaviour
{
    public short localPort = 8080;
    public short targetPort = 8080;
    public string targetIp; // Opponent's address
    public bool server;

    private Socket listenerSocket; // Listens for connection requests
    private Socket clientSocket; // Opponent endpoint
    private BufferedStream stream; // Wraps socket for data retrieval
    private readonly byte[] readBuffer = new byte[5000];

    public void Listen()
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

        Debug.Log("Listening...");
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
        stream.BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);
    }

    private void OnRead(IAsyncResult ar)
    {
        int length = stream.EndRead(ar);

        if (length <= 0)
        {
            OnDisconnect();
        }
        else
        {
            Debug.Log(BitConverter.ToInt16(readBuffer, 0));
        }
    }

    public void OnDisconnect()
    {
        stream.Dispose();
        clientSocket = null;
    }

    public void Send(short message)
    {
        // Must send type, length first but still asynchronously
        stream.WriteAsync(BitConverter.GetBytes(message), 0, 2);
    }
}
