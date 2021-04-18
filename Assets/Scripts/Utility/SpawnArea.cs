using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PII.Utilities
{
    public class SpawnArea : MonoBehaviour
    {
        [SerializeField] private Transform[] SpawnPoints;

        protected virtual void Awake()
        {

        }

        public Vector3 GetSpawnPoint(int index)
        {
            if (SpawnPoints.Length < 1) return transform.position;

            var i = index + 1 > SpawnPoints.Length ? SpawnPoints.Length - 1 : index;
            return SpawnPoints[i].position;
        }

        public Vector3 GetRandomSpawnPoint()
        {
            if (SpawnPoints.Length < 1) return transform.position;

            return SpawnPoints[Random.Range(0, SpawnPoints.Length - 1)].position;
        }

        private void OnDrawGizmosSelected()
        {
            if (SpawnPoints == null) return;

            Gizmos.color = Color.cyan;
            for (int i = 0; i < SpawnPoints.Length; i++)
            {
                Gizmos.DrawCube(SpawnPoints[i].position - Vector3.down * 0.5f, new Vector3(0.5f,1,0.5f));
            }
        }
    }
}