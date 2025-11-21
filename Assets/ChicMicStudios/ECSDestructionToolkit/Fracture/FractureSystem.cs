using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
namespace Frimus
{
    namespace ECSDestructionToolkit
    {
        public partial class FractureSystem : SystemBase
        {
            protected override void OnCreate()
            {
                base.OnCreate();
                RequireForUpdate<FractureTag>();
            }

            protected override void OnUpdate()
            {
                var ecb = new EntityCommandBuffer(Allocator.Temp);
                try
                {
                    foreach (var (fractureTag, fractureComponent, entity) in SystemAPI.Query<FractureTag,FractureComponent>().WithEntityAccess())
                    {
                        if (!fractureTag.fractureNow)
                        {
                            if (KeyCode.None == fractureTag.KeyCode || !Input.GetKey(fractureTag.KeyCode))
                            {
                                continue;
                            }
                        }

                        if (fractureComponent.TotalChunks <= 0)
                            continue;

                        Mesh mesh = StaticFunctions.GetMeshFromEntity(EntityManager, entity);
                        if(mesh == null)
                        {
                            Debug.LogWarning("Mesh is null");
                            continue;
                        } 
                        else if(mesh.isReadable == false)
                        {
                            Debug.LogWarning("Mesh is not readable. Please enable Read/Write in the model import settings.");
                            continue;
                        }
                        Material material = StaticFunctions.GetMaterialFromEntity(EntityManager, entity);
                        if (material == null)
                        {
                            Debug.Log("Material is null");
                        }
                        FractureMesh.FractureEntity(ecb, entity, fractureComponent, mesh, material);
                        if (fractureComponent.particleSystem != Entity.Null)
                        {
                            Entity psEntity = ecb.Instantiate(fractureComponent.particleSystem);
                            ecb.SetComponent(psEntity, new LocalTransform
                            {
                                Position = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<LocalTransform>(entity).Position,
                                Rotation = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<LocalTransform>(entity).Rotation,
                                Scale = 1f
                            });
                        }
                        if (fractureComponent.destroysSelf)
                        {
                            ecb.DestroyEntity(entity);
                        }
                        else
                        {
                            ecb.RemoveComponent<FractureTag>(entity);
                        }

                    }
                }
                catch (Exception ex)
                {
                    Debug.Log("Failed to run fracture : " + ex.Message);
                }
                ecb.Playback(EntityManager);
                ecb.Dispose();
            }
        }
    }
}