using UnityEngine;
using System.Collections;
using PII;

namespace PII.Utilities
{
    [System.Serializable]
    public class Shake
    {
        public float Duration;
        public float Range;
        public float MaxAngle;
        public float Intensity;
        public float Noise;
        public float Damping;

        private bool shaking;
        private Vector3 positionOffset;
        private Quaternion rotationOffset;
        
        public bool Shaking { get { return shaking; } }
        public Vector3 PositionOffset { get { return positionOffset; } }
        public Quaternion RotationOffset { get { return rotationOffset; } }

        public IEnumerator StartShaking()
        {
            shaking = true;
            var targetPosition = positionOffset = Vector3.zero;
            var targetRotation = rotationOffset = Quaternion.identity;
            var angle = Range * Mathf.Deg2Rad - Mathf.PI;

            var time = Time.time;
            var timeEnd = Time.time + Duration;

            while (time < timeEnd)
            {
                time = Time.time;

                float noiseAngle = (Random.value - .5f) * Mathf.PI;
                angle += Mathf.PI + noiseAngle * Noise;

                targetPosition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * Intensity;
                targetRotation = Quaternion.Euler(new Vector3(targetPosition.x, targetPosition.y).normalized * MaxAngle * Intensity);

                var damping = Damping * Time.deltaTime;
                positionOffset = Vector3.Lerp(positionOffset, targetPosition, damping);
                rotationOffset = Quaternion.Slerp(rotationOffset, targetRotation, damping);

                yield return new WaitForEndOfFrame();
            }

            positionOffset = Vector3.zero;
            rotationOffset = Quaternion.identity;
            shaking = false;
        }

        public Shake()
        {
            Duration = 1.2f;
            Range = 0.5f;
            MaxAngle = 10;
            Intensity = 1;
            Noise = 1;
            Damping = 10;
        }

        public Shake(float duration, float range, float maxAngle)
        {
            Duration = duration;
            Range = range;
            MaxAngle = maxAngle;
            Intensity = 1;
            Noise = 1;
            Damping = 10;
        }

        public Shake(float duration, float range, float maxAngle, float intensity, float noise)
        {
            Duration = duration;
            Range = range;
            MaxAngle = maxAngle;
            Intensity = intensity;
            Noise = noise;
            Damping = 10;
        }

        public Shake(float duration, float range, float maxAngle, float intensity, float noise, float damping)
        {
            Duration = duration;
            Range = range;
            MaxAngle = maxAngle;
            Intensity = intensity;
            Noise = noise;
            Damping = damping;
        }
    }
}