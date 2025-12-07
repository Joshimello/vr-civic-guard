using UnityEngine;

public class SectorDetector : MonoBehaviour
{
    [Header("偵測設定")]
    public Transform visualSector;     // 扇形物件 Transform（掛 RadarSectorVisual 的那個）
    public float radius = 10f;         // 扇形半徑
    [Range(1f, 360f)]
    public float angle = 120f;         // 扇形角度（張角）
    public LayerMask detectMask;       // 目前可以先不用，留著之後做遮蔽物判斷

    [Header("顯示設定")]
    public Renderer sectorRenderer;    // 扇形 MeshRenderer
    public Color baseColor = new Color(1f, 0f, 0f, 0.4f); // 一開始就亮紅色

    [Tooltip("閃爍透明度變化幅度")]
    public float pulseAmplitude = 0.3f;

    [Tooltip("閃爍速度")]
    public float pulseSpeed = 6f;

    // 內部狀態
    bool _isAlert = false;

    void Start()
    {
        // 一開始就設成亮紅色
        ApplyColor(baseColor);
    }

    void Update()
    {
        if (sectorRenderer == null) return;

        if (_isAlert)
        {
            // 掃到人 → 做呼吸式閃爍（改 alpha）
            float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f; // 0~1
            float extra = Mathf.Lerp(0f, pulseAmplitude, t);

            Color pulsed = baseColor;
            pulsed.a = Mathf.Clamp01(baseColor.a + extra);

            ApplyColor(pulsed);
        }
        else
        {
            // 沒掃到人 → 穩定亮紅色
            ApplyColor(baseColor);
        }
    }

    /// <summary>
    /// 判斷目標是否在扇形內（只看 XZ 平面）
    /// </summary>
    public bool DetectTarget(Transform target)
    {
        bool isDetected = false;

        if (visualSector == null || target == null)
        {
            SetAlert(false);
            return false;
        }

        // —— 在 XZ 平面上做扇形判斷 ——
        Vector3 center = visualSector.position;
        Vector3 targetPos = target.position;
        center.y = 0f;
        targetPos.y = 0f;

        Vector3 dirFlat = targetPos - center;
        float distFlat = dirFlat.magnitude;

        if (distFlat <= radius && distFlat > 1e-4f)
        {
            dirFlat /= distFlat;

            Vector3 forwardFlat = visualSector.forward;
            forwardFlat.y = 0f;
            if (forwardFlat.sqrMagnitude < 1e-4f)
                forwardFlat = Vector3.forward;
            forwardFlat.Normalize();

            float dot = Mathf.Clamp(Vector3.Dot(forwardFlat, dirFlat), -1f, 1f);
            float degree = Mathf.Acos(dot) * Mathf.Rad2Deg;

            if (degree <= angle * 0.5f)
                isDetected = true;
        }

        SetAlert(isDetected);
        return isDetected;
    }

    void SetAlert(bool alert)
    {
        _isAlert = alert;
    }

    void ApplyColor(Color c)
    {
        if (sectorRenderer == null) return;

        // URP Lit 材質用 _BaseColor
        sectorRenderer.material.SetColor("_BaseColor", c);
    }
}
