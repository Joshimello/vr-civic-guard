using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RadarSectorVisual : MonoBehaviour
{
    [Header("扇形設定")]
    public float radius = 10f;      
    [Range(1f, 360f)]
    public float angle = 120f;      
    public int segments = 40;       
    public float heightOffset = 0.02f;

    [Header("跟隨無人機")]
    public Transform drone;               // 指向無人機
    public float groundY = 0f;            // 地板高度

    [Header("扇形位置偏移設定")]
    public float forwardDistance = 5f;    // 扇形中心與無人機水平距離
    public float tiltAngle = 30f;         // 想要往上偏 30°（投射到地面）
    public SectorDetector detector;
    public Transform playerTarget;   // 指向 Cube
    public FindYouUI ui;



    private Mesh mesh;

    void Awake()
    {
        mesh = new Mesh();
        mesh.name = "RadarSectorMesh";
        GetComponent<MeshFilter>().mesh = mesh;
        GenerateMesh();
    }

    void LateUpdate()
    {
        if (drone == null) return;

        // --- Step 1. 計算 30 度斜前方方向 ---
        // 把無人機 forward 往上旋轉 30 度（但最終仍然投射到地板）
        Quaternion tiltRot = Quaternion.AngleAxis(tiltAngle, drone.right);

        // 產生斜前方的方向
        Vector3 tiltedDir = tiltRot * drone.forward;

        // 計算地板上的位置：取 XZ，Y 固定為地板高度
        Vector3 pos = drone.position + tiltedDir.normalized * forwardDistance;
        pos.y = groundY;   // 貼地板

        transform.position = pos;

        // --- Step 2. 扇形方向跟著無人機 --
        transform.rotation = Quaternion.Euler(0f, drone.eulerAngles.y, 0f);
        // 偵測物件
// 單一目標偵測
        if (detector != null && playerTarget != null)
        {
            if (detector.DetectTarget(playerTarget))
            {
                Debug.Log("Find Player");
                if (ui != null) ui.ShowMessage();
            }
            else
            {
                if (ui != null) ui.HideMessage();
            }
        }



    }

    void OnValidate()
    {
        if (mesh != null) GenerateMesh();
    }

    public void GenerateMesh()
    {
        int vertexCount = segments + 2;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[segments * 3];

        vertices[0] = new Vector3(0f, heightOffset, 0f);

        float halfAngle = angle * 0.5f;
        float angleStep = angle / segments;

        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = -halfAngle + angleStep * i;
            float rad = currentAngle * Mathf.Deg2Rad;

            float x = Mathf.Sin(rad) * radius;
            float z = Mathf.Cos(rad) * radius;

            vertices[i + 1] = new Vector3(x, heightOffset, z);
        }

        for (int i = 0; i < segments; i++)
        {
            int triIndex = i * 3;
            triangles[triIndex] = 0;
            triangles[triIndex + 1] = i + 1;
            triangles[triIndex + 2] = i + 2;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    public void SetFromFOV(float newRadius, float fullAngle)
    {
        radius = newRadius;
        angle = fullAngle;
        GenerateMesh();
    }
}
