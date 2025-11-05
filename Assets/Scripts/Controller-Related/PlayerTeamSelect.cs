using System.Collections.Generic;
using PurrNet;
using UnityEngine;

public class PlayerTeamSelect : NetworkIdentity
{
    [Header("Spawn Points")]
    public List<Transform> spawnpointsPotato;
    public List<Transform> spawnpointsTomato;

    [Header("Prefabs")]
    public NetworkIdentity potatoPlayer;
    public NetworkIdentity tomatoPlayer;

    [Header("UI")]
    public GameObject teamSelectUI;

    private NetworkIdentity _spawnedPlayer;


    // Called from UI button
    public void JoinTeamPotato()
    {
        ServerSpawnPlayer(localPlayer.Value, "Potato");
    }

    public void JoinTeamTomato()
    {
        ServerSpawnPlayer(localPlayer.Value, "Tomato");
    }

    // Runs on the server
    [ServerRpc]
    private void ServerSpawnPlayer(PlayerID playerId, string team)
    {
        NetworkIdentity prefab;
        List<Transform> spawnList;

        if (team == "Potato")
        {
            prefab = potatoPlayer;
            spawnList = spawnpointsPotato;
        }
        else if (team == "Tomato")
        {
            prefab = tomatoPlayer;
            spawnList = spawnpointsTomato;
        } else
        {
            Debug.LogError("Unknown team: " + team);
            return;
        }

        Transform spawn = spawnList[Random.Range(0, spawnList.Count)];

        NetworkIdentity playerObj = Instantiate(prefab, spawn.position, spawn.rotation);
        PlayerProfileNet playerProfileNet = playerObj.GetComponent<PlayerProfileNet>();
        playerProfileNet.playerInfo.teamName = team;
        playerObj.GiveOwnership(playerId);

        TargetHideUI(playerId);
    }


    [TargetRpc]
    private void TargetHideUI(PlayerID target)
    {
        teamSelectUI.SetActive(false);
    }
}
