using PurrNet;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct Team
{
    public int memberCount;
    public int score;
    public List<PlayerProfileNet> player;
}

public class MatchManager : NetworkIdentity
{
    public List<PlayerProfileNet> connectedPlayers = new List<PlayerProfileNet>();

    
    public Team potatoTeam, tomatoTeam;

    public GameObject barrier;
    bool matchStarted = false;

    [ServerRpc]
    public void AddPlayer(PlayerProfileNet player)
    {
        connectedPlayers.Add(player);
        Debug.Log($"Player name:{player.playerInfo.name} id:{player.GetComponent<NetworkIdentity>().owner.Value} joined lobby.");
        if (player.playerInfo.teamName == "Potato")
        {
            potatoTeam.player.Add(player);
        }
        else if (player.playerInfo.teamName == "Tomato")
        {
            tomatoTeam.player.Add(player);
        }
    }



    void Start()
    {

    }


    void Update()
    {
        if (!isServer)
        {
            return;
        }

        if (potatoTeam.memberCount > 0 && tomatoTeam.memberCount > 0)
        {
            gameObject.GetComponent<Deathmatch>().enabled = true;
            RemoveBarrier(barrier);
            //matchStarted = true;
        }
    }

    [ObserversRpc(bufferLast: true)]
    void RemoveBarrier(GameObject barrier)
    {
        barrier.SetActive(false);
    }
}
