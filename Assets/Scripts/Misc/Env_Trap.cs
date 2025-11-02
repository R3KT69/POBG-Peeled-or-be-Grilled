using PurrNet;
using UnityEngine;

public class Env_Trap : NetworkIdentity
{
    public float DamageAmount = 25f;

    public void ApplyDamage(GameObject instigator)
    {
        Debug.Log($"{instigator.name} hit {gameObject.name} for {DamageAmount} damage!");
        
    }
}
