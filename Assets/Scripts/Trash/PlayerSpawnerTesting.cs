using UnityEngine;
using PurrNet;

public class PlayerSpawnerTesting : NetworkIdentity
{
    [Header("Prefabs")]
    public NetworkIdentity team1Prefab; // Potato
    public NetworkIdentity team2Prefab; // Tomato

    [Header("Spawn Points")]
    public Transform[] team1SpawnPoints;
    public Transform[] team2SpawnPoints;

    public GameObject teamSelectUI; // UI panel

    private bool hasChosenTeam = false;

    protected override void OnSpawned()
    {
        base.OnSpawned();

        Debug.Log($"[SPAWNER] OnSpawned() | isServer={isServer} | isOwner={isOwner} | isSpawned={isSpawned}");
    }

    // --------------------------
    // Client calls this to pick a team
    // --------------------------
    public void SelectTeamPotato()
    {
        if (hasChosenTeam) return;
        hasChosenTeam = true;

        teamSelectUI.SetActive(false);

        ChooseTeamServerRpc(1, localPlayer.Value);
    }

    public void SelectTeamTomato()
    {
        if (hasChosenTeam) return;
        hasChosenTeam = true;

        teamSelectUI.SetActive(false);

        ChooseTeamServerRpc(2, localPlayer.Value);
    }

    // --------------------------
    // Server handles spawning
    // --------------------------
    [ServerRpc(requireOwnership: false)]
    private void ChooseTeamServerRpc(int teamIndex, PlayerID sender)
    {
        Debug.Log($"[SERVER] Received spawn request from {sender} for team {teamIndex}");

        NetworkIdentity prefabToSpawn;
        Transform[] spawnPoints;

        if (teamIndex == 1)
        {
            prefabToSpawn = team1Prefab;
            spawnPoints = team1SpawnPoints;
        }
        else
        {
            prefabToSpawn = team2Prefab;
            spawnPoints = team2SpawnPoints;
        }

        if (!prefabToSpawn || spawnPoints.Length == 0)
        {
            Debug.LogError("[SERVER] Prefab or spawn points not assigned for this team!");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        NetworkIdentity newPlayer = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);

        // Make sure prefab has PrefabLink
        if (!prefabToSpawn.TryGetComponent<PrefabLink>(out _))
        {
            Debug.LogError($"[SERVER] Prefab '{prefabToSpawn.name}' missing PrefabLink!");
            Destroy(newPlayer.gameObject);
            return;
        }

        newPlayer.Spawn(prefabToSpawn.gameObject, NetworkManager.main);
        newPlayer.GiveOwnership(sender);

        Debug.Log($"[SERVER] Spawned player for {sender} on team {teamIndex} at {spawnPoint.name}");
    }
}
