using UnityEngine;
using PurrNet;

public class BulletSender : NetworkIdentity
{
    public PlayerID ShooterId;

    public void Initialize(PlayerID shooter)
    {
        ShooterId = shooter;
    }
}
