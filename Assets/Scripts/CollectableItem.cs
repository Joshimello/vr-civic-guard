using System.Collections;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    [Header("Effect Settings")]
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private float effectDuration = 2.0f;

    [Header("Optional Settings")]
    [SerializeField] private bool destroyInsteadOfDeactivate = false;

    private bool isCollected = false;

    /// <summary>
    /// Collects this item, spawns effect, and deactivates/destroys after duration
    /// </summary>
    public void Collect()
    {
        if (isCollected) return; // Prevent multiple collections

        isCollected = true;

        // Spawn effect if prefab is assigned
        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, transform);

            // Auto-destroy the effect after duration
            Destroy(effect, effectDuration);
        }

        // Start coroutine to handle timed deactivation
        StartCoroutine(DeactivateAfterDelay());
    }

    private IEnumerator DeactivateAfterDelay()
    {
        yield return new WaitForSeconds(effectDuration);

        if (destroyInsteadOfDeactivate)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
