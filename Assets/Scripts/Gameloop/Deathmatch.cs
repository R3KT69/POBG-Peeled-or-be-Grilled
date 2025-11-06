using PurrNet;
using TMPro;
using UnityEngine;
using System.Collections;

public class Deathmatch : NetworkIdentity
{
    [Header("References")]
    public MatchManager matchManager;
    public TextMeshPro matchTimer;

    [Header("Timer Settings")]
    public float startTime = 60f;
    private float currentTime;
    private bool isRunning = false;

    private float syncInterval = 1f;
    private float syncTimer = 0f;

    void Awake()
    {
        Debug.Log($"[Awake] isServer={isServer} | isClient={isClient}");
    }

    void Start()
    {
        Debug.Log($"[Start] isServer={isServer} | isClient={isClient}");

        if (startTime <= 0)
        {
            startTime = 60f;
            Debug.LogWarning("startTime was 0! Defaulted to 60 seconds.");
        }
    }

    protected override void OnSpawned()
    {
        base.OnSpawned();
        Debug.Log($"[OnSpawned] isServer={isServer} | isClient={isClient}");

        if (isServer)
        {
            currentTime = startTime;
            isRunning = true;
            UpdateTimerDisplay();   // update UI for server
            SyncTimer(currentTime); // push to all clients
        }
        else
        {
            matchTimer.text = "00:00"; // clients will update after first sync
        }
    }

    void Update()
    {
        if (!isServer || !isRunning)
            return;

        currentTime -= Time.deltaTime;
        syncTimer += Time.deltaTime;

        if (currentTime <= 0f)
        {
            Debug.Log($"Timer reached zero | currentTime={currentTime}");
            currentTime = 0f;
            isRunning = false;
            TimerEnded();
        }

        if (syncTimer >= syncInterval)
        {
            syncTimer = 0f;
            SyncTimer(currentTime);
        }
    }

    // This sends the current time to all clients
    [ObserversRpc(bufferLast: true)]
    void SyncTimer(float time)
    {
        currentTime = time;
        UpdateTimerDisplay();
    }

    // This updates the visible timer text
    void UpdateTimerDisplay()
    {
        if (matchTimer == null)
        {
            Debug.LogWarning("matchTimer reference missing!");
            return;
        }

        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        matchTimer.text = $"{minutes:00}:{seconds:00}";
    }

    [ObserversRpc(bufferLast: true)]
    void TimerEnded()
    {
        string winnerMsg;
        CalculateWinner(out winnerMsg);
        StartCoroutine(ShowWinner(winnerMsg));
    }



    void CalculateWinner(out string winnerMsg)
    {
        int team1_score, team2_score;
        for (int i = 0; i < matchManager.potatoTeam.player.Count; i++)
        {
            matchManager.potatoTeam.score += matchManager.potatoTeam.player[i].playerInfo.score;
        }
        for (int i = 0; i < matchManager.tomatoTeam.player.Count; i++)
        {
            matchManager.tomatoTeam.score += matchManager.tomatoTeam.player[i].playerInfo.score;
        }
        team1_score = matchManager.potatoTeam.score;
        team2_score = matchManager.tomatoTeam.score;

        if (team1_score > team2_score)
        {
            Debug.Log($"Potato team won with score {team1_score}");
            winnerMsg = $"Potato team won with score {team1_score}";
        }
        else if (team1_score < team2_score)
        {
            Debug.Log($"Tomato team won with score {team2_score}");
            winnerMsg = $"Tomato team won with score {team2_score}";
        }
        else
        {
            Debug.Log($"Draw!");
            winnerMsg = $"Potato team won with score {team1_score}";
        }

        ResetAllPositions();
    }
    
    private IEnumerator ShowWinner(string winnerMsg)
    {
        matchManager.global_ui.GetComponent<UI_references>().winnerScreen.SetActive(true);
        matchManager.global_ui.GetComponent<UI_references>().winnerScreen.GetComponent<TextMeshProUGUI>().text = winnerMsg;
        yield return new WaitForSeconds(2);
        matchManager.global_ui.GetComponent<UI_references>().winnerScreen.SetActive(false);
    }

    void ResetAllPositions()
    {
        for (int i = 0; i < matchManager.connectedPlayers.Count; i++)
        {
            PlayerProfileNet selectedPlayer = matchManager.connectedPlayers[i];
            if (selectedPlayer.playerInfo.teamName == "Potato")
            {
                selectedPlayer.RpcRespawn(matchManager.GetComponent<PlayerTeamSelect>().spawnpointsPotato[0].position);
            }
            else if (selectedPlayer.playerInfo.teamName == "Tomato")
            {
                selectedPlayer.RpcRespawn(matchManager.GetComponent<PlayerTeamSelect>().spawnpointsTomato[0].position);
            }

        }
    }
    
}
