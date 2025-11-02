using LiteNetLib;
using System.Net;
using PurrNet;
using PurrNet.Transports;
using UnityEngine;
using PurrNet.Steam;
using Steamworks;

public class NetworkDriverSteam : NetworkIdentity
{
    private string ip;
    public SteamTransport transport;

    void Start()
    {
        Debug.Log("Steam initialized. Your ID: " + SteamUser.GetSteamID());
        transport.address = SteamUser.GetSteamID().ToString();
    }

    /*
    public void OnPeerConnected(NetPeer peer)
    {
        string clientIP = peer.Address.ToString(); // inherited from IPEndPoint
        int clientPort = peer.Port; // inherited from IPEndPoint

        Debug.Log($"Client connected: {clientIP}:{clientPort}");
    }*/

}
