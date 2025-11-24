using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class udp_listener : MonoBehaviour
{
    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;

    private void Start()
    {
        // Example: Start the UDP client and connect to the remote server
        StartUDPClient(50000);
    }

    private void StartUDPClient(int port)
    {
        Debug.Log($"UDP receiving data on port {port}");
        udpClient = new UdpClient();
        udpClient.EnableBroadcast = true;

        remoteEndPoint = new IPEndPoint(IPAddress.Any, port);

        // Start receiving data asynchronously
        udpClient.BeginReceive(ReceiveData, null);
    }

    private void ReceiveData(IAsyncResult result)
    {
        byte[] receivedBytes = udpClient.EndReceive(result, ref remoteEndPoint);
        string receivedMessage = System.Text.Encoding.UTF8.GetString(receivedBytes);

        Debug.Log("Received from server: " + receivedMessage);

        // Continue receiving data asynchronously
        udpClient.BeginReceive(ReceiveData, null);
    }
}
