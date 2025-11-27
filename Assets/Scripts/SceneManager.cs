using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneManager : MonoBehaviour
{
    [Header("References")]
    public Volume postProcessVolume;
    public GameObject spawnPoint;
    public GameObject player;

    [Header("Spawn Settings")]
    public bool setLookDirection = true;
    public float defaultLookDirection = 180f; // Look behind

    [Header("Fade Settings")]
    public float fadeInDuration = 1f;
    public float fadeOutDuration = 1f;
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Vignette Fade Settings")]
    public Vector2 defaultCenter = new Vector2(0.5f, 0.5f);
    public float defaultIntensity = 0.3f;
    public Vector2 fadeStartCenter = new Vector2(0.5f, 0f);
    public float fadeStartIntensity = 1f;

    private Vignette vignette;
    private bool isFading = false;

    // Static instance for easy access
    public static SceneManager Instance { get; private set; }

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
        // Try to find post process volume if not assigned
        if (postProcessVolume == null)
        {
            postProcessVolume = FindObjectOfType<Volume>();
        }

        // Get the vignette component
        if (postProcessVolume != null && postProcessVolume.profile != null)
        {
            if (postProcessVolume.profile.TryGet<Vignette>(out vignette))
            {
                // Start with vignette at fade start position and intensity
                vignette.center.Override(fadeStartCenter);
                vignette.intensity.Override(fadeStartIntensity);

                // Teleport player to spawn point
                TeleportPlayerToSpawnPoint();

                // Fade in on scene start
                StartFadeIn();
            }
            else
            {
                Debug.LogError("SceneManager: No Vignette found in the post-process profile!");
            }
        }
        else
        {
            Debug.LogError("SceneManager: No post-process volume or profile assigned!");
        }
    }

    public void StartFadeIn()
    {
        if (vignette != null && !isFading)
        {
            StartCoroutine(FadeVignette(fadeStartCenter, fadeStartIntensity, defaultCenter, defaultIntensity, fadeInDuration));
        }
    }

    public void ChangeScene(string sceneName)
    {
        if (!isFading)
        {
            StartCoroutine(ChangeSceneWithFade(sceneName));
        }
    }

    public void ChangeScene(int sceneIndex)
    {
        if (!isFading)
        {
            StartCoroutine(ChangeSceneWithFade(sceneIndex));
        }
    }

    private IEnumerator ChangeSceneWithFade(string sceneName)
    {
        // Fade out (vignette to fade start position and intensity)
        yield return StartCoroutine(FadeVignette(defaultCenter, defaultIntensity, fadeStartCenter, fadeStartIntensity, fadeOutDuration));

        // Load the new scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);

        // Note: Fade in will happen automatically in Start() of the new scene
    }

    private IEnumerator ChangeSceneWithFade(int sceneIndex)
    {
        // Fade out (vignette to fade start position and intensity)
        yield return StartCoroutine(FadeVignette(defaultCenter, defaultIntensity, fadeStartCenter, fadeStartIntensity, fadeOutDuration));

        // Load the new scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);

        // Note: Fade in will happen automatically in Start() of the new scene
    }

    private IEnumerator FadeVignette(Vector2 startCenter, float startIntensity, Vector2 endCenter, float endIntensity, float duration)
    {
        if (vignette == null)
        {
            Debug.LogWarning("SceneManager: Vignette not available for fade!");
            yield break;
        }

        isFading = true;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float normalizedTime = elapsedTime / duration;
            float curveValue = fadeCurve.Evaluate(normalizedTime);

            // Interpolate vignette center and intensity
            Vector2 currentCenter = Vector2.Lerp(startCenter, endCenter, curveValue);
            float currentIntensity = Mathf.Lerp(startIntensity, endIntensity, curveValue);

            vignette.center.Override(currentCenter);
            vignette.intensity.Override(currentIntensity);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure we end at the exact target values
        vignette.center.Override(endCenter);
        vignette.intensity.Override(endIntensity);
        isFading = false;
    }

    public void FadeOut(System.Action onComplete = null)
    {
        if (!isFading)
        {
            StartCoroutine(FadeOutCoroutine(onComplete));
        }
    }

    private IEnumerator FadeOutCoroutine(System.Action onComplete)
    {
        yield return StartCoroutine(FadeVignette(defaultCenter, defaultIntensity, fadeStartCenter, fadeStartIntensity, fadeOutDuration));
        onComplete?.Invoke();
    }

    public void FadeIn(System.Action onComplete = null)
    {
        if (!isFading)
        {
            StartCoroutine(FadeInCoroutine(onComplete));
        }
    }

    private IEnumerator FadeInCoroutine(System.Action onComplete)
    {
        yield return StartCoroutine(FadeVignette(fadeStartCenter, fadeStartIntensity, defaultCenter, defaultIntensity, fadeInDuration));
        onComplete?.Invoke();
    }

    // Utility properties
    public bool IsFading => isFading;

    private void TeleportPlayerToSpawnPoint()
    {
        if (player != null && spawnPoint != null)
        {
            Vector3 playerPosition = player.transform.position;
            Vector3 spawnPosition = spawnPoint.transform.position;

            // Keep player's Y position, only change X and Z
            player.transform.position = new Vector3(spawnPosition.x, playerPosition.y, spawnPosition.z);

            // Set default look direction if enabled
            if (setLookDirection)
            {
                Vector3 currentRotation = player.transform.eulerAngles;
                player.transform.rotation = Quaternion.Euler(currentRotation.x, defaultLookDirection, currentRotation.z);
            }
        }
        else
        {
            if (player == null)
                Debug.LogWarning("SceneManager: Player GameObject not assigned!");
            if (spawnPoint == null)
                Debug.LogWarning("SceneManager: SpawnPoint GameObject not assigned!");
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
