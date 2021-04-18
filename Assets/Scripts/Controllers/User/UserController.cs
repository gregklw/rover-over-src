using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PII.Utilities;

namespace PII
{
    public class UserController : Controller
    {
        [SerializeField] private Anchor UserCamera;
        [SerializeField] private UserHUD HUD;

        public override void Possess(Character character)
        {
            base.Possess(character);

            character.RegisterHitCallback(PlayCameraShake);
        }

        public override void UnPossess()
        {
            base.UnPossess();
            RemoveCameraShake();
        }

        protected override void Start()
        {

        }

        protected override void Update()
        {
            base.Update();

            if (HUD)
            {
                HUD.SetUserCamera(UserCamera.Cam);
                HUD.SetUserCharacter(Body);
            }
        }

        protected override void LateUpdate()
        {
            ControlAnchor();
        }

        protected override void ControlDrone(Drone drone)
        {
            if (!drone || !GameManager.InputManager)
                return;
            
            if (GameManager.InputManager.GetAxisPressed(InputSystem.DroneAxes[5]))
            {
                drone.TogglePower();
            }

            drone.PickUpInput = GameManager.InputManager.GetAxisPressed(InputSystem.DroneAxes[6]);
            drone.BoostInput = GameManager.InputManager.GetAxisPressing(InputSystem.DroneAxes[4]);

            var forward = GameManager.InputManager.GetCombinedAxesValue(InputSystem.DroneAxes[0], InputSystem.DroneAxes[1]);
            var turn = GameManager.InputManager.GetCombinedAxesValue(InputSystem.DroneAxes[2], InputSystem.DroneAxes[3]);

            var move = UserCamera ?
               UserCamera.Cam.transform.forward * forward + UserCamera.Cam.transform.right * turn :
               drone.transform.forward * forward + drone.transform.right * turn;

            var direction = UserCamera ?
                UserCamera.Cam.transform.forward :
                move;

            drone.MoveInput = move;
            drone.LookDirection = direction;

            if (UserCamera)
            {
                drone.SetFlashLight(UserCamera.Cam.transform);
                if (!drone.Active) UserCamera.SetInput(0, 0);
            }
        }

        private void ControlAnchor()
        {
            if (!UserCamera || GameManager.GamePaused)
                return;

            if (Body)
            {
                var characterObject = Body.gameObject;
                UserCamera.target = Body.Type == CharacterType.Drone ? Body.GetComponent<Drone>().CameraPosition : characterObject.transform.position;
                UserCamera.CameraPositionOffset = Vector3.zero;
                UserCamera.MaxHorizontalAngle = 90;
                UserCamera.MinHorizontalAngle = -UserCamera.MaxHorizontalAngle;
                UserCamera.SetForward(Body.transform.forward);
            }
            else
            {
                UserCamera.target = Ship.Instance.gameObject.transform.position + Ship.Instance.CameraPivot;
                UserCamera.CameraPositionOffset = Vector3.zero;
                UserCamera.MinHorizontalAngle = UserCamera.MaxHorizontalAngle = 0;
                UserCamera.SetForward(Vector3.forward);
            }
            
            UserCamera.reset = Body;

            var y = GameManager.InputManager ? GameManager.InputManager.GetCombinedAxesValue(InputSystem.CameraAxes[0], InputSystem.CameraAxes[1]) : 0;
            var x = GameManager.InputManager ? GameManager.InputManager.GetCombinedAxesValue(InputSystem.CameraAxes[2], InputSystem.CameraAxes[3]) : 0;
            UserCamera.AddInput(x, y);
        }

        private void PlayCameraShake()
        {
            if (!UserCamera || GameManager.GamePaused) return;

            UserCamera.ShakeCamera(new Shake());
        }

        private void RemoveCameraShake()
        {
            if (!UserCamera || GameManager.GamePaused) return;

            UserCamera.StopShakingCamera();
        }
    }
}