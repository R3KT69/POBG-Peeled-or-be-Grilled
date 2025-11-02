using PurrNet;
using TMPro;
using UnityEngine;


public class TestNet_Cpy : NetworkIdentity
{
    [SerializeField] private Color color;
    [SerializeField] private TMP_Text health_text;
    [SerializeField] private Renderer rend;
    [SerializeField] private SyncVar<int> health = new(initialValue: 100);
    [SerializeField] private NetworkTransform netTransform;

    private void Awake()
    {
        health.onChanged += OnChanged;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        health.onChanged -= OnChanged;
    }

    private void OnChanged(int newValue)
    {
        health_text.text = newValue.ToString();
        Debug.Log($"Health : {newValue}");
    }

    protected override void OnSpawned()
    {
        base.OnSpawned();

        if (!isOwner)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
                cam.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        /*-----------Client Specific Codes-----------*/
        if (!isOwner) return;
        
        if (Input.GetKeyDown(KeyCode.G))
        {
            color = new Color(Random.value, Random.value, Random.value);
            SetColor(color);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            TakeDamage(10);
        }

        

        /*-----------Server Specific Codes-----------
        if (!isServer) return;
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            PlayerID clientId = new PlayerID(002, false);
            TargetExample(clientId, "Sent from Server");
        }*/
        
    }


    [ObserversRpc(bufferLast: true)]
    private void SetColor(Color newColor)
    {
        color = newColor;
        rend.material.color = color;
    }

    [ServerRpc]
    private void TakeDamage(int damage)
    {
        health.value -= damage;

        if (health.value <= 0)
        {
            health.value = 0;
            Debug.Log("Has Died");
        }
    }

/*
    [TargetRpc]
    private void TargetExample(PlayerID target, string message)
    {
       Debug.Log($"[Server] Sent to PlayerID: {target} Message: {message}");
    }
*/
    
}
