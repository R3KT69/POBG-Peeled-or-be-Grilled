using UnityEngine;
using PurrNet;

public class PlayerShootingNet : NetworkIdentity
{
    public Transform shootPoint;
    public GameObject bulletProjLocal, bulletProjObserver;
    public PlayerInventoryNet Inventory;
    public SendMsgNet sendMsgNet;
    public PlayerHud playerHud;
    public float bulletSpeed = 25f;

    void Start()
    {
        if (!isOwner) return;

        Inventory = GetComponent<PlayerInventoryNet>();
        sendMsgNet = GetComponent<SendMsgNet>();
        playerHud = GameObject.Find("PlayerHud").GetComponent<PlayerHud>();
        playerHud.weaponName.text = $"{Inventory.userInventory.CurrentWeapon}";
        playerHud.weaponimage.sprite = Inventory.userInventory.weaponIcon;
        UpdateAmmo();
    }

    void Update()
    {
        // Only the local player should trigger shooting input
        if (!isOwner) return;

        if (Input.GetMouseButtonDown(0)) // Left click
        {
            if (Inventory.userInventory.CurrentAmmo > 0)
            {
                ShootBullet_ServerRpc();
                ShootBullet_Local();
            }
            else
            {
                sendMsgNet.SendToLocal("You are out of bullets!");
            }

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadAction();
        }

    }
    
    

    [ServerRpc] // runs on the server
    private void ShootBullet_ServerRpc()
    {
        var bullet = Instantiate(bulletProjObserver, shootPoint.position, shootPoint.rotation);

        if (bullet.TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = shootPoint.forward * bulletSpeed;
        }
    }

    private void ShootBullet_Local()
    {
        var bullet = Instantiate(bulletProjLocal, shootPoint.position, shootPoint.rotation);

        if (bullet.TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = shootPoint.forward * Inventory.userInventory.range;
        }

        Inventory.userInventory.CurrentAmmo -= 1;
        UpdateAmmo();
    }

    void UpdateAmmo()
    {
        playerHud.weaponAmmoStatus.text = $"{Inventory.userInventory.CurrentAmmo}/{Inventory.userInventory.MaxAmmo}";
    }

    void ReloadAction()
    {
        int neededAmmo = Inventory.userInventory.MagSize - Inventory.userInventory.CurrentAmmo; // bullets needed to fill mag
        if (neededAmmo <= 0 || Inventory.userInventory.MaxAmmo <= 0) return; // nothing to reload

        int ammoToLoad = Mathf.Min(neededAmmo, Inventory.userInventory.MaxAmmo);

        Inventory.userInventory.CurrentAmmo += ammoToLoad;
        Inventory.userInventory.MaxAmmo -= ammoToLoad;

        UpdateAmmo();
    }
}
