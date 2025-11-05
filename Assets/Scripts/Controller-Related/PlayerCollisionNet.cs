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
            var bullet = other.GetComponent<BulletSender>();
            
            // Get shooter via PlayerIdentity
            if (PlayerProfileNet.TryGetPlayer(bullet.ShooterId, out var shooter))
            {
                shooter.playerInfo.score += 10;
                Debug.Log($"Score given to: {shooter.playerInfo.name}");
            }

            // Apply damage to this player
            playerProfile.TakeDamage(10);
        }

    }
    
}
