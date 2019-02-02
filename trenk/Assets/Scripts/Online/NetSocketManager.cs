using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class NetSocketManager
{
    public short localPort = 9999; // Local port
    public short remotePort = 9999; // Opponent's port
    public string remoteIp; // Opponent's address
    public bool Hosting { get; set; } // Is this hosting or joining a game?

    private Socket serverSocket; // Listens for connection requests
    private Socket clientSocket; // Opponent endpoint
    private BufferedStream stream; // Wraps socket for data retrieval
    private INetSerializer serializer;
    private readonly byte[] readBuffer = new byte[5000]; // Store received data

    public NetSocketManager(INetSerializer serializer)
    {
        this.serializer = serializer;
    }

    public void Listen()
    {
        IPAddress localIpa = IPAddress.Any;

        if (Hosting)
        {
            Debug.Log("Host");

            IPEndPoint localIpe = new IPEndPoint(localIpa, localPort);

            serverSocket = new Socket(localIpa.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(localIpe);
            serverSocket.Listen(100);
            serverSocket.BeginAccept(OnEndAccept, null);
        }
        else
        {
            Debug.Log("Client");

            IPAddress remoteIpa = IPAddress.Parse(remoteIp);
            IPEndPoint remoteIpe = new IPEndPoint(remoteIpa, remotePort);

            clientSocket = new Socket(localIpa.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            clientSocket.BeginConnect(remoteIpe, OnEndConnect, null);

            Debug.Log("Connecting");
        }
    }

    public void StopListening()
    {
        if (serverSocket != null)
        {
           //serverSocket.Shutdown(SocketShutdown.Both);
            serverSocket.Dispose();
            serverSocket = null;
        }
    }

    protected void OnEndAccept(IAsyncResult ar)
    {
        Debug.Log("Received request");

        clientSocket = serverSocket.EndAccept(ar);
        clientSocket.NoDelay = true; // Improve performance

        stream = new BufferedStream(new NetworkStream(clientSocket));
        stream.BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);

        EventManager.Instance.Raise("connect", new BoolParam(true));
        Debug.Log("Accepted client");
        StopListening(); // Don't listen for additional clients
    }

    private void OnEndConnect(IAsyncResult ar)
    {
        clientSocket.EndConnect(ar);
        clientSocket.NoDelay = true; // Improve performance

        stream = new BufferedStream(new NetworkStream(clientSocket));
        stream.BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);

        EventManager.Instance.Raise("connect", new BoolParam(false));
        Debug.Log("Connected to server");
    }

    private void OnRead(IAsyncResult ar)
    {
        //Debug.Log("Received");

        int readLength = stream.EndRead(ar);

        if (readLength <= 0)
        {
            Debug.Log("Client disconnected");
            OnDisconnect();
        }
        else
        {
            short bodyLength = BitConverter.ToInt16(readBuffer, 1);
            byte[] body = new byte[bodyLength];
            Array.Copy(readBuffer, 3, body, 0, bodyLength);
            
            serializer.Receive(readBuffer[0], body);
        }

        stream.BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);
    }

    public void OnDisconnect()
    {
        if (clientSocket != null)
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket = null;
        }

        if (stream != null)
            stream.Dispose();
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

        //Debug.Log("Sent message");
    }
}
