using System.Collections;
using PurrNet;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Leaderboard : NetworkIdentity
{
    public MatchManager matchManager;
    public TextMeshPro lbText;
    string message = "";

    protected override void OnSpawned()
    {
        base.OnSpawned();
        if (!isServer)
        {
            return;
        }
        StartCoroutine(UpdateLeaderboard());
    }

    void Update()
    {
        if (!isServer)
        {
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            UpdateManual();
            lbText.text = message;
            UpdateText(message);
        }
    }

    void UpdateManual()
    {
        message = "";
        for (int i = 0; i < matchManager.connectedPlayers.Count; i++)
        {
            message += $"{i + 1}. {matchManager.connectedPlayers[i].Player_Name} - Team {matchManager.connectedPlayers[i].Player_team}\n";
            Debug.Log(message);
        }
    }

    IEnumerator UpdateLeaderboard()
    {
        while (true)
        {
            UpdateManual();
            lbText.text = message;
            UpdateText(message);


            yield return new WaitForSeconds(1f);
        }
    }

    [ObserversRpc(bufferLast: true)]
    void UpdateText(string text)
    {
        lbText.text = text;
    }
}
