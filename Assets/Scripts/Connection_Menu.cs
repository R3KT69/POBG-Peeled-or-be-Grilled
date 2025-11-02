using UnityEngine;
using TMPro;
using UnityEditor;

public class Connection_Menu : MonoBehaviour
{
    public TMP_InputField inputField_ipAddress, inputField_Port, inputField_playerName;
    public static string IPAddr;
    public static string PortAddr;
    public static string startMode;
    public static string PlayerName;
    public string sceneNameWAN;
    public string lobbyScene;
    public TextMeshProUGUI text;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SaveConnection();
        }
    }

    public void Start_Srv_host()
    {
        startMode = "Host";
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneNameWAN);
    }

    public void Start_Srv_client()
    {
        startMode = "Client";
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneNameWAN);
    } 

    public void Start_Lobby_Steam()
    {
        startMode = "Steam";
        UnityEngine.SceneManagement.SceneManager.LoadScene(lobbyScene);
    }
    
    public void SaveConnection()
    {
            if (string.IsNullOrWhiteSpace(inputField_ipAddress.text))
            {
                Connection_Menu.IPAddr = "127.0.0.1";
            } else
            {
                Connection_Menu.IPAddr = inputField_ipAddress.text;
            }

            if (string.IsNullOrWhiteSpace(inputField_Port.text))
            {
                Connection_Menu.PortAddr = "7777";
            }
            else
            {
                Connection_Menu.PortAddr = inputField_Port.text;
            }
                
            if (string.IsNullOrWhiteSpace(inputField_playerName.text))
            {
                Connection_Menu.PlayerName = "Guest";
            }else
            {
                Connection_Menu.PlayerName = inputField_playerName.text;
            }

            text.text = $"Name: {PlayerName} | Connection: {IPAddr}:{PortAddr} [Selected]";
            text.color = Color.red;

            Debug.Log(IPAddr);
            Debug.Log(PortAddr);
    }

}
