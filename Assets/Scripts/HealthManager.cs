using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Events;
using System.Collections;

public class HealthManager : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Vignette Settings")]
    public Volume postProcessVolume;

    [Header("Health-Based Vignette Parameters")]
    [SerializeField] private float fullHealthIntensity = 0.3f;
    [SerializeField] private float zeroHealthIntensity = 0.9f;
    [SerializeField] private Color fullHealthColor = Color.black;
    [SerializeField] private Color zeroHealthColor = Color.red;

    [Header("Heart Beat Effect Settings")]
    [SerializeField] private float heartBeatThreshold = 50f;
    [SerializeField] private float heartBeatRate = 1.2f; // beats per second
    [SerializeField] private float heartBeatIntensityMultiplier = 0.3f;
    [SerializeField] private AnimationCurve heartBeatCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vignette vignette;
    private bool isHeartBeating = false;
    private float baseIntensity;
    private Coroutine heartBeatCoroutine;

    // Static instance for easy access
    public static HealthManager Instance { get; private set; }

    [Header("Events")]
    public UnityEvent OnDie; // Inspector-assignable UnityEvent

    // Code Events
    public System.Action<float> OnHealthChanged;
    public System.Action OnDeath;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        currentHealth = maxHealth;
        SetupVignette();
        UpdateVignette();
    }

    private void SetupVignette()
    {
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
                Debug.LogError("HealthManager: No Vignette found in the post-process profile!");
            }
        }
        else
        {
            Debug.LogError("HealthManager: No post-process volume or profile assigned!");
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealth);

        UpdateVignette();
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0f)
        {
            OnDeath?.Invoke();
            OnDie?.Invoke(); // Invoke UnityEvent for inspector assignments
        }
    }

    public void Heal(float healAmount)
    {
        currentHealth = Mathf.Clamp(currentHealth + healAmount, 0f, maxHealth);

        UpdateVignette();
        OnHealthChanged?.Invoke(currentHealth);
    }

    private void UpdateVignette()
    {
        if (vignette == null) return;

        // Calculate health percentage (0 to 1)
        float healthPercentage = currentHealth / maxHealth;

        // Calculate base intensity (inverse of health - lower health = higher intensity)
        baseIntensity = Mathf.Lerp(zeroHealthIntensity, fullHealthIntensity, healthPercentage);

        // Calculate color (blend from black to red based on health)
        Color targetColor = Color.Lerp(zeroHealthColor, fullHealthColor, healthPercentage);

        // Apply base intensity and color
        vignette.intensity.Override(baseIntensity);
        vignette.color.Override(targetColor);

        // Handle heart beat effect
        if (currentHealth <= heartBeatThreshold && currentHealth > 0f)
        {
            if (!isHeartBeating)
            {
                StartHeartBeat();
            }
        }
        else
        {
            if (isHeartBeating)
            {
                StopHeartBeat();
            }
        }
    }

    private void StartHeartBeat()
    {
        if (isHeartBeating) return;

        isHeartBeating = true;
        if (heartBeatCoroutine != null)
        {
            StopCoroutine(heartBeatCoroutine);
        }
        heartBeatCoroutine = StartCoroutine(HeartBeatEffect());
    }

    private void StopHeartBeat()
    {
        if (!isHeartBeating) return;

        isHeartBeating = false;
        if (heartBeatCoroutine != null)
        {
            StopCoroutine(heartBeatCoroutine);
            heartBeatCoroutine = null;
        }

        // Reset to base intensity
        if (vignette != null)
        {
            vignette.intensity.Override(baseIntensity);
        }
    }

    private IEnumerator HeartBeatEffect()
    {
        float beatDuration = 1f / heartBeatRate;
        float halfBeat = beatDuration * 0.5f;

        while (isHeartBeating && currentHealth > 0f)
        {
            // First beat (up)
            yield return StartCoroutine(PulseIntensity(baseIntensity,
                baseIntensity + (heartBeatIntensityMultiplier * (1f - (currentHealth / heartBeatThreshold))),
                halfBeat * 0.3f));

            // First beat (down)
            yield return StartCoroutine(PulseIntensity(
                baseIntensity + (heartBeatIntensityMultiplier * (1f - (currentHealth / heartBeatThreshold))),
                baseIntensity, halfBeat * 0.2f));

            // Short pause
            yield return new WaitForSeconds(halfBeat * 0.1f);

            // Second beat (up)
            yield return StartCoroutine(PulseIntensity(baseIntensity,
                baseIntensity + (heartBeatIntensityMultiplier * (1f - (currentHealth / heartBeatThreshold))),
                halfBeat * 0.2f));

            // Second beat (down)
            yield return StartCoroutine(PulseIntensity(
                baseIntensity + (heartBeatIntensityMultiplier * (1f - (currentHealth / heartBeatThreshold))),
                baseIntensity, halfBeat * 0.2f));

            // Longer pause between heartbeats
            yield return new WaitForSeconds(beatDuration - halfBeat);
        }
    }

    private IEnumerator PulseIntensity(float startIntensity, float endIntensity, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float normalizedTime = elapsedTime / duration;
            float curveValue = heartBeatCurve.Evaluate(normalizedTime);
            float currentIntensity = Mathf.Lerp(startIntensity, endIntensity, curveValue);

            if (vignette != null)
            {
                vignette.intensity.Override(currentIntensity);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (vignette != null)
        {
            vignette.intensity.Override(endIntensity);
        }
    }

    // Public methods for explosion effects to work with current health-based intensity
    public float GetCurrentBaseIntensity()
    {
        return baseIntensity;
    }

    public void ApplyExplosionShake(float shakeDuration, float shakeMagnitude)
    {
        // Get the VignetteShaker and apply shake relative to current health state
        VignetteShaker vignetteShaker = FindObjectOfType<VignetteShaker>();
        if (vignetteShaker != null)
        {
            // Temporarily stop heart beat during explosion
            bool wasHeartBeating = isHeartBeating;
            if (isHeartBeating)
            {
                StopHeartBeat();
            }

            // Apply shake with current base intensity
            vignetteShaker.ApplyHealthBasedShake(shakeDuration, shakeMagnitude, baseIntensity);

            // Restart heart beat after shake if it was active
            if (wasHeartBeating)
            {
                StartCoroutine(RestartHeartBeatAfterDelay(shakeDuration));
            }
        }
    }

    private IEnumerator RestartHeartBeatAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (currentHealth <= heartBeatThreshold && currentHealth > 0f)
        {
            StartHeartBeat();
        }
    }

    // Public getters
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float HealthPercentage => currentHealth / maxHealth;
    public bool IsAlive => currentHealth > 0f;

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
