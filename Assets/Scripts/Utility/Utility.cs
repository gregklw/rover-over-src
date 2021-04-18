using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using System.Collections.Generic;
using PII;

namespace PII.Utilities
{
	public static class Utility
	{
		#region Array Functions
		public static T[] AddToArray<T> (T[] array, T item) 
		{
			List<T> list = new List<T>();
			list.AddRange(array);
			list.Add(item);
			return list.ToArray();
		}

		public static T[] AddRangeToArray<T> (T[] array, T[] items)
		{
			List<T> list = new List<T>();
			list.AddRange(array);
			list.AddRange(items);
			return list.ToArray();
		}

		public static T[] InsertToArray<T> (T[] array, T item, int index) 
		{
			List<T> list = new List<T>();
			list.AddRange(array);
			list.Insert(index, item);
			return list.ToArray();
		}

		public static T[] RemoveFromArray<T> (T[] array, T item)
		{
			List<T> list = new List<T>();
			list.AddRange(array);
			list.Remove(item);
			return list.ToArray();
		}

		public static T[] RemoveAtFromArray<T> (T[] array, int index)
		{
			List<T> list = new List<T>();
			list.AddRange(array);
			list.RemoveAt(index);
			return list.ToArray();
		}

		public static bool ArrayContains <T> (T[] array, T item)
		{
			List<T> list = new List<T>();
			list.AddRange(array);
			return list.Contains(item);
		}
        
		public static int IndexFromArray<T> (T[] array, T item)
		{
			List<T> list = new List<T>();
			list.AddRange(array);
			return list.IndexOf(item);
		}
        #endregion

        public static Vector3 RandomWorldPoint(Vector3 orgin, float radius)
        {
            return orgin + (Random.insideUnitSphere * radius);
        }

        public static Vector3 RandomWorldDirection(float maxAngle)
        {
            var angle = Random.Range(-maxAngle, maxAngle);
            return (Quaternion.Euler(angle,angle,angle) * Vector3.one).normalized;
        }

        public static Vector3 SamplePointOnNavMesh(Vector3 point, float maxDistance, int areaMask = NavMesh.AllAreas)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(point, out hit, maxDistance, areaMask))
            {
                return hit.position;
            }

            return point + Vector3.up * maxDistance;
        }

        public static Vector3 RandomWorldPointOnNavMesh(Vector3 orgin, float radius, int areaMask = NavMesh.AllAreas)
        {
            return SamplePointOnNavMesh(RandomWorldPoint(orgin, radius), radius, areaMask);
        }

        public static Vector3[] GetCirclePathOnNavMesh(Vector3 midPoint, float radius = 100, int length = 10, int areaMask = NavMesh.AllAreas)
        {
            if (length <= 1)
                return null;

            var mid = SamplePointOnNavMesh(midPoint, radius, areaMask);
            var points = new Vector3[length];

            for (int i = 0; i < length; i++)
            {
                var dir = Quaternion.Euler(0, (360 / length) * i, 0) * Vector3.forward;
                var pos = mid + dir * radius;
                points[i] = SamplePointOnNavMesh(pos, radius, areaMask);
            }

            return points;
        }

        public static Vector3[] GetLinePathOnNavMesh(Vector3 startPoint, Vector3 direction, float distance = 100, int length = 10, int areaMask = NavMesh.AllAreas)
        {
            if (length <= 1)
                return null;

            var start = SamplePointOnNavMesh(startPoint, distance, areaMask);
            var points = new Vector3[length];

            points[0] = start;
            for (int i = 1; i < points.Length; i++)
            {
                var dis = distance / length;
                var pos = start + direction * (dis * i);
                points[i] = SamplePointOnNavMesh(pos, dis, areaMask);
            }

            return points;
        }

        public static Vector3[] GetPathOnNavMesh(Vector3 startPoint, Vector3 stopPoint, int length, float sampleRadius, int areaMask = NavMesh.AllAreas)
        {
            if (length <= 1)
                return null;

            var start = SamplePointOnNavMesh(startPoint, sampleRadius, areaMask);
            var stop = SamplePointOnNavMesh(stopPoint, sampleRadius, areaMask);
            var distance = Vector3.Distance(start, stop) / (length - 2);
            var path = new Vector3[length];

            var lastPos = path[0] = start;
            for (int i = 1; i < length - 2; i++)
            {
                var direction = (stop - lastPos).normalized;
                var pos = lastPos + direction * distance;
                lastPos = path[i] = SamplePointOnNavMesh(pos, distance, areaMask);
            }
            path[length - 1] = stop;
            
            return path;
        }

        public static Vector3[] GetPathOnNavMesh(Vector3 startPoint, Vector3 stopPoint, int areaMask = NavMesh.AllAreas)
        {
            var path = new NavMeshPath();
            if (NavMesh.CalculatePath(startPoint, stopPoint, areaMask, path))
            {
                return path.corners;
            }

            return new Vector3[0];
        }
        
        public static Vector3 GetPathDirectionNavMesh(Vector3 position, Vector3 stopPoint, int areaMask = NavMesh.AllAreas)
        {
            var points = GetPathOnNavMesh(position, stopPoint, areaMask);

            if (points != null)
            {
                if (points.Length > 1)
                {
                    var direction = points[1] - position;
                    if (direction.magnitude > 1) direction.Normalize();
                    return direction;
                }
            }

            return Vector3.zero;
        }

        public static Vector3 GetPathDirectionNavMesh(Vector3 position, Vector3 stopPoint, int quality, float sampleRadius)
        {
            var points = GetPathOnNavMesh(position, stopPoint, quality, sampleRadius);

            if (points != null)
            {
                if (points.Length > 1)
                {
                    var direction = points[1] - position;
                    if (direction.magnitude > 1) direction.Normalize();
                    return direction;
                }
            }

            return Vector3.zero;
        }

        public static float Interpolate(float y0, float y1, float x0, float x1, float x)
        {
            return y0 + (x - x0) * ((y1 - y0) / (x1 - x0));
        }

        public static AudioSource SetUpAudioSource(GameObject gameObject, AudioMixerGroup mixer, AudioClip clip)
        {
            // create the new audio source component on the game object and set up its properties
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = 0;
            source.loop = true;
            source.outputAudioMixerGroup = mixer;

            if (clip)
            {
                source.time = Random.Range(0f, clip.length);
                source.Play();
            }

            source.minDistance = 5;
            source.maxDistance = 500;
            source.dopplerLevel = 1;

            return source;
        }
    }
}