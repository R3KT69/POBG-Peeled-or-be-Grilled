using PurrNet;
using UnityEngine;

public class BulletSender : NetworkIdentity
{
    // SyncVar ensures all observers get the updated value
    public SyncVar<string> sender = new(initialValue: "");

    /// <summary>
    /// Called whenever the sender changes.
    /// You can use this for UI updates, kill feed, etc.
    /// </summary>
    private void OnSenderChanged(string newValue)
    {
        Debug.Log($"Bullet fired by: {newValue}");
    }

    private void Awake()
    {
        // Subscribe to the SyncVar change event
        sender.onChanged += OnSenderChanged;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        // Unsubscribe to prevent memory leaks
        sender.onChanged -= OnSenderChanged;
    }

    /// <summary>
    /// Sets the bullet's sender. Must be called on the server.
    /// </summary>
    /// <param name="playerName">Name of the player who fired the bullet</param>
    public void SetSender(string playerName)
    {
        if (!isServer) return; // Only server can change SyncVars
        sender.value = playerName;
    }
}