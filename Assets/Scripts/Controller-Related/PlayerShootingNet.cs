using UnityEngine;
using PurrNet;
using System.Collections;

public class PlayerShootingNet : NetworkIdentity
{
    public Transform shootPoint;
    public GameObject bulletProjLocal, bulletProjObserver;
    public PlayerInventoryNet Inventory;
    public PlayerProfileNet playerProfileNet;
    public SendMsgNet sendMsgNet;
    public PlayerHud playerHud;
    public GameObject reloadIndicator;
    public float reloadTime = 10f; // seconds
    private bool canShoot = true;

    void Awake()
    {
        Inventory = GetComponent<PlayerInventoryNet>();
        playerProfileNet = GetComponent<PlayerProfileNet>();
        sendMsgNet = GetComponent<SendMsgNet>();

        // Truely Local, uses scene UI
        playerHud = GameObject.Find("PlayerHud").GetComponent<PlayerHud>();
        reloadIndicator = GameObject.Find("Reload");
        reloadIndicator.SetActive(false);
        playerHud.weaponName.text = $"{Inventory.userInventory.CurrentWeapon}";
        playerHud.weaponimage.sprite = Inventory.userInventory.weaponIcon;
        UpdateAmmo();
    }

    /*
    void Start()
    {
        
        if (!isOwner) return;

        Inventory = GetComponent<PlayerInventoryNet>();
        playerProfileNet = GetComponent<PlayerProfileNet>();
        sendMsgNet = GetComponent<SendMsgNet>();

        // Truely Local, uses scene UI
        playerHud = GameObject.Find("PlayerHud").GetComponent<PlayerHud>();
        reloadIndicator = GameObject.Find("Reload");
        reloadIndicator.SetActive(false);
        playerHud.weaponName.text = $"{Inventory.userInventory.CurrentWeapon}";
        playerHud.weaponimage.sprite = Inventory.userInventory.weaponIcon;
        UpdateAmmo();
    }*/

    void Update()
    {
        // Only the local player should trigger shooting input
        if (!isOwner) return;

        if (Input.GetKeyDown(KeyCode.B))
            Debug.Log($"TimeScale = {Time.timeScale}");

        if (Input.GetMouseButtonDown(0) && canShoot) // Left click
        {
            if (Inventory.userInventory.CurrentAmmo > 0)
            {
                ShootBullet_ServerRpc(Inventory.userInventory.range);
                ShootBullet_Local();
            }
            else
            {
                sendMsgNet.SendToLocal("You are out of bullets!");
            }

        }

        if (Input.GetKeyDown(KeyCode.R) && canShoot)
        {

            StartCoroutine(Reload());
        }

    }
    
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 20), $"FPS: {(1f / Time.unscaledDeltaTime):F0}");
    }
    
    

    [ServerRpc] // runs on the server
    private void ShootBullet_ServerRpc(float range)
    {
        var bullet = Instantiate(bulletProjObserver, shootPoint.position, shootPoint.rotation);

        if (bullet.TryGetComponent(out Rigidbody rb))
        {
            Debug.Log(range);
            rb.linearVelocity = shootPoint.forward * range;
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


    private bool isReloading = false;

    IEnumerator Reload()
    {
        if (!isOwner || isReloading) yield break;

        isReloading = true;
        canShoot = false;

        reloadIndicator.SetActive(true);

        yield return new WaitForSecondsRealtime(reloadTime);

        ReloadAction();

        reloadIndicator.SetActive(false);
        canShoot = true;
        isReloading = false;
    }



}
