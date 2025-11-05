using PurrNet;
using UnityEngine;

public class HealthBox : NetworkIdentity
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 100f;
    public int healthCount = 25;

    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;

        if (other.CompareTag("Player"))
        {
            PlayerID playerID = other.gameObject.GetComponent<NetworkIdentity>().owner.Value; // How to get this from the "other" object
            UpdateMyHealth(playerID, other.gameObject);
            Destroy(gameObject);
        }

    }

    [TargetRpc]
    void UpdateMyHealth(PlayerID target, GameObject other)
    {
        other.GetComponent<PlayerProfileNet>().GiveHealth(healthCount);
        Debug.Log($"[Server] Health send to PlayerID: {target}, Collided with {other.name}");
    }
}
