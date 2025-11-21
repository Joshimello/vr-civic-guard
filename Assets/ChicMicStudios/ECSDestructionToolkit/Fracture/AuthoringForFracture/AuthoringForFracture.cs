using Unity.Entities;
using UnityEngine;
namespace Frimus
{
    namespace ECSDestructionToolkit
    {
        [RequireComponent(typeof(MeshRenderer))]
        [RequireComponent(typeof(MeshFilter))]
        public class AuthoringForFracture : MonoBehaviour
        {
            [SerializeField] private FractureTag fractureTag;
            [SerializeField] private FractureComponent fractureComponent;
            [SerializeField] private GameObject prefab;
            [SerializeField] private bool particleSystemAvailable;
            [SerializeField] private GameObject particleSystemPrefab;

            public class Baker : Baker<AuthoringForFracture>
            {
                public override void Bake(AuthoringForFracture authoring)
                {
                    var selfEntity = GetEntity(TransformUsageFlags.Dynamic);
                    Entity prefabEntity = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic);
                    AddComponent(selfEntity, new FractureComponent()
                    {
                        TotalChunks = authoring.fractureComponent.TotalChunks,
                        prefab = prefabEntity,
                        destroysSelf = authoring.fractureComponent.destroysSelf,
                        fracturedTagData = authoring.fractureComponent.fracturedTagData,
                        saveMeshes = authoring.fractureComponent.saveMeshes,
                        particleSystem = authoring.particleSystemAvailable && authoring.particleSystemPrefab != null ?
                            GetEntity(authoring.particleSystemPrefab, TransformUsageFlags.Dynamic) : Entity.Null,
                        fillHole = authoring.fractureComponent.fillHole
                    });

                    AddComponent(selfEntity, authoring.fractureTag);
                }
            }
        }
    }
}