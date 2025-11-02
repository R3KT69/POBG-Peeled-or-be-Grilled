using PurrNet.Steam;
using Steamworks;
using UnityEngine;

public class SteamInit : MonoBehaviour
{
    public void JoinFriendLobby(ulong friendSteamId)
    {
        SteamAPI.Init();

        
        var steamTransport = GetComponent<SteamTransport>();
        steamTransport.Connect(friendSteamId.ToString(), steamTransport.serverPort);
    }
}