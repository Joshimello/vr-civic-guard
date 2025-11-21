using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using UnityEngine;

namespace Frimus
{
    namespace ECSDestructionToolkit
    {
        [BurstCompile]
        public partial struct AfterFractureSystem : ISystem
        {
            public void OnUpdate(ref SystemState state)
            {
                var ecb = new EntityCommandBuffer(Allocator.TempJob);
                var entityManager = state.EntityManager;


                foreach (var (fracturedTag, entity) in SystemAPI.Query<FracturedTag>().WithEntityAccess())
                {
                    if (fracturedTag.ifVelocity)
                    {
                        float3 velocity = UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(fracturedTag.minVelocityAfterCollision, fracturedTag.maxVelocityAfterCollision);

                        if (entityManager.HasComponent<PhysicsVelocity>(entity))
                        {
                            var physicsVelocity = entityManager.GetComponentData<PhysicsVelocity>(entity);
                            physicsVelocity.Linear = velocity;
                            ecb.SetComponent(entity, physicsVelocity);
                        }
                        else
                        {
                            ecb.AddComponent(entity, new PhysicsVelocity { Linear = velocity });
                        }
                    }
                    else
                    {
                        if (entityManager.HasComponent<PhysicsVelocity>(entity))
                        {
                            ecb.RemoveComponent<PhysicsVelocity>(entity);
                        }
                    }
                    if (fracturedTag.setMeshCollider)
                    {
                        Mesh mesh = StaticFunctions.GetMeshFromEntity(entityManager, entity);

                        if (mesh == null)
                        {
                            Debug.LogWarning("Mesh is null");
                        }
                        else
                        {
                            bool hasCollider = entityManager.HasComponent<PhysicsCollider>(entity);
                            if (hasCollider)
                            {
                                var oldCollider = entityManager.GetComponentData<PhysicsCollider>(entity);

                                var newCollider = fracturedTag.makeMeshColliderConvex
                                    ? Unity.Physics.ConvexCollider.Create(
                                        StaticFunctions.GetFloat3FromVector3(mesh.vertices),
                                        ConvexHullGenerationParameters.Default,
                                        oldCollider.Value.Value.GetCollisionFilter(),
                                        Unity.Physics.Material.Default)
                                    : Unity.Physics.MeshCollider.Create(
                                        mesh,
                                        oldCollider.Value.Value.GetCollisionFilter(),
                                        Unity.Physics.Material.Default);

                                var physicsCollider = new PhysicsCollider { Value = newCollider };
                                physicsCollider.MakeUnique(entity, ecb);
                                ecb.SetComponent(entity, physicsCollider);
                            }
                            else
                            {
                                var newCollider = fracturedTag.makeMeshColliderConvex
                                   ? Unity.Physics.ConvexCollider.Create(
                                       StaticFunctions.GetFloat3FromVector3(mesh.vertices),
                                       ConvexHullGenerationParameters.Default,
                                        CollisionFilter.Default,
                                       Unity.Physics.Material.Default)
                                   : Unity.Physics.MeshCollider.Create(
                                       mesh,
                                       CollisionFilter.Default,
                                       Unity.Physics.Material.Default);

                                var physicsCollider = new PhysicsCollider { Value = newCollider };
                                physicsCollider.MakeUnique(entity, ecb);
                                ecb.SetComponent(entity, physicsCollider);
                            }
                        }

                    }

                    // Do Something you want to do.
                    ecb.RemoveComponent<FracturedTag>(entity);
                }
                ecb.Playback(entityManager);
                ecb.Dispose();
            }
        }
    }
}