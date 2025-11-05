using PurrNet;
using UnityEngine;
using System.Collections.Generic;

public class MatchManager : NetworkIdentity
{
    public List<PlayerProfileNet> connectedPlayers = new List<PlayerProfileNet>();

    [ServerRpc]
    public void AddPlayer(PlayerProfileNet player)
    {
        connectedPlayers.Add(player);
        Debug.Log($"Player name:{player.playerInfo.name} id:{player.GetComponent<NetworkIdentity>().owner.Value} joined lobby.");
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
