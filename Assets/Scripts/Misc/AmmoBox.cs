using PurrNet;
using UnityEngine;

public class AmmoBox : NetworkIdentity
{
    private PlayerInventoryNet playerInventoryNet;

    [Header("Rotation Settings")]
    public float rotationSpeed = 100f;
    public int ammoCount = 25;

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
            UpdateMyAmmo(playerID, other.gameObject);
            Destroy(gameObject);
        }

    }

    [TargetRpc]
    void UpdateMyAmmo(PlayerID target, GameObject other)
    {
        other.GetComponent<PlayerInventoryNet>().userInventory.MaxAmmo += ammoCount;
        Debug.Log($"[Server] send to PlayerID: {target}, Collided with {other.name}");
        other.GetComponent<PlayerShootingNet>().UpdateAmmo();
    }
}
