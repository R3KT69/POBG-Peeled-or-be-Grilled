using UnityEngine;
using System.Collections;
using PurrNet;

public class Weapon : NetworkIdentity
{
    public WeaponData weaponData;
    public AudioSource audioSource;
    public ParticleSystem muzzleFlash;
    public Light muzzleLight;

    void Start()
    {
        if (muzzleLight != null) muzzleLight.enabled = false;
    }

    [ObserversRpc(bufferLast: true)]
    public void FireWeaponVisual()
    {
        if (muzzleFlash != null) muzzleFlash.Play();
        if (muzzleLight != null) StartCoroutine(FlashMuzzleLight());
    }
    
    [ObserversRpc(bufferLast: true)]
    public void PlayerWepSound()
    {
        if (audioSource != null) audioSource.PlayOneShot(weaponData.fireSound);
    }

    private IEnumerator FlashMuzzleLight()
    {
        muzzleLight.enabled = true;
        yield return new WaitForSeconds(0.1f); // light on duration (100 ms)
        muzzleLight.enabled = false;
    }
}
