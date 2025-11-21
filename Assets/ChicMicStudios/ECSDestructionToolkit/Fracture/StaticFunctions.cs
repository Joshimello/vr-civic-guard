using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
namespace Frimus
{
    namespace ECSDestructionToolkit
    {
        public static class StaticFunctions
        {
            public static NativeArray<float3> GetFloat3FromVector3(Vector3[] vector3s)
            {
                NativeArray<float3> float3s = new NativeArray<float3>(vector3s.Length, Allocator.Temp);
                for (int i = 0; i < vector3s.Length; i++)
                {
                    float3s[i] = new float3(vector3s[i].x, vector3s[i].y, vector3s[i].z);
                }
                return float3s;
            }

            public static Mesh GetMeshFromEntity(EntityManager entityManager, Entity entity)
            {
                if (!entityManager.HasComponent<RenderMeshArray>(entity))
                {
                    Debug.LogWarning("[GetMeshFromEntity] Entity does NOT have RenderMeshArray component.");
                    return null;
                }

                if (!entityManager.HasComponent<MaterialMeshInfo>(entity))
                {
                    Debug.LogWarning("[GetMeshFromEntity] Entity does NOT have MaterialMeshInfo component.");
                    return null;
                }
                var renderMeshArray = entityManager.GetSharedComponentManaged<RenderMeshArray>(entity);

                var materialMeshInfo = entityManager.GetComponentData<MaterialMeshInfo>(entity);

                int indexOfMesh = materialMeshInfo.Mesh;

                indexOfMesh = (-indexOfMesh) - 1;
                if (indexOfMesh < renderMeshArray.MeshReferences.Length && indexOfMesh >= 0)
                {
                    return renderMeshArray.MeshReferences[indexOfMesh];
                }

                Debug.LogError($"[GetMeshFromEntity] Fallback index {indexOfMesh} still out of bounds. Returning null.");
                return null;
            }

            public static Material GetMaterialFromEntity(EntityManager entityManager, Entity entity) 
            {
                if (!entityManager.HasComponent<RenderMeshArray>(entity))
                {
                    Debug.LogWarning("[GetMeshFromEntity] Entity does NOT have RenderMeshArray component.");
                    return null;
                }

                if (!entityManager.HasComponent<MaterialMeshInfo>(entity))
                {
                    Debug.LogWarning("[GetMeshFromEntity] Entity does NOT have MaterialMeshInfo component.");
                    return null;
                }
                var renderMeshArray = entityManager.GetSharedComponentManaged<RenderMeshArray>(entity);

                var materialMeshInfo = entityManager.GetComponentData<MaterialMeshInfo>(entity);

                int indexOfMaterial = materialMeshInfo.Material;

                indexOfMaterial = (-indexOfMaterial) - 1;
                if (indexOfMaterial < renderMeshArray.MaterialReferences.Length && indexOfMaterial >= 0)
                {
                    return renderMeshArray.MaterialReferences[indexOfMaterial];
                }

                Debug.LogError($"[GetMeshFromEntity] Fallback index {indexOfMaterial} still out of bounds of {renderMeshArray.MaterialReferences}. Returning null.");
                return null;
            }
        }
    }
}