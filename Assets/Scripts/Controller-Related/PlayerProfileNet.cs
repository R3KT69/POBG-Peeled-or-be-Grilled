using PurrNet;
using TMPro;
using UnityEngine;
using Steamworks;
using PurrNet.Steam;
using PurrNet.Transports;

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
    //public Color color;
    public TMP_Text health_text, name_text;
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
        sendMsgNet = GetComponent<SendMsgNet>();
        playerNetworkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        matchManager = GameObject.Find("MatchManager").GetComponent<MatchManager>();
        playerNameplate = GetComponentInChildren<PlayerNameplate>();
    }


    protected override void OnSpawned() // Runs both on server and all clients.
    {
        base.OnSpawned();

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

        if (Input.GetKeyDown(KeyCode.F))
        {
            TakeDamage(10);
        }

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
    public void TakeDamage(int damage)
    {
        health.value -= damage;

        if (health.value <= 0)
        {
            health.value = 0;
            ChangeNameplateColor(Color.red);
            Debug.Log("Has Died");
        }
    }


    private void SetPlayerName(string name)
    {
        name_text.text = name;
    }

    [ObserversRpc(bufferLast: true)]
    public void ChangeNameplateColor(Color color)
    {
        name_text.color = color;
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
