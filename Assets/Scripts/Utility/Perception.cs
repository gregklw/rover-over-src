using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PII.Utilities
{
    [RequireComponent(typeof(SphereCollider))]
    public class Perception : MonoBehaviour
    {
        [SerializeField] private float Range = 100;
        [SerializeField] private float FieldOfViewAngle = 180;
        [SerializeField] private Vector3 CenterOffset = Vector3.zero;
        [SerializeField] private string[] EnemyTags;
        [SerializeField] private LayerMask Mask;

        private SphereCollider coll;
        private List<Collider> collidersInRange = new List<Collider>();
        
        private List<GameObject> enemysInSight = new List<GameObject>();
        
        private Vector3 center { get { return transform.position + CenterOffset; } }
        
        public bool EnemyInSight { get { return enemysInSight.Count > 0; } }
        public GameObject[] EnemysInSight { get { return enemysInSight.ToArray(); } }
        
        public GameObject GetClosestEnemy()
        {
            if (enemysInSight.Count < 1)
                return null;

            var distance = Vector3.Distance(center, enemysInSight[0].transform.position);
            var index = 0;

            for (int i = 1; i < enemysInSight.Count; i++)
            {
                var dis = Vector3.Distance(center, enemysInSight[i].transform.position);
                if (dis < distance)
                {
                    distance = dis;
                    index = i;
                }
            }

            return enemysInSight[index];
        }
        
        private void Start()
        {
            SetCollider();
        }

        private void Update()
        {
            SetCollider();
            Clear();
            GetGameObjectsInSight();
        }

        private void SetCollider()
        {
            coll = GetComponent<SphereCollider>();
            if (coll)
            {
                coll.radius = Range;
                coll.isTrigger = true;
                coll.center = CenterOffset;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other)
                return;

            if (other.isTrigger)
                return;

            if (other.gameObject == gameObject)
                return;

            if (!collidersInRange.Contains(other))
            {
                collidersInRange.Add(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other)
                return;
            
            if (other.isTrigger)
                return;
            
            if (other.gameObject == gameObject)
                return;

            if (collidersInRange.Contains(other))
            {
                collidersInRange.Remove(other);
            }
        }

        private void GetGameObjectsInSight()
        {
            for (int i = 0; i < collidersInRange.Count; i++)
            {
                if (isInSight(collidersInRange[i]))
                {
                    if (collidersInRange[i].attachedRigidbody)
                    {
                        AddGameObjectToSights(collidersInRange[i].attachedRigidbody.gameObject);
                    }
                    else
                    {
                        AddGameObjectToSights(collidersInRange[i].gameObject);
                    }
                }
            }
        }

        private bool isInSight(Collider collider)
        {
            if (collider != null)
            {
                var direction = (collider.transform.position - center).normalized;
                var angle = Vector3.Angle(direction, transform.forward);
                
                /*if (angle < FieldOfViewAngle * 0.5f)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(center, direction, out hit, Range, Mask, QueryTriggerInteraction.Ignore))
                    {
                        return angle < FieldOfViewAngle * 0.5f;
                    }
                }*/

                return angle < FieldOfViewAngle * 0.5f;
            }

            return false;
        }

        private void AddGameObjectToSights(GameObject gameObject)
        {
            if (gameObject == this.gameObject)
                return;

            if (Utility.ArrayContains(EnemyTags, gameObject.tag) && !enemysInSight.Contains(gameObject))
            {
                enemysInSight.Add(gameObject);
            }
        }

        private void Clear()
        {
            enemysInSight.Clear();
        }

        private void OnValidate()
        {
            SetCollider();
        }
    }
}