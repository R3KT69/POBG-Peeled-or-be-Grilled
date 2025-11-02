using UnityEngine;
using PurrNet;

public class PlayerShootingNet : NetworkIdentity
{
    public Transform shootPoint;
    public GameObject bulletProjLocal, bulletProjObserver;
    public float bulletSpeed = 25f;

    void Update()
    {
        // Only the local player should trigger shooting input
        if (!isOwner) return;

        if (Input.GetMouseButtonDown(0)) // Left click
        {
            ShootBullet_ServerRpc();
            ShootBullet_Local();
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
            rb.linearVelocity = shootPoint.forward * bulletSpeed;
        }
    }
}
