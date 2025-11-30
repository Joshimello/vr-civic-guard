using UnityEngine;
using System.Collections;

public class CatSpawner : MonoBehaviour
{
    [Header("Spawn Objects")]
    public GameObject spawningEffect;
    public GameObject spawnedEffect;
    public GameObject cat;

    [Header("Timing Settings")]
    public float spawningEffectDuration = 2f;
    public float spawnedEffectDuration = 1f;

    private bool isSpawning = false;

    void Start()
    {
        // Ensure all effects and cat are disabled at start
        if (spawningEffect != null)
            spawningEffect.SetActive(false);

        if (spawnedEffect != null)
            spawnedEffect.SetActive(false);

        if (cat != null)
            cat.SetActive(false);
    }

    public void SpawnCat()
    {
        if (!isSpawning)
        {
            StartCoroutine(SpawnCatSequence());
        }
    }

    private IEnumerator SpawnCatSequence()
    {
        isSpawning = true;

        // Enable spawning effect for 2 seconds
        if (spawningEffect != null)
        {
            spawningEffect.SetActive(true);
        }

        yield return new WaitForSeconds(spawningEffectDuration);

        // Disable spawning effect
        if (spawningEffect != null)
        {
            spawningEffect.SetActive(false);
        }

        // Immediately enable spawned effect and cat
        if (spawnedEffect != null)
        {
            spawnedEffect.SetActive(true);
        }

        if (cat != null)
        {
            cat.SetActive(true);
        }

        yield return new WaitForSeconds(spawnedEffectDuration);

        // Disable spawned effect after 1 second
        if (spawnedEffect != null)
        {
            spawnedEffect.SetActive(false);
        }

        isSpawning = false;
    }

    // Utility properties
    public bool IsSpawning => isSpawning;

    // Optional: Stop spawning sequence if needed
    public void StopSpawning()
    {
        if (isSpawning)
        {
            StopAllCoroutines();

            // Ensure all objects are in correct state
            if (spawningEffect != null)
                spawningEffect.SetActive(false);

            if (spawnedEffect != null)
                spawnedEffect.SetActive(false);

            // Keep cat active if it was already spawned

            isSpawning = false;
        }
    }
}
