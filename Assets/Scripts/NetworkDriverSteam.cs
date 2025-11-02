using LiteNetLib;
using System.Net;
using PurrNet;
using PurrNet.Transports;
using UnityEngine;
using PurrNet.Steam;
using Steamworks;
using PurrLobby;


public class NetworkDriverSteam : NetworkIdentity
{
    private string ip;
    public SteamTransport transport;
    public LobbyManager lobbyManager;

    void Start()
    {

        Debug.Log("Steam initialized. Your ID: " + SteamUser.GetSteamID());
        
        
    }

    

    /*
    public void OnPeerConnected(NetPeer peer)
    {
        string clientIP = peer.Address.ToString(); // inherited from IPEndPoint
        int clientPort = peer.Port; // inherited from IPEndPoint

        Debug.Log($"Client connected: {clientIP}:{clientPort}");
    }*/

}
