using PurrNet;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SendMsgNet : NetworkIdentity
{
    public GameObject textObject;
    private TMP_InputField inputField;
    private Transform MsgBox;
    private string textToSend;
    private PlayerProfileNet playerProfile;
    

    void Awake()
    {
        inputField = GameObject.Find("Input").GetComponent<TMP_InputField>();
        MsgBox = GameObject.Find("Msg-Box").transform;
        playerProfile = GetComponent<PlayerProfileNet>();
    }

    void Update()
    {
        if (!isOwner) return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendMessage();
        }

    }

    public void SendMessage()
    {
        if (string.IsNullOrWhiteSpace(inputField.text)) return;

        textToSend = $"[{playerProfile.networkIdentity.localPlayer.Value}] {playerProfile.name_text.text}: " + inputField.text;
        SendToAll(textToSend);
        inputField.text = null;
    }
    
    public void SendMessageServer(string Message)
    {
        inputField.text = Message;
        textToSend = inputField.text;
        SendToAll(textToSend);
        inputField.text = null;
    }

    [ObserversRpc(bufferLast: true)]
    public void SendToAll(string message)
    {
        GameObject obj = Instantiate(textObject, MsgBox);
        obj.GetComponent<TextMeshProUGUI>().text = message;
    }
}
