using UnityEngine;

public class SectorDetector : MonoBehaviour
{
    [Header("偵測設定")]
    public Transform visualSector;     // 綠色扇形的 Transform
    public float radius = 10f;         // 扇形半徑
    public float angle = 120f;         // 扇形角度
    public LayerMask detectMask;

    [Header("特效設定 (新增)")]
    public Renderer sectorRenderer;    // 請把那個綠色扇形物件拖進這裡
    public Color safeColor = new Color(0, 1, 0, 0.3f); // 安全：半透明綠
    public Color alertColor = new Color(1, 0, 0, 0.5f); // 警戒：半透明紅

    // 用來判斷偵測結果的函式
    public bool DetectTarget(Transform target)
    {
        // 預設結果為 false (沒看到)
        bool isDetected = false;

        if (visualSector != null)
        {
            // 1️⃣ 使用 RadarVisual 的位置
            Vector3 center = visualSector.position;

            // 2️⃣ 使用 RadarVisual 的 forward 方向
            Vector3 forward = visualSector.forward;

            // 3️⃣ 計算玩家方向
            Vector3 dir = target.position - center;
            float dist = dir.magnitude;

            // 只有距離小於半徑，才繼續算角度
            if (dist <= radius)
            {
                dir.Normalize();

                // 4️⃣ dot 計算角度
                float dot = Vector3.Dot(forward, dir);
                float degree = Mathf.Acos(dot) * Mathf.Rad2Deg;

                // 如果角度符合，就代表偵測到了
                if (degree <= angle * 0.5f)
                {
                    isDetected = true;
                }
            }
        }

        // 5️⃣ 【關鍵修改】根據結果改變顏色
        UpdateColor(isDetected);

        return isDetected;
    }

    // 這是幫你新增的變色小工具
    void UpdateColor(bool found)
    {
        if (sectorRenderer != null)
        {
            // 根據 found 決定目標顏色
            Color targetColor = found ? alertColor : safeColor;
            
            // 因為你是用 URP，所以要用 "_BaseColor"
            sectorRenderer.material.SetColor("_BaseColor", targetColor);
        }
    }
}