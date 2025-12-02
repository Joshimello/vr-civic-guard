using UnityEngine;
using UnityEngine.UI; // 記得引用 UI 命名空間！
using System.Collections; // 協程需要這個

public class PlayerCollision : MonoBehaviour
{
    // 您的血條組件
    public RectangularSequentialHealthBar healthBar;

    // 🌟 新增：連結到您建立的血絲 Image
    public Image bloodSplatterImage;

    // 🌟 新增：可在 Inspector 中設定的多個標籤
    [Header("Damage Tags")]
    [Tooltip("可造成傷害的物體標籤列表")]
    public string[] damageTags = new string[] { "FractureCollider" };

    private bool canTakeDamage = true;
    private float damageCooldown = 5f;
    private float damageAmount = 20f;

    // 🌟 新增：血絲顯示時間和淡出速度
    private float displayDuration = 0.2f; // 血絲完全顯示的時間
    private float fadeDuration = 0.8f;    // 血絲淡出的時間


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

        // 1. 扣血
        healthBar.TakeDamage(damageAmount);

        // 2. 顯示受傷血絲效果
        StartCoroutine(ShowDamageVignette());

        // 3. 傷害冷卻
        StartCoroutine(DamageDelay());
    }

    // 傷害冷卻協程 (您原有的)
    IEnumerator DamageDelay()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }

    // 🌟 新增：控制血絲圖片顯示與淡出的協程
    IEnumerator ShowDamageVignette()
    {
        if (bloodSplatterImage == null)
        {
            Debug.LogError("Blood Splatter Image 未設定！");
            yield break;
        }

        // 立即顯示血絲 (設定為不透明)
        Color startColor = bloodSplatterImage.color;
        startColor.a = 1f; // 讓血絲立即顯示
        bloodSplatterImage.color = startColor;

        // 等待血絲完全顯示的時間
        yield return new WaitForSeconds(displayDuration);

        // 開始淡出
        float timer = 0f;
        while (timer < fadeDuration)
        {
            // 計算當前透明度：從 1 到 0 漸變
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);

            // 更新圖片透明度
            Color newColor = bloodSplatterImage.color;
            newColor.a = alpha;
            bloodSplatterImage.color = newColor;

            timer += Time.deltaTime;
            yield return null; // 等待下一幀
        }

        // 確保最終完全隱藏 (透明度設為 0)
        Color finalColor = bloodSplatterImage.color;
        finalColor.a = 0f;
        bloodSplatterImage.color = finalColor;
    }
}
