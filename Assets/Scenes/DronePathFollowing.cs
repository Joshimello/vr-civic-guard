using UnityEngine;

public class DronePathFollowing : MonoBehaviour
{
    public Transform[] waypoints;  // 路徑點
    public float speed = 5f;       // 無人機移動速度
    public float rotationSpeed = 2f;  // 旋轉速度
    private int currentWaypointIndex = 0;

    private Rigidbody rb;  // Rigidbody，用於物理移動

    void Start()
    {
        // 確保無人機有 Rigidbody
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 設置無人機移動到當前路徑點
        MoveToNextWaypoint();
    }

    void MoveToNextWaypoint()
    {
        if (waypoints.Length == 0) return;  // 如果沒有設置路徑點，則返回

        // 無人機當前應該前往的目標位置
        Vector3 targetPosition = waypoints[currentWaypointIndex].position;

        // 移動無人機
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // 旋轉無人機以朝向目標位置
        Vector3 direction = targetPosition - transform.position;
        if (direction.sqrMagnitude > 0.1f) // 防止小誤差
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 當無人機到達當前路徑點，切換到下一個
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }
}
