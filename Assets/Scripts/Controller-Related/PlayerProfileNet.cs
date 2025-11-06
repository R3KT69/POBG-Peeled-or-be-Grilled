using PurrNet;
using TMPro;
using UnityEngine;
using Steamworks;
using PurrNet.Steam;
using PurrNet.Transports;
using System.Collections;

[System.Serializable]
public struct PlayerInfo
{
    public string name;
    public string teamName;
    public int score;
    public int death;
    public int kill;
}

public class PlayerProfileNet : PlayerIdentity<PlayerProfileNet>
{
    [Header("Player Information")]
    public PlayerInfo playerInfo;
    public TMP_Text health_text, name_text;
    public GameObject UI;
    //public string Player_Name;
    public SyncVar<int> health = new(initialValue: 100);
    //public int score;
    public NetworkIdentity networkIdentity;
    private NetworkManager playerNetworkManager;
    private PlayerNameplate playerNameplate;
    private SendMsgNet sendMsgNet;
    private MatchManager matchManager;
    //public string Player_team = "none";


    private void Awake()
    {


        health.onChanged += OnHealthChanged;
        networkIdentity = GetComponent<NetworkIdentity>();
        UI = GameObject.Find("UI");
        sendMsgNet = GetComponent<SendMsgNet>();
        playerNetworkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        matchManager = GameObject.Find("MatchManager").GetComponent<MatchManager>();
        playerNameplate = GetComponentInChildren<PlayerNameplate>();
    }


    protected override void OnSpawned() // Runs both on server and all clients.
    {
        base.OnSpawned();

        if (playerInfo.teamName == "Potato")
        {
            matchManager.potatoTeam.memberCount += 1;
        } else if (playerInfo.teamName == "Tomato")
        {
            matchManager.tomatoTeam.memberCount += 1;
        }
        //UI.GetComponent<UI_references>().deathScreen.SetActive(false);

        Camera cam = GetComponentInChildren<Camera>();
        if (cam == null) return;

        if (!isOwner)
        {
            cam.gameObject.SetActive(false);
            cam.tag = "Untagged";
            return;
        }

        cam.gameObject.SetActive(true);
        cam.tag = "MainCamera";
        playerNameplate.cameraTarget = cam.transform;
        

        if (networkManager.transport is SteamTransport)
        {
            InitializePlayer(SteamFriends.GetPersonaName());
        }
        else if (networkManager.transport is UDPTransport)
        {
            InitializePlayer(Connection_Menu.PlayerName);
        }
        else
        {
            InitializePlayer("Guest");
        }

        matchManager.AddPlayer(gameObject.GetComponent<PlayerProfileNet>());
    }

    [ObserversRpc(bufferLast: true)]
    void InitializePlayer(string name)
    {
        playerInfo.name = name;
        SetPlayerName(playerInfo.name);
        Debug.Log($"Connected: {playerInfo.name}");
        sendMsgNet.SendToAll($"{playerInfo.name} joined the game!");
    }

    void Start()
    {
        if (isOwner)
        {
            GameObject matchCam = GameObject.Find("MatchCamera");
            matchCam.SetActive(false);
        }

        /*
        Player_Name = SteamFriends.GetPersonaName();
        SetPlayerName(Player_Name);
        Debug.Log($"Connected: {Player_Name}");
        sendMsgNet.SendToAll($"{Player_Name} joined the game!");*/
    }


    private void Update()
    {
        /*-----------Client Specific Codes-----------*/
        if (!isOwner) return;

        

        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log($"isServer: {isServer}, isOwner: {isOwner}, id: {networkIdentity.localPlayer.Value}");
        }


        /*-----------Server Specific Codes-----------
        if (!isServer) return;
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            PlayerID clientId = new PlayerID(002, false);
            TargetExample(clientId, "Sent from Server");
        }*/

    }

    [ServerRpc]
    public void TakeDamage(int damage, PlayerProfileNet attacker = null)
    {
        health.value -= damage;

        if (health.value <= 100)
        {
            ChangeNameplateColor(Color.white);
        }

        if (health.value <= 30)
        {
            ChangeNameplateColor(Color.yellow);
        }

        if (health.value <= 0)
        {
            health.value = 0;
            ChangeNameplateColor(Color.red);
            Debug.Log("Has Died");
            
            // Bug here, while player is freezed, if it gets hit then stats stack up
            // need to prevent these lines from excecuting after getting hit initially
            playerInfo.death += 1;
            attacker.playerInfo.kill += 1;
            Debug.Log($"my ID: {owner.Value}");
            StartCoroutine(RespawnAction(owner.Value));
        }
    }

    private IEnumerator RespawnAction(PlayerID target)
    {
        TriggerDeathScreen(target, true);
        yield return new WaitForSeconds(2f);
        TriggerDeathScreen(target, false);
        if (playerInfo.teamName == "Potato")
        {
            RpcRespawn(matchManager.GetComponent<PlayerTeamSelect>().spawnpointsPotato[0].position);
        }
        else if (playerInfo.teamName == "Tomato")
        {
            RpcRespawn(matchManager.GetComponent<PlayerTeamSelect>().spawnpointsTomato[0].position);
        }

        health.value = 100;
        ChangeNameplateColor(Color.white);

        Debug.Log("2 seconds have passed!");
    }

    [ObserversRpc(bufferLast: true)]
    void RpcRespawn(Vector3 newPos)
    {
        transform.position = newPos;
    }

    [TargetRpc]
    void TriggerDeathScreen(PlayerID target, bool state)
    {
        UI.GetComponent<UI_references>().deathScreen.SetActive(state);
    }
    

    [ServerRpc]
    public void GiveHealth(int amount)
    {
        health.value += amount;

        if (health.value >= 30)
        {
            ChangeNameplateColor(Color.white);
        }

        if (health.value >= 100)
        {
            health.value = 110;
            ChangeNameplateColor(Color.cyan);
            Debug.Log("Max Hp");
        }
    }


    private void SetPlayerName(string name)
    {
        name_text.text = name;
    }

    [ObserversRpc(bufferLast: true)]
    public void ChangeNameplateColor(Color color)
    {
        health_text.color = color;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        health.onChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int newValue)
    {
        health_text.text = newValue.ToString();
        //Debug.Log($"Health : {newValue}");
    }

    /*
        [TargetRpc]
        private void TargetExample(PlayerID target, string message)
        {
           Debug.Log($"[Server] Sent to PlayerID: {target} Message: {message}");
        }
    */

}
