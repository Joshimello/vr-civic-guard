using System;
using Unity.Entities;
using UnityEngine;
namespace Frimus
{
    namespace ECSDestructionToolkit
    {
        [Serializable]
        public struct FractureComponent : IComponentData
        {
            public bool fillHole;
            public bool saveMeshes;
            public bool destroysSelf;
            public int TotalChunks;
            public FracturedTag fracturedTagData;
            public Entity prefab;
            public Entity particleSystem;
        }

        [Serializable]
        public struct FractureTag : IComponentData
        {
            [Header("Set this bool true when you want to fracture.")]
            public bool fractureNow;
            [Header("Or you can give a key.")]
            public KeyCode KeyCode;
        }

        [Serializable]
        public struct FracturedTag : IComponentData
        {
            public bool ifVelocity;
            public float maxVelocityAfterCollision;
            public float minVelocityAfterCollision;
            public bool setMeshCollider;
            public bool makeMeshColliderConvex;
        }
    }
}