using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FractureCollision : MonoBehaviour
{
    // 🌟 新增：連結到您建立的血絲 CanvasGroup
    public CanvasGroup bloodSplatterCanvasGroup;

    // 🌟 新增：可在 Inspector 中設定的多個標籤
    [Header("Damage Tags")]
    [Tooltip("可造成傷害的物體標籤列表")]
    public string[] damageTags = new string[] { "FractureCollider" };

    [Header("Damage Settings")]
    public float damageAmount = 20f;
    public float damageCooldown = 5f;

    private bool canTakeDamage = true;

    // 🌟 新增：血絲顯示時間和淡出速度
    [Header("Blood Splatter Settings")]
    public float displayDuration = 0.2f; // 血絲完全顯示的時間
    public float fadeDuration = 0.8f;    // 血絲淡出的時間

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Detected with: " + collision.gameObject.name);

        // 檢查物體的標籤是否在傷害標籤列表中
        if (HasDamageTag(collision.gameObject.tag))
        {
            Debug.Log($"Hit {collision.gameObject.tag}! TryTakeDamage triggered.");
            TryTakeDamage();
        }
    }

    /// <summary>
    /// 檢查給定的標籤是否在傷害標籤列表中
    /// </summary>
    /// <param name="tag">要檢查的標籤</param>
    /// <returns>如果標籤在列表中返回 true，否則返回 false</returns>
    private bool HasDamageTag(string tag)
    {
        if (damageTags == null || damageTags.Length == 0)
        {
            Debug.LogWarning("Damage tags array is null or empty!");
            return false;
        }

        for (int i = 0; i < damageTags.Length; i++)
        {
            if (damageTags[i] == tag)
            {
                return true;
            }
        }
        return false;
    }

    void TryTakeDamage()
    {
        if (!canTakeDamage) return;

        // 1. 使用新的 HealthManager 系統扣血
        if (HealthManager.Instance != null)
        {
            HealthManager.Instance.TakeDamage(damageAmount);
            Debug.Log($"Dealt {damageAmount} damage via HealthManager. Current health: {HealthManager.Instance.CurrentHealth}");
        }
        else
        {
            Debug.LogError("HealthManager not found! Make sure HealthManager is in the scene.");
        }

        // 2. 顯示受傷血絲效果
        StartCoroutine(ShowDamageVignette());

        // 3. 傷害冷卻
        StartCoroutine(DamageDelay());
    }

    // 傷害冷卻協程
    IEnumerator DamageDelay()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }

    // 🌟 控制血絲顯示與淡出的協程
    IEnumerator ShowDamageVignette()
    {
        if (bloodSplatterCanvasGroup == null)
        {
            Debug.LogWarning("Blood Splatter CanvasGroup 未設定！血絲效果將被跳過。");
            yield break;
        }

        // 立即顯示血絲 (設定為不透明)
        bloodSplatterCanvasGroup.alpha = 1f;

        // 等待血絲完全顯示的時間
        yield return new WaitForSeconds(displayDuration);

        // 開始淡出
        float timer = 0f;
        while (timer < fadeDuration)
        {
            // 計算當前透明度：從 1 到 0 漸變
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);

            // 更新 CanvasGroup 透明度
            bloodSplatterCanvasGroup.alpha = alpha;

            timer += Time.deltaTime;
            yield return null; // 等待下一幀
        }

        // 確保最終完全隱藏 (透明度設為 0)
        bloodSplatterCanvasGroup.alpha = 0f;
    }

    void OnDestroy()
    {
        // 停止所有協程以防止錯誤
        StopAllCoroutines();
    }
}
