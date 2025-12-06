using UnityEngine;

public class RotateImage : MonoBehaviour
{
    // 每秒旋轉的速度（可以在 Inspector 調整）
    public float rotationSpeed = 50f;

    void Update()
    {
        // 讓物件繞著 Y 軸旋轉
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
