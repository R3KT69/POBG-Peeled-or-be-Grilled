using LiteNetLib;
using System.Net;
using PurrNet;
using PurrNet.Transports;
using UnityEngine;

public class NetworkDriver : NetworkIdentity
{
    private string ip;
    public string port;
    public NetworkManager networkmngr;
    public UDPTransport transport;
    public SendMsgNet sendMsgNet;

    private void OnEnable()
    {
        transport.onConnected += HandleConnected;
    }

    private void OnDisable()
    {
        transport.onConnected -= HandleConnected;
    }


    void Start()
    {
        ip = Connection_Menu.IPAddr;
        port = Connection_Menu.PortAddr;
        string mode = Connection_Menu.startMode;

        if (string.IsNullOrEmpty(ip))
        {
            Debug.LogWarning("No IP specified, defaulting to localhost");
            ip = "127.0.0.1";
        }


        transport.address = ip;
        transport.serverPort = ushort.Parse(port);

        if (mode == "Host")
        {
            networkmngr.StartHost();
        }
        else if (mode == "Client")
        {
            networkmngr.StartClient();
        }

        Debug.Log(ip + ":" + port);


    }

    /*
    public void OnPeerConnected(NetPeer peer)
    {
        string clientIP = peer.Address.ToString(); // inherited from IPEndPoint
        int clientPort = peer.Port; // inherited from IPEndPoint

        Debug.Log($"Client connected: {clientIP}:{clientPort}");
    }*/

    private void HandleConnected(Connection conn, bool asServer)
    {
        if (!asServer) return; 
        var peer = transport.peers[conn]; 
        Debug.Log($"Client connected: {peer.Address}:{peer.Port}");
        
        sendMsgNet.SendMessageServer($"Client connected: {peer.Address}:{peer.Port}");
        
    }
    
    

    
   

    
    

}
