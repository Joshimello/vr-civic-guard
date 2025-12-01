using UnityEngine;
using TMPro;

public class FindYouUI : MonoBehaviour
{
    public TextMeshProUGUI warningText;

    public void ShowMessage()
    {
        warningText.alpha = 1f;    // 讓文字顯示
    }

    public void HideMessage()
    {
        warningText.alpha = 0f;    // 隱藏文字
    }
}
