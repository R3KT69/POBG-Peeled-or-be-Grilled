using UnityEngine;

public class PlayerMisc : MonoBehaviour
{
    public Animator UI;
    public bool chatBoxDeployed = false; // starts hidden

    void Start()
    {
        UI = GameObject.Find("UI").GetComponent<Animator>();
        UI.SetBool("IsChatOpen", chatBoxDeployed); // sync initial state
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            chatBoxDeployed = !chatBoxDeployed;       // toggle
            UI.SetBool("IsChatOpen", chatBoxDeployed); // Animator handles transition
        }
    }
}
