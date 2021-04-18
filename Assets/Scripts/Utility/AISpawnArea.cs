using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PII.Utilities
{
    [RequireComponent(typeof(SphereCollider))]
    public class AISpawnArea : SpawnArea
    {
        [SerializeField] private int AIInArea = 3;
        [SerializeField] private string EnemyTag = "Drone";
        [SerializeField] private float TimeBetweenSpawns = 2f;

        private bool hasSpawned = false;
        private bool spawning = false;

        private void Update()
        {
            if (spawning && !GameManager.GameStarted) StopCoroutine(SpawnAI());
        }

        private void OnTriggerEnter(Collider other)
        {
            if (GameManager.GameStarted)
            {
                var isEnemy = other.tag == EnemyTag || other.attachedRigidbody.tag == EnemyTag;
                if (isEnemy && !spawning)
                {
                    StartCoroutine(SpawnAI());
                }
            }
        }

        private IEnumerator SpawnAI()
        {
            spawning = true;
            if (!hasSpawned)
            {
                for (int i = 0; i < AIInArea; i++)
                {
                    var ai = GameManager.SpawnAI();
                    if (ai)
                    {
                        var position = GetRandomSpawnPoint();
                        var body = GameManager.SpawnHuman(position);
                        if (body)
                        {
                            ai.Possess(body);
                            ai.SetTarget(position);
                        }
                    }
                    yield return new WaitForSeconds(TimeBetweenSpawns);
                }
                
                hasSpawned = true;
            }
            spawning = false;
        }
    }
}
