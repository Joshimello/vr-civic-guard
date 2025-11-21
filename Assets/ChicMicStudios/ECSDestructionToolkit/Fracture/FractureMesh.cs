using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Frimus
{
    namespace ECSDestructionToolkit
    {
        public static class FractureMesh
        {
            public static void FractureEntity(EntityCommandBuffer ecb, Entity originalEntity, FractureComponent fractureComp, Mesh sourceMesh, Material material)
            {
                NvBlastExtUnity.setSeed(UnityEngine.Random.Range(0, 100));

                (List<Mesh> meshes, List<Vector3> chunkOffsets) = FractureMeshes(fractureComp.TotalChunks, fractureComp.fillHole, fractureComp.saveMeshes, new NvMesh(
                    sourceMesh.vertices,
                    sourceMesh.normals,
                    sourceMesh.uv,
                    sourceMesh.vertexCount,
                    sourceMesh.GetIndices(0),
                    (int)sourceMesh.GetIndexCount(0)
                ));

                CreateChunks(ecb, originalEntity, meshes, material, chunkOffsets, fractureComp, fractureComp.fracturedTagData);
            }

            private static (List<Mesh>, List<Vector3>) FractureMeshes(int totalChunks, bool fillHole, bool save, NvMesh nvMesh)
            {
                var fractureTool = new NvFractureTool();
                fractureTool.setRemoveIslands(false);
                fractureTool.setSourceMesh(nvMesh);

                var sites = new NvVoronoiSitesGenerator(nvMesh);
                sites.uniformlyGenerateSitesInMesh(totalChunks);
                fractureTool.voronoiFracturing(0, sites);
                fractureTool.finalizeFracturing();

                var meshCount = fractureTool.getChunkCount();
                var meshes = new List<Mesh>(meshCount);
                var chunkOffsets = new List<Vector3>(meshCount);

                for (var i = 1; i < meshCount; i++)
                {
                    (Mesh chunkMesh, Vector3 offset) = ExtractChunkMesh(fractureTool, fillHole, save, i);
                    meshes.Add(chunkMesh);
                    chunkOffsets.Add(offset);
                }

                return (meshes, chunkOffsets);
            }

            private static (Mesh, Vector3) ExtractChunkMesh(NvFractureTool fractureTool, bool fillHole, bool save, int index)
            {
                var outside = fractureTool.getChunkMesh(index, false);
                var inside = fractureTool.getChunkMesh(index, true);
                var chunkMesh = outside.toUnityMesh();

                Vector3 pivotOffset = chunkMesh.bounds.center;

                CenterMeshPivot(ref chunkMesh, pivotOffset);

                chunkMesh.subMeshCount = 2;
                chunkMesh.SetIndices(inside.getIndexes(), MeshTopology.Triangles, 1);
                if (fillHole)
                {
                    CapMeshHolesWithMinimalGeometry(ref chunkMesh);
                }

#if UNITY_EDITOR
                if (save)
                {
                    SaveMeshAsset(chunkMesh, $"GeneratedMesh_Chunk_{index}");
                }
#endif

                return (chunkMesh, pivotOffset);
            }

            private static void CenterMeshPivot(ref Mesh mesh, Vector3 pivotOffset)
            {
                Vector3[] vertices = mesh.vertices;

                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] -= pivotOffset;
                }

                mesh.vertices = vertices;
                mesh.RecalculateBounds();
            }

            private static void CapMeshHolesWithMinimalGeometry(ref Mesh mesh)
            {
                var vertices = new List<Vector3>(mesh.vertices);
                var triangles = new List<int>(mesh.triangles);

                var edgeCount = new Dictionary<(int, int), int>();
                for (int i = 0; i < triangles.Count; i += 3)
                {
                    int v1 = triangles[i];
                    int v2 = triangles[i + 1];
                    int v3 = triangles[i + 2];
                    AddEdge(v1, v2, edgeCount);
                    AddEdge(v2, v3, edgeCount);
                    AddEdge(v3, v1, edgeCount);
                }
                var boundaryEdges = new List<(int, int)>();
                foreach (var kvp in edgeCount)
                {
                    if (kvp.Value == 1) boundaryEdges.Add(kvp.Key);
                }
                if (boundaryEdges.Count == 0) return;

                var loops = FindEdgeLoops(boundaryEdges);

                foreach (var loop in loops)
                {
                    var newTriangles = TriangulateEarClipping(loop, vertices);
                    triangles.AddRange(newTriangles);
                }

                mesh.Clear();
                mesh.vertices = vertices.ToArray();
                mesh.triangles = triangles.ToArray();
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
            }
            private static List<int> TriangulateEarClipping(List<int> loop, List<Vector3> vertices)
            {
                var newTriangles = new List<int>();
                var remainingIndices = new List<int>(loop);
                if (remainingIndices.Count == 3)
                {
                    newTriangles.AddRange(remainingIndices);
                    return newTriangles;
                }
                Vector3 normal = Vector3.zero;
                for (int i = 0; i < remainingIndices.Count; i++)
                {
                    var p1 = vertices[remainingIndices[i]];
                    var p2 = vertices[remainingIndices[(i + 1) % remainingIndices.Count]];
                    normal.x += (p1.y - p2.y) * (p1.z + p2.z);
                    normal.y += (p1.z - p2.z) * (p1.x + p2.x);
                    normal.z += (p1.x - p2.x) * (p1.y + p2.y);
                }
                normal.Normalize();
                int axis1 = 0, axis2 = 1;
                if (Mathf.Abs(normal.y) > Mathf.Abs(normal.x) && Mathf.Abs(normal.y) > Mathf.Abs(normal.z)) { axis1 = 0; axis2 = 2; }
                else if (Mathf.Abs(normal.z) > Mathf.Abs(normal.x) && Mathf.Abs(normal.z) > Mathf.Abs(normal.y)) { axis1 = 0; axis2 = 1; }
                int maxIterations = remainingIndices.Count * remainingIndices.Count;
                int currentIteration = 0;
                while (remainingIndices.Count > 2 && currentIteration < maxIterations)
                {
                    bool earFound = false;
                    for (int i = 0; i < remainingIndices.Count; i++)
                    {
                        int prev = remainingIndices[(i == 0) ? remainingIndices.Count - 1 : i - 1];
                        int curr = remainingIndices[i];
                        int next = remainingIndices[(i + 1) % remainingIndices.Count];
                        Vector2 p_prev = Get2DPoint(vertices[prev], axis1, axis2);
                        Vector2 p_curr = Get2DPoint(vertices[curr], axis1, axis2);
                        Vector2 p_next = Get2DPoint(vertices[next], axis1, axis2);
                        float crossProduct = (p_curr.x - p_prev.x) * (p_next.y - p_curr.y) - (p_curr.y - p_prev.y) * (p_next.x - p_curr.x);
                        if (crossProduct < 0)
                        {
                            continue;
                        }
                        bool isEar = true;
                        for (int j = 0; j < remainingIndices.Count; j++)
                        {
                            int testIndex = remainingIndices[j];
                            if (testIndex == prev || testIndex == curr || testIndex == next) continue;
                            if (IsPointInTriangle(Get2DPoint(vertices[testIndex], axis1, axis2), p_prev, p_curr, p_next))
                            {
                                isEar = false;
                                break;
                            }
                        }
                        if (isEar)
                        {
                            newTriangles.Add(prev);
                            newTriangles.Add(curr);
                            newTriangles.Add(next);
                            remainingIndices.RemoveAt(i);
                            earFound = true;
                            break;
                        }
                    }
                    if (!earFound) { currentIteration++; } else { currentIteration = 0; }
                }
                return newTriangles;
            }
            private static Vector2 Get2DPoint(Vector3 v3, int axis1, int axis2)
            {
                return new Vector2(v3[axis1], v3[axis2]);
            }
            private static bool IsPointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
            {
                float s = a.y * c.x - a.x * c.y + (c.y - a.y) * p.x + (a.x - c.x) * p.y;
                float t = a.x * b.y - a.y * b.x + (a.y - b.y) * p.x + (b.x - a.x) * p.y;
                if ((s < 0) != (t < 0) && s != 0 && t != 0) return false;
                float A = -b.y * c.x + a.y * (c.x - b.x) + a.x * (b.y - c.y) + b.x * c.y;
                return A < 0 ? (s <= 0 && s + t >= A) : (s >= 0 && s + t <= A);
            }
            private static void AddEdge(int a, int b, Dictionary<(int, int), int> edgeCount)
            {
                var key = a < b ? (a, b) : (b, a);
                if (edgeCount.ContainsKey(key)) edgeCount[key]++;
                else edgeCount[key] = 1;
            }
            private static List<List<int>> FindEdgeLoops(List<(int, int)> edges)
            {
                var loops = new List<List<int>>();
                var edgeDict = new Dictionary<int, List<int>>();
                foreach (var (a, b) in edges)
                {
                    if (!edgeDict.ContainsKey(a)) edgeDict[a] = new List<int>();
                    if (!edgeDict.ContainsKey(b)) edgeDict[b] = new List<int>();
                    edgeDict[a].Add(b);
                    edgeDict[b].Add(a);
                }
                var visited = new HashSet<int>();
                foreach (var (start, _) in edges)
                {
                    if (visited.Contains(start)) continue;
                    var loop = new List<int>();
                    int current = start, prev = -1;
                    do
                    {
                        loop.Add(current);
                        visited.Add(current);
                        int next = -1;
                        foreach (var n in edgeDict[current])
                        {
                            if (n != prev)
                            {
                                next = n;
                                break;
                            }
                        }
                        prev = current;
                        current = next;
                    } while (current != start && current != -1 && !visited.Contains(current));
                    if (loop.Count > 2)
                        loops.Add(loop);
                }
                return loops;
            }

            private static void CreateChunks(EntityCommandBuffer ecb, Entity originalEntity, List<Mesh> meshes, Material material, List<Vector3> chunkOffsets, FractureComponent fractureComp, FracturedTag fracturedTag)
            {
                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                bool hasPostTransform = entityManager.HasComponent<PostTransformMatrix>(originalEntity);
                var originalPostTransform = hasPostTransform ? entityManager.GetComponentData<PostTransformMatrix>(originalEntity) : new PostTransformMatrix();
                var originalTransform = entityManager.GetComponentData<LocalTransform>(originalEntity);
                var originalLocalToWorld = entityManager.GetComponentData<LocalToWorld>(originalEntity);

                var materialArray = new[] { material };
                var renderMeshArray = new RenderMeshArray(materialArray, meshes.ToArray());

                for (int i = 0; i < meshes.Count; i++)
                {
                    var entity = ecb.Instantiate(fractureComp.prefab);
                    float3 localChunkPosition = chunkOffsets[i];
                    float3 worldChunkPosition = math.transform(originalLocalToWorld.Value, localChunkPosition);
                    ecb.SetComponent(entity, new LocalTransform
                    {
                        Position = worldChunkPosition,
                        Rotation = originalTransform.Rotation,
                        Scale = originalTransform.Scale
                    });
                    ecb.SetComponent(entity, new LocalToWorld
                    {
                        Value = float4x4.TRS(worldChunkPosition, originalTransform.Rotation, originalTransform.Scale)
                    });

                    if (hasPostTransform)
                    {
                        ecb.AddComponent(entity, new PostTransformMatrix
                        {
                            Value = originalPostTransform.Value
                        });
                    }

                    ecb.AddSharedComponentManaged(entity, renderMeshArray);
                    ecb.AddComponent(entity, MaterialMeshInfo.FromRenderMeshArrayIndices(0, i));

                    var bounds = meshes[i].bounds;
                    ecb.AddComponent(entity, new RenderBounds
                    {
                        Value = new AABB
                        {
                            Center = bounds.center,
                            Extents = bounds.extents
                        }
                    });
                    ecb.AddComponent<WorldRenderBounds>(entity);
                    ecb.AddComponent(entity, fracturedTag);
                }
            }

#if UNITY_EDITOR
            public static void SaveMeshAsset(Mesh mesh, string name = "GeneratedMesh")
            {
                string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                string assetFolderPath = "Assets/Fracture/Fracture/SavedMeshes";
                string fileSystemPath = Path.Combine(Application.dataPath, "Fracture/Fracture/SavedMeshes");
                string assetPath = $"{assetFolderPath}/{name}_{timestamp}.asset";

                if (!Directory.Exists(fileSystemPath))
                {
                    Directory.CreateDirectory(fileSystemPath);
                    UnityEditor.AssetDatabase.Refresh();
                }

                AssetDatabase.CreateAsset(mesh, assetPath);
                AssetDatabase.SaveAssets();
            }
#endif
        }
    }
}