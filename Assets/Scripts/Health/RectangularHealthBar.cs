using UnityEngine;
using UnityEngine.UI;

public class RectangularSequentialHealthBar : MonoBehaviour
{
    [Header("Blood Bar Images")]
    public Image topBar;
    public Image rightBar;
    public Image bottomBar;
    public Image leftBar;

    [Header("HP Settings")]
    public float maxHP = 100f;
    public float currentHP = 100f;

    [Header("Test Mode")]
    public bool testMode = true;     // 開啟測試血量下降
    public float testDamagePerSecond = 5f;

    void Update()
    {
        if (testMode)
        {
            currentHP -= testDamagePerSecond * Time.deltaTime;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        }

        UpdateHealthBars();
    }

    void UpdateHealthBars()
    {
        float hpPercent = currentHP / maxHP;

        // 血條四段（每段 25%）
        float segment = 1f / 4f;

        // ------ Top Bar ------
        if (hpPercent >= 0.75f)
        {
            float fill = (hpPercent - 0.75f) / segment;
            topBar.fillAmount = Mathf.Clamp01(fill);
        }
        else
        {
            topBar.fillAmount = 0;
        }

        // ------ Right Bar ------
        if (hpPercent < 0.75f && hpPercent >= 0.50f)
        {
            float fill = (hpPercent - 0.50f) / segment;
            rightBar.fillAmount = Mathf.Clamp01(fill);
        }
        else if (hpPercent >= 0.75f)
        {
            rightBar.fillAmount = 1;
        }
        else
        {
            rightBar.fillAmount = 0;
        }

        // ------ Bottom Bar ------
        if (hpPercent < 0.50f && hpPercent >= 0.25f)
        {
            float fill = (hpPercent - 0.25f) / segment;
            bottomBar.fillAmount = Mathf.Clamp01(fill);
        }
        else if (hpPercent >= 0.50f)
        {
            bottomBar.fillAmount = 1;
        }
        else
        {
            bottomBar.fillAmount = 0;
        }

        // ------ Left Bar ------
        if (hpPercent < 0.25f)
        {
            float fill = (hpPercent / segment);
            leftBar.fillAmount = Mathf.Clamp01(fill);
        }
        else
        {
            leftBar.fillAmount = 1;
        }

        UpdateBarColor(hpPercent);
    }

    void UpdateBarColor(float hpPercent)
    {
        Color c = Color.green;

        if (hpPercent > 0.66f)
            c = Color.green;
        else if (hpPercent > 0.33f)
            c = Color.yellow;
        else
            c = Color.red;

        topBar.color = c;
        rightBar.color = c;
        bottomBar.color = c;
        leftBar.color = c;
    }

    // 外部可呼叫此方法扣血
    public void TakeDamage(float dmg)
    {
        currentHP -= dmg;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
    }
}
