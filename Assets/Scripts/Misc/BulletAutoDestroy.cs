using UnityEngine;
using PurrNet;

public class BulletAutoDestroyNet : NetworkIdentity
{
    private void OnTriggerEnter(Collider other)
    {
        // Only the server should destroy networked bullets
        if (!isServer) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log($"Hit player {other.gameObject.GetComponent<PlayerProfileNet>().name_text.text}");
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


    }
    
    

}
