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
    public GameObject target;
    public float fireRate = 0.2f; // seconds between shots
    private float nextFireTime = 0f;
    public float reloadTime = 10f; // seconds
    private bool canShoot = true;
    public float holdDuration = 2f; // seconds to reach full
    private float holdTime = 0f;
    private bool isHolding = false;

    void Awake()
    {
        Inventory = GetComponent<PlayerInventoryNet>();
        playerProfileNet = GetComponent<PlayerProfileNet>();
        sendMsgNet = GetComponent<SendMsgNet>();

        /*/ Truely Local, uses scene UI
        playerHud = GameObject.Find("PlayerHud").GetComponent<PlayerHud>();
        reloadIndicator = GameObject.Find("Reload");
        reloadIndicator.SetActive(false);
        playerHud.weaponName.text = $"{Inventory.userInventory.CurrentWeapon}";
        playerHud.weaponimage.sprite = Inventory.userInventory.weaponIcon;
        UpdateAmmo();*/
    }

    void Start()
    {
        if (!isOwner)
        {
            target.SetActive(false);
            return;
        }
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

    void ShootInstant(float modifier = 1)
    {
        ShootBullet_ServerRpc(Inventory.userInventory.range * modifier);
        ShootBullet_Local(Inventory.userInventory.range * modifier);
        Inventory.currentWeapon.FireWeaponVisual();
        Inventory.currentWeapon.PlayerWepSound();
    }

    void ShootStrength()
    {
        if (Input.GetMouseButton(0) && canShoot)
        {
            isHolding = true;
            holdTime += Time.deltaTime;
            holdTime = Mathf.Clamp(holdTime, 0f, holdDuration);
        }

        // Trigger action when released
        if (Input.GetMouseButtonUp(0))
        {
            isHolding = false;
            OnHoldReleased();
            holdTime = 0f; // reset or leave to drain gradually
        }

        // Gradually drain if not holding
        if (!Input.GetMouseButton(0))
        {
            holdTime -= Time.deltaTime * 0.5f; // optional slower drain
            holdTime = Mathf.Clamp(holdTime, 0f, holdDuration);
        }

        // Update UI bar
        if (playerHud != null && playerHud.StrengthBar != null)
        {
            playerHud.StrengthBar.fillAmount = holdTime / holdDuration;
        }
    }
    
    void OnHoldReleased()
    {
        float strength = GetStrengthNormalized();
        Debug.Log($"Released! Strength = {strength}");

        ShootInstant(strength);
        // Example: fire a charged shot based on strength
        
    }

    void Update()
    {
        // Only the local player should trigger shooting input
        if (!isOwner) return;

        if (Input.GetKeyDown(KeyCode.B))
            Debug.Log($"TimeScale = {Time.timeScale}");


        // Instant Shooting
        /*
        if (Input.GetMouseButtonDown(0) && canShoot && Time.time >= nextFireTime) // Left click
        {
            if (Inventory.userInventory.CurrentAmmo > 0)
            {
                nextFireTime = Time.time + fireRate;
                ShootInstant();
            }
            else
            {
                sendMsgNet.SendToLocal("You are out of bullets!");
            }

        }*/

        // Strength Based
        
        if (Inventory.userInventory.CurrentAmmo > 0 && canShoot)
        {
            ShootStrength();
        }
        
        
        //Debug.Log(holdTime);
        

        if (Input.GetKeyDown(KeyCode.R) && canShoot)
        {

            StartCoroutine(Reload());
        }

    }

    public float GetStrengthNormalized()
    {
        return Mathf.Clamp01(holdTime / holdDuration);
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

    private void ShootBullet_Local(float range)
    {
        var bullet = Instantiate(bulletProjLocal, shootPoint.position, shootPoint.rotation);

        if (bullet.TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = shootPoint.forward * range;
        }

        Inventory.userInventory.CurrentAmmo -= 1;
        UpdateAmmo();
    }

    public void UpdateAmmo()
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
