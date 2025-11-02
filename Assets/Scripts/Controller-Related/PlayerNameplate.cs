using UnityEngine;
using TMPro;
using PurrNet;

public class PlayerNameplate : MonoBehaviour
{
    [Header("References")]
    public TMP_Text nameText;
    public TMP_Text healthText;
    private Transform cameraTarget; // usually your local camera

    private PlayerProfileNet profile;

    void Start()
    {
        // Find the PlayerProfileNet on parent
        profile = GetComponentInParent<PlayerProfileNet>();
        if (profile == null)
        {
            Debug.LogWarning("PlayerProfileNet not found in parent!");
            return;
        }

        // Assign the camera if not set
        if (cameraTarget == null && Camera.main != null)
            cameraTarget = Camera.main.transform;

        // Initial values
        nameText.text = profile.name_text.text;
        healthText.text = profile.health.value.ToString();

        profile.health.onChanged += OnHealthChanged;
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


    private void OnHealthChanged(int newHealth)
    {
        healthText.text = newHealth.ToString();
    }

    private void OnDestroy()
    {
        if (profile != null)
        {
            profile.health.onChanged -= OnHealthChanged;
        }
    }
}
