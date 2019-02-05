using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class NetTest2 : MonoBehaviour
{
    public short localPort = 9999;
    public short remotePort = 9999;
    public string remoteIp; // Opponent's address
    public bool Server { get; set; }

    private Socket serverSocket; // Listens for connection requests
    private Socket clientSocket; // Opponent endpoint
    private BufferedStream stream; // Wraps socket for data retrieval
    private readonly byte[] readBuffer = new byte[5000];

    public void Listen()
    {
        IPAddress localIpa = IPAddress.Any;

        if (Server)
        {
            IPEndPoint localIpe = new IPEndPoint(localIpa, localPort);

            Debug.Log("Server");

            serverSocket = new Socket(localIpa.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(localIpe);
            serverSocket.Listen(100);
            serverSocket.BeginAccept(OnEndAccept, null);

            Debug.Log("Listening");
        }
        else
        {
            IPAddress remoteIpa = IPAddress.Parse(remoteIp);
            IPEndPoint remoteIpe = new IPEndPoint(remoteIpa, remotePort);

            Debug.Log("Client");

            clientSocket = new Socket(localIpa.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            clientSocket.BeginConnect(remoteIpe, OnEndConnect, null);

            Debug.Log("Connecting");
        }
    }

    protected void OnEndAccept(IAsyncResult ar)
    {
        Debug.Log("Received request");

        clientSocket = serverSocket.EndAccept(ar);
        clientSocket.NoDelay = true; // Improve performance

        stream = new BufferedStream(new NetworkStream(clientSocket));
        stream.BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);

        Debug.Log("Accepted client");
    }

    private void OnEndConnect(IAsyncResult ar)
    {
        Debug.Log("Attempting to end connect");
        clientSocket.EndConnect(ar);
        clientSocket.NoDelay = true; // Improve performance

        stream = new BufferedStream(new NetworkStream(clientSocket));
        stream.BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);

        Debug.Log("Connected to server");
    }

    private void OnRead(IAsyncResult ar)
    {
        Debug.Log("Received");

        int length = stream.EndRead(ar);

        if (length <= 0)
        {
            Debug.Log("Client disconnected");
            OnDisconnect();
        }
        else
        {
            Debug.Log(readBuffer[0]);
            Debug.Log(BitConverter.ToInt16(readBuffer, 1));
            Debug.Log(readBuffer[3]);
        }

        stream.BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);
    }

    public void OnDisconnect()
    {
        clientSocket.Shutdown(SocketShutdown.Both);
        stream.Dispose();
        clientSocket = null;
    }

    public void Send(byte type, short length, byte[] body)
    {
        byte[] lengthBytes = BitConverter.GetBytes(length);

        byte[] data = new byte[1 + 2 + length];

        data[0] = type;
        data[1] = lengthBytes[0];
        data[2] = lengthBytes[1];

        for (int i = 3; i < data.Length; i++)
            data[i] = body[i - 3];

        clientSocket.BeginSend(data, 0, data.Length, 0, null, null);

        Console.WriteLine("Sent message");
    }

    public void SendTest()
    {
        short length = 1;
        Send(0, length, new byte[] { 2 });
    }
}
