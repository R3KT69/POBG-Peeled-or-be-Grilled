using System.Collections;
using PurrNet;
using UnityEngine;

[System.Serializable]
public struct Inventory
{
    public string CurrentWeapon;
    public int MagSize;
    public int CurrentAmmo;
    public int MaxAmmo;
    public float range;
    public Sprite weaponIcon;
}

public class PlayerInventoryNet : NetworkIdentity
{
    public Inventory userInventory;
    public WeaponData weaponData;
    public SendMsgNet sendMsgNet;

    void Start()
    {
        if (!isOwner) return;
        StartCoroutine(SendEverySec());
    }

    protected override void OnSpawned()
    {
        base.OnSpawned();
        if (!isOwner) return;

        userInventory.CurrentWeapon = weaponData.name;
        userInventory.MagSize = weaponData.magSize;
        userInventory.CurrentAmmo = weaponData.magSize; // Player spawns with 100% ammo
        userInventory.MaxAmmo = weaponData.maxAmmo;
        userInventory.weaponIcon = weaponData.weaponSprite;
        userInventory.range = weaponData.range;
    }
    
    IEnumerator SendEverySec()
    {
        while (true)
        {
            sendMsgNet.SendToLocal($"{userInventory.CurrentAmmo}/{userInventory.MaxAmmo}");
            yield return new WaitForSeconds(1f);
        }
    }

    
}
