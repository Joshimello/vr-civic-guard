using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RadarSectorVisual : MonoBehaviour
{
    [Header("3D 掃描高度")]
    public float coneHeight = 3f;  // 會在 LateUpdate 裡自動更新

    [Header("扇形設定")]
    public float radius = 10f;
    [Range(1f, 360f)]
    public float angle = 120f;
    public int segments = 40;
    public float heightOffset = 0.02f; // 控制圓錐的基礎高度（避免完全貼地）

    [Header("跟隨無人機")]
    public Transform drone;   // 指向無人機
    public float groundY = 0f; // 地板高度

    [Header("扇形位置偏移設定（之後想做前方偏移可以用）")]
    public float forwardDistance = 5f;
    public float tiltAngle = 30f;

    [Header("偵測 & UI")]
    public SectorDetector detector;
    public Transform playerTarget; // 目標（例如玩家或 Cube）
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

        // 以無人機高度決定錐體高度（無人機到地板的距離）
        coneHeight = Mathf.Max(0.1f, drone.position.y - groundY);
        GenerateMesh();

        // ✅ 直接把物件放在無人機位置（最上面那層就在無人機身上）
        transform.position = drone.position;

        // 目前先讓 Y 軸方向跟無人機一致（需要再調 tilt 再改）
        transform.rotation = Quaternion.Euler(0f, drone.eulerAngles.y, 0f);

        // --- 偵測邏輯 ---
        if (detector != null && playerTarget != null)
        {
            bool found = detector.DetectTarget(playerTarget);

            if (found)
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
        int vCount = (segments + 1) * 2; // 上層 + 下層
        Vector3[] vertices = new Vector3[vCount];
        int[] triangles = new int[segments * 6]; // 每段 2 個三角形

        float halfAngle = angle * 0.5f;
        float step = angle / segments;
        float topRadius = 0.01f;    // 上口小一點，避免尖成 0
        float bottomRadius = radius;

        // 底部的 y（往下拉 coneHeight，高度再略微抬起 heightOffset）
        float bottomY = -coneHeight + heightOffset;

        // 建兩層扇形
        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = (-halfAngle + step * i) * Mathf.Deg2Rad;

            float xTop = Mathf.Sin(currentAngle) * topRadius;
            float zTop = Mathf.Cos(currentAngle) * topRadius;

            float xBottom = Mathf.Sin(currentAngle) * bottomRadius;
            float zBottom = Mathf.Cos(currentAngle) * bottomRadius;

            // 🔼 上層：local y = 0，剛好在無人機位置那一層
            vertices[i] = new Vector3(xTop, 0f, zTop);

            // 🔽 下層：往下延伸到 -coneHeight（接近地板）
            vertices[i + segments + 1] = new Vector3(xBottom, bottomY, zBottom);
        }

        // 組立側邊三角形（上層與下層之間）
        for (int i = 0; i < segments; i++)
        {
            int topA = i;
            int topB = i + 1;
            int bottomA = i + segments + 1;
            int bottomB = i + segments + 2;

            int tri = i * 6;

            triangles[tri]     = topA;
            triangles[tri + 1] = bottomA;
            triangles[tri + 2] = topB;

            triangles[tri + 3] = topB;
            triangles[tri + 4] = bottomA;
            triangles[tri + 5] = bottomB;
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
