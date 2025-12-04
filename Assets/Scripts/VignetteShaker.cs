using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class VignetteShaker : MonoBehaviour
{
    [Header("References")]
    public Volume postProcessVolume;

    [Header("Shake Settings")]
    public float shakeDuration = 2f;
    public float initialIntensity = 1f;
    public float shakeSpeed = 10f;
    public AnimationCurve intensityCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("Vignette Shake Range")]
    public float maxOffset = 0.5f;

    [Header("Vignette Intensity")]
    public float vignetteIntensity = 0.5f;

    private Vignette vignette;
    private Vector2 defaultCenter = new Vector2(0.5f, 0.5f);
    private float defaultIntensity = 0.3f;
    private bool isShaking = false;
    private float noiseOffsetX;
    private float noiseOffsetY;
    private Color originalColor;
    private float healthBasedIntensity;

    void Start()
    {
        // Get random noise offsets to make each shake unique
        noiseOffsetX = Random.Range(0f, 1000f);
        noiseOffsetY = Random.Range(0f, 1000f);

        // Try to find post process volume if not assigned
        if (postProcessVolume == null)
        {
            postProcessVolume = FindObjectOfType<Volume>();
        }

        // Get the vignette component
        if (postProcessVolume != null && postProcessVolume.profile != null)
        {
            if (!postProcessVolume.profile.TryGet<Vignette>(out vignette))
            {
                Debug.LogError("VignetteShaker: No Vignette found in the post-process profile!");
            }
        }
        else
        {
            Debug.LogError("VignetteShaker: No post-process volume or profile assigned!");
        }
    }

    public void StartShake()
    {
        if (vignette != null && !isShaking)
        {
            StartCoroutine(ShakeVignette());
        }
    }

    public void StartShake(float duration)
    {
        shakeDuration = duration;
        StartShake();
    }

    public void StartShake(float duration, float intensity)
    {
        shakeDuration = duration;
        initialIntensity = intensity;
        StartShake();
    }

    private IEnumerator ShakeVignette()
    {
        isShaking = true;
        float elapsedTime = 0f;

        // Store original color to preserve it during shake
        if (vignette != null)
        {
            originalColor = vignette.color.value;
        }

        while (elapsedTime < shakeDuration)
        {
            float normalizedTime = elapsedTime / shakeDuration;
            float currentIntensity = initialIntensity * intensityCurve.Evaluate(normalizedTime);

            // Generate Perlin noise for smooth random movement
            float noiseX = Mathf.PerlinNoise((Time.time * shakeSpeed) + noiseOffsetX, 0f);
            float noiseY = Mathf.PerlinNoise(0f, (Time.time * shakeSpeed) + noiseOffsetY);

            // Convert noise from 0-1 to -1 to 1 range
            noiseX = (noiseX - 0.5f) * 2f;
            noiseY = (noiseY - 0.5f) * 2f;

            // Apply intensity and max offset
            Vector2 shakeOffset = new Vector2(
                noiseX * currentIntensity * maxOffset,
                noiseY * currentIntensity * maxOffset
            );

            // Apply the shake to vignette center
            vignette.center.value = defaultCenter + shakeOffset;

            // Apply intensity animation (stronger at the beginning, fading back to default)
            float targetIntensity = defaultIntensity + (vignetteIntensity * currentIntensity);
            vignette.intensity.value = targetIntensity;

            // Preserve the original color throughout the shake
            vignette.color.Override(originalColor);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset to default position and intensity
        vignette.center.value = defaultCenter;
        vignette.intensity.value = defaultIntensity;
        isShaking = false;
    }

    public void StopShake()
    {
        if (isShaking)
        {
            StopAllCoroutines();
            if (vignette != null)
            {
                vignette.center.value = defaultCenter;
                vignette.intensity.value = defaultIntensity;
                // Restore original color
                vignette.color.Override(originalColor);
            }
            isShaking = false;
        }
    }

    void OnDestroy()
    {
        // Ensure vignette is reset when object is destroyed
        if (vignette != null)
        {
            vignette.center.value = defaultCenter;
            vignette.intensity.value = defaultIntensity;
        }
    }

    // New method for health-based shake that preserves current health state
    public void ApplyHealthBasedShake(float duration, float magnitude, float baseIntensity)
    {
        if (vignette != null && !isShaking)
        {
            // Store the current health-based intensity and use it as the base
            healthBasedIntensity = baseIntensity;
            defaultIntensity = baseIntensity;

            // Start shake with health-adjusted parameters
            shakeDuration = duration;
            initialIntensity = magnitude;
            StartCoroutine(HealthBasedShakeCoroutine());
        }
    }

    private IEnumerator HealthBasedShakeCoroutine()
    {
        isShaking = true;
        float elapsedTime = 0f;

        // Store original color and intensity to preserve health state
        if (vignette != null)
        {
            originalColor = vignette.color.value;
            healthBasedIntensity = vignette.intensity.value;
        }

        while (elapsedTime < shakeDuration)
        {
            float normalizedTime = elapsedTime / shakeDuration;
            float currentIntensity = initialIntensity * intensityCurve.Evaluate(normalizedTime);

            // Generate Perlin noise for smooth random movement
            float noiseX = Mathf.PerlinNoise((Time.time * shakeSpeed) + noiseOffsetX, 0f);
            float noiseY = Mathf.PerlinNoise(0f, (Time.time * shakeSpeed) + noiseOffsetY);

            // Convert noise from 0-1 to -1 to 1 range
            noiseX = (noiseX - 0.5f) * 2f;
            noiseY = (noiseY - 0.5f) * 2f;

            // Apply intensity and max offset
            Vector2 shakeOffset = new Vector2(
                noiseX * currentIntensity * maxOffset,
                noiseY * currentIntensity * maxOffset
            );

            // Apply the shake to vignette center
            vignette.center.value = defaultCenter + shakeOffset;

            // Apply intensity relative to health-based intensity
            float targetIntensity = healthBasedIntensity + (vignetteIntensity * currentIntensity);
            vignette.intensity.value = targetIntensity;

            // Preserve the health-based color throughout the shake
            vignette.color.Override(originalColor);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset to health-based position, intensity, and color
        vignette.center.value = defaultCenter;
        vignette.intensity.value = healthBasedIntensity;
        vignette.color.Override(originalColor);
        isShaking = false;
    }

    public bool IsShaking => isShaking;
}
