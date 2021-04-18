using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PII;

namespace PII.Utilities
{
    public class Water : MonoBehaviour
    {
        private static List<Water> Waters = new List<Water>();

        [SerializeField] private Vector3 Center = Vector3.zero;
        [SerializeField] private Vector3 Size = Vector3.one;
        private Bounds bounds { get { return new Bounds(transform.position + Center, Size); } }

        private void Awake()
        {
            if (!Waters.Contains(this))
                Waters.Add(this);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        public static bool InWater(Vector3 position)
        {
            if (Waters != null)
            {
                for (int i = 0; i < Waters.Count; i++)
                {
                    if (Waters[i].bounds.Contains(position))
                        return true;
                }
            }

            return false;
        }
    }
}