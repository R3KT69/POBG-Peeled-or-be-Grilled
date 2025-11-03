using System.Collections;
using PurrNet;
using UnityEngine;

public struct Inventory
{
    public string CurrentWeapon;
    public int MagSize;
    public int CurrentAmmo;
    public int MaxAmmo;
}

public class PlayerInventoryNet : NetworkIdentity
{
    public Inventory userInventory;
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

        userInventory.CurrentWeapon = "PotatoCanon";
        userInventory.MagSize = 5;
        userInventory.CurrentAmmo = 5;
        userInventory.MaxAmmo = 25;
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
