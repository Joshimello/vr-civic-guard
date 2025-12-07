using UnityEngine;
using TMPro;

public class FindYouUI : MonoBehaviour
{
    public TextMeshProUGUI warningText;

    // 目前有幾台無人機偵測到玩家
    private int detectingCount = 0;

    void Start()
    {
        SetVisible(false);
    }

    // 讓某一台無人機「加入偵測到玩家」
    public void DroneEnterDetect()
    {
        detectingCount++;
        if (detectingCount == 1)
        {
            // 第一次從 0 變成 1，顯示 UI
            SetVisible(true);
        }
    }

    // 讓某一台無人機「不再偵測到玩家」
    public void DroneExitDetect()
    {
        detectingCount = Mathf.Max(0, detectingCount - 1);
        if (detectingCount == 0)
        {
            // 全部都沒偵測到玩家，關掉 UI
            SetVisible(false);
        }
    }

    // 如果你想在其它地方重設，可以呼叫這個
    public void ResetAll()
    {
        detectingCount = 0;
        SetVisible(false);
    }

    private void SetVisible(bool visible)
    {
        if (warningText == null) return;

        // 用 TextMeshPro 自帶的 alpha
        warningText.alpha = visible ? 1f : 0f;
    }
}
