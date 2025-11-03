using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Game/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public Sprite weaponSprite;
    public int damage;
    public float range;
    public int magSize;
    public int maxAmmo;
}
