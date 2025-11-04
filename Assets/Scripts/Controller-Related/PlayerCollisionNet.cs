using PurrNet;
using UnityEngine;

public class PlayerCollisionNet : NetworkIdentity
{
    private PlayerProfileNet playerProfile;
    private float lastHitTime = 0f;
    private float hitCooldown = 0.1f; 
    void Awake()
    {
        playerProfile = GetComponent<PlayerProfileNet>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;

        if (Time.time - lastHitTime < hitCooldown) return; // collision cooldown
        lastHitTime = Time.time;

        if (other.CompareTag("Traps"))
        {
            Debug.Log($"Player {playerProfile.name_text.text} entered trap: {other.gameObject.name}");
            playerProfile.TakeDamage(10);
        }

        if (other.CompareTag("Bullet"))
        {
            Debug.Log($"Player {playerProfile.name_text.text} got hit by {other.name}");
            playerProfile.TakeDamage(10);
        }
    }
        
    
}
