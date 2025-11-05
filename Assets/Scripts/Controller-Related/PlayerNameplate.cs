using UnityEngine;
using TMPro;
using PurrNet;

public class PlayerNameplate : NetworkIdentity
{
    [Header("References")]
   
    public Transform cameraTarget; // usually your local camera

    private PlayerProfileNet profile;

    void Awake()
    {
        if (!isOwner)
        {
            return;
        }

        // Find the PlayerProfileNet on parent
        profile = GetComponentInParent<PlayerProfileNet>();
        if (profile == null)
        {
            Debug.LogWarning("PlayerProfileNet not found in parent!");
            return;
        }


    }

    void Start()
    {
        //nameText.text = profile.Player_Name;
        //healthText.text = profile.health.value.ToString();
        //profile.health.onChanged += OnHealthChanged;
    }

    protected override void OnSpawned()
    {
        base.OnSpawned();

        GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
        cameraTarget = cam.transform;
    }

    void LateUpdate()
    {
        if (cameraTarget == null) return;

        // Make the nameplate face the camera
        Vector3 direction = cameraTarget.position - transform.position;
        direction.y = 0f; // optional: keep upright
        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
            // Fix TMP flipping
            transform.Rotate(0, 180f, 0f);
        }
    }
    
}
