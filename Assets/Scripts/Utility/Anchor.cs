using UnityEngine;
using System.Collections;
using PII;

namespace PII.Utilities
{
    public class Anchor : MonoBehaviour
    {
        public float MoveSpeed = 400f;
        public float MaxVerticalAngle = 30f;
        public float MinVerticalAngle = -60f;
        public float MaxHorizontalAngle = 180f;
        public float MinHorizontalAngle = -180f;
        public float Sensitivity = 0.3f;
        public float Damping = 10f;

        private float X;
        private float Y;
        private Vector3 smoothPivotOffset;
        private Vector3 smoothCamOffset;
        private Vector3 forward = Vector3.forward;
        private Shake currentShake;

        public Camera Cam { get; private set; }
        public Vector3 target { get; set; }
        public Vector3 CameraPositionOffset { get; set; }
        public bool reset { get; set; }

        private void Start()
        {
            Cam = GetComponentInChildren<Camera>();

            Cam.transform.localPosition = CameraPositionOffset;
        }

        private void LateUpdate()
        {
            ClampInputs();

            var aimRotation = Quaternion.LookRotation(forward) * Quaternion.Euler(-Y, X, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, aimRotation, Damping * Time.deltaTime);

            smoothPivotOffset = Vector3.Lerp(smoothPivotOffset, target, Damping * Time.deltaTime);
            transform.position = target;

            Cam.transform.SetParent(transform);
            Cam.transform.localPosition = CameraPositionOffset;
            Cam.transform.localRotation = Quaternion.identity;

            if (currentShake != null)
            {
                Cam.transform.localPosition += currentShake.PositionOffset;
                Cam.transform.localRotation *= currentShake.RotationOffset;

                if (!currentShake.Shaking) currentShake = null;
            }

            ResetInputs();
        }

        public void SetForward(Vector3 forward)
        {
            this.forward = forward;
        }

        public void AddInput(float x, float y)
        {
            X += x * MoveSpeed * Time.deltaTime;
            Y += y * MoveSpeed * Time.deltaTime;
        }

        public void SetInput(float x, float y)
        {
            X = x * MoveSpeed;
            Y = y * MoveSpeed;
        }

        public void ShakeCamera(Shake shake)
        {
            if (currentShake != null) StopCoroutine(currentShake.StartShaking());

            currentShake = shake;

            if (currentShake != null) StartCoroutine(currentShake.StartShaking());
        }

        public void StopShakingCamera()
        {
            currentShake = null;
        }

        private void ClampInputs()
        {
            if (MinHorizontalAngle != 0 && MaxHorizontalAngle != 0)
                X = Mathf.Clamp(X, MinHorizontalAngle, MaxHorizontalAngle);

            Y = Mathf.Clamp(Y, MinVerticalAngle, MaxVerticalAngle);
        }

        private void ResetInputs()
        {
            if (!reset) return;

            X = Mathf.Lerp(X, 0, Damping * Time.deltaTime);
            Y = Mathf.Lerp(Y, 0, Damping * Time.deltaTime);
        }
    }
}