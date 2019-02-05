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
    public bool Server { get; set; }

    private const string localIp = "127.0.0.1";
    private Socket serverSocket; // Listens for connection requests
    private Socket clientSocket; // Opponent endpoint
    private BufferedStream stream; // Wraps socket for data retrieval
    private readonly byte[] readBuffer = new byte[5000];

    public void Listen()
    {
        IPAddress myIpa = IPAddress.Parse(localIp);
        IPEndPoint myIpe = new IPEndPoint(myIpa, localPort);

        IPAddress oIpa = IPAddress.Parse(localIp);
        IPEndPoint oIpe = new IPEndPoint(oIpa, localPort);

        if (Server)
        {
            Debug.Log("Server");

            serverSocket = new Socket(myIpa.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(myIpe);
            serverSocket.Listen(100);
            serverSocket.BeginAccept(OnEndAccept, null);
        }
        else
        {
            Debug.Log("Client");

            clientSocket = new Socket(myIpa.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            clientSocket.BeginConnect(oIpe, OnEndConnect, null);
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
            OnDisconnect();
        }
        else
        {
            Debug.Log(BitConverter.ToInt16(readBuffer, 0));
        }

        stream.BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);
    }

    public void OnDisconnect()
    {
        clientSocket.Shutdown(SocketShutdown.Both);
        stream.Dispose();
        clientSocket = null;
    }

    public void Send()
    {
        short s = 2;

        // Must send type, length first but still asynchronously
        clientSocket.BeginSend(BitConverter.GetBytes(s), 0, 2, 0, null, null);

        Console.WriteLine("Sent message");
    }
}
