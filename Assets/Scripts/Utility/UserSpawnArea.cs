using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PII.Utilities
{
    [RequireComponent(typeof(SphereCollider))]
    public class UserSpawnArea : SpawnArea
    {
        private static UserSpawnArea instance;

        public static UserSpawnArea Instance { get { return instance; } }
        public static Vector3 SpawnPoint { get { return instance.GetRandomSpawnPoint(); } }

        protected override void Awake()
        {
            base.Awake();
            if (instance)
                DestroyImmediate(this);
            else
                instance = this;
        }
    }
}
