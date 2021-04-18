using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using PII.Utilities;

namespace PII
{
    public class Drone : Character
    {
        [SerializeField] private GameObject OnlineModel;
        [SerializeField] private GameObject OfflineModel;
        [SerializeField] private GameObject DamagedModel;
        [SerializeField] private float MaxBatteryCharge = 200f;
        [SerializeField] private float PowerDownTimeLimit = 10f;
        [SerializeField] [Range(0f, 1f)] private float ChargeEfficiency = 0.7f;
        [SerializeField] [Range(0f, 1f)] private float DischargeRate = (10f/200f);
        [SerializeField] [Range(0f, 1f)] private float FlashLightDischargeRate = 0.0035f;
        [SerializeField] private Light FlashLight;
        [SerializeField] private Vector3 CenterOfMassOffset = Vector3.zero;
        [SerializeField] private Transform Camera;
        [SerializeField] private WheelPair[] Wheels;
        [SerializeField] private float Topspeed = 60;
        [SerializeField] private float MaximumSteerAngle = 20;
        [SerializeField] [Range(0, 1)] private float SteerHelper = 1;
        [SerializeField] [Range(0, 1)] private float TractionControl = 1;
        [SerializeField] private float FullTorqueOverAllWheels = 1000;
        [SerializeField] private float ReverseTorque = 400;
        [SerializeField] private float MaxHandbrakeTorque = 700;
        [SerializeField] private float Downforce = 100f;
        [SerializeField] private float SlipLimit = 10;
        [SerializeField] private float BrakeTorque = 100;
        [SerializeField] private float AntiRollForce = 5000;
        [SerializeField] private float BoostForce = 1200;
        [SerializeField] private float BoostTime = 5;
        [SerializeField] private float BoostCoolDownTime = 20;
        [SerializeField] private AudioClip PowerUpClip;
        [SerializeField] private AudioClip PowerDownClip;
        [SerializeField] private AudioClip AccelerateClip;
        [SerializeField] private AudioClip BoostClip;
        [SerializeField] private AudioClip[] HitClips;
        [SerializeField] private AudioClip NoSignalClip;

        private bool active = true;
        private bool flash = true;
        private float batteryCharge;
        private float timer;
        private bool startedSound = false;
        private float oldRotation;
        private float currentTorque;
        private bool boosting;
        private float boostCoolDownTimer;

        private AudioSource powerSource;
        private AudioSource accelSource;
        private AudioSource hitSource;
        private AudioSource signalSource;

        public bool Active { get { return active; } }
        public bool isFlashOn { get { return flash; } }
        public float Charge { get { return batteryCharge; } }
        public float ChargePercentage { get { return batteryCharge / MaxBatteryCharge; } }
        public bool isBatteryDead { get { return ChargePercentage <= 0; } }
        public float DistanceFromShip { get { return Vector3.Distance(Ship.Instance.transform.position, transform.position); } }
        public float DistanceFromShipPercentage { get { return DistanceFromShip / Ship.MaxDroneDistance; } }
        public int PowerDownTimer { get { return (int)timer; } }
        public float Speed { get { return rigidBody.velocity.magnitude * 3.6f; } }
        public bool Boosting { get { return boosting; } }
        public float BoostCoolDownPercentage { get { return Utility.Interpolate(0, 1, BoostCoolDownTime, 0, boostCoolDownTimer); } }
        public Vector3 CameraPosition { get { return Camera ? Camera.position : transform.position; } }
        public float SteerAngle { get; private set; }
        public float Accel { get; private set; }
        public float Brake { get; private set; }
        public float HandBrake { get; private set; }

        public bool BoostInput { get; set; }
        
        public void TogglePower()
        {
            active = Alive ? !active : false;

            if (active) PlayPowerUpSound();
            else PlayPowerDownSound();
            
            if (!active) flash = false;
        }

        public void SetFlashLightActive(bool value)
        {
            flash = active ? value : false;
        }

        public void SetFlashLight(Transform parent)
        {
            if (FlashLight)
            {
                FlashLight.transform.parent = transform;

                var value = Alive && parent;
                var position = value ? parent.position : FlashLight.transform.position;
                var rotation = value ? parent.rotation : FlashLight.transform.rotation;
                
                FlashLight.transform.position = Vector3.Lerp(FlashLight.transform.position, position, 10 * Time.deltaTime);
                FlashLight.transform.rotation = Quaternion.Lerp(FlashLight.transform.rotation, rotation, 10 * Time.deltaTime);
            }
        }

        public void ChargeBattery(float rate)
        {
            batteryCharge = Mathf.Clamp((batteryCharge + (rate * ChargeEfficiency * Time.deltaTime)), 0, MaxBatteryCharge);
        }

        public void DischargeBattery(float rate)
        {
            batteryCharge = Mathf.Clamp((batteryCharge - (rate * Time.deltaTime)), 0, MaxBatteryCharge);
        }

        public void KillBattery()
        {
            batteryCharge = 0;
        }

        protected override void Awake()
        {
            base.Awake();
            charaterType = CharacterType.Drone;
        }

        protected override void Start()
        {
            base.Start();
            active = true;
            batteryCharge = MaxBatteryCharge;
            timer = PowerDownTimeLimit;
            RegisterHitCallback(PlayHitSound);
            StartSound();
        }

        protected override void Update()
        {
            base.Update();

            if (OnlineModel) OnlineModel.SetActive(Alive && Active);
            if (OfflineModel) OfflineModel.SetActive(Alive && !Active);
            if (DamagedModel) DamagedModel.SetActive(!Alive);

            if (!Alive || isBatteryDead)
            {
                StopSound();
            }

            UpdateAudio();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            active = Alive && !isBatteryDead ? active : false;

            if (!Alive) return;

            rigidBody.centerOfMass = CenterOfMassOffset;

            if (FlashLight)
            {
                FlashLight.enabled = flash;
            }

            if (active)
            {
                var rate = flash ? FlashLightDischargeRate + DischargeRate : DischargeRate;
                DischargeBattery(rate);
                timer = PowerDownTimeLimit;
            }
            else if (Alive && !isBatteryDead)
            {
                flash = false;
                if (timer <= 0)
                {
                    KillBattery();
                }
                else
                {
                    timer -= Time.deltaTime;
                }
            }

            GroundedMove();
            if (!boosting) CapSpeed();
            CheckGroundStatus();
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            TogglePower();
            SetFlashLight(null);
        }

        protected override void Animate(Animator animator)
        {
            
        }
        
        private void CapSpeed()
        {
            if (Speed > Topspeed)
                rigidBody.velocity = (Topspeed / 3.6f) * rigidBody.velocity.normalized;
        }

        private void GroundedMove()
        {
            rigidBody.useGravity = true;

            MoveInput = Active ? Vector3.ProjectOnPlane(MoveInput, Vector3.up) : Vector3.zero;
            LookDirection = Vector3.ProjectOnPlane(LookDirection, Vector3.up);

            var accel = Vector3.Dot(transform.forward, MoveInput);
            var steer = Vector3.Dot(transform.right, MoveInput);
            var handBrake = MoveInput.magnitude > 0 ? 0 : 1;

            if (BoostInput && !boosting && boostCoolDownTimer <= 0) StartCoroutine(Boost());

            ApplyWheelDrive(accel, steer, -accel, handBrake);
            AddDownForce();
        }

        private void ApplyWheelDrive(float accel, float steering, float footbrake, float handbrake)
        {
            //clamp input values
            SteerAngle = Mathf.Clamp(steering, -1, 1) * MaximumSteerAngle;
            Accel = Mathf.Clamp(accel, 0, 1);
            Brake = Mathf.Clamp(footbrake, 0, 1);
            HandBrake = Mathf.Clamp(handbrake, 0, 1);

            for (int i = 0; i < Wheels.Length; i++)
            {
                WheelHit leftWheelhit;
                var leftWheelGrounded = Wheels[i].LeftWheel.Colldier.GetGroundHit(out leftWheelhit);
                SetWheel(Wheels[i].LeftWheel);
                AdjustTorque(leftWheelhit.forwardSlip);

                WheelHit rightWheelhit;
                var rightWheelGrounded = Wheels[i].RightWheel.Colldier.GetGroundHit(out rightWheelhit);
                SetWheel(Wheels[i].RightWheel);
                AdjustTorque(rightWheelhit.forwardSlip);

                if (Wheels[i].UseAntiRoll)
                {
                    var travelL = 1.0f;
                    var travelR = 1.0f;

                    if (leftWheelGrounded)
                        travelL = (-Wheels[i].LeftWheel.Colldier.transform.InverseTransformPoint(leftWheelhit.point).y - Wheels[i].LeftWheel.Colldier.radius) / Wheels[i].LeftWheel.Colldier.suspensionDistance;
                    
                    if (rightWheelGrounded)
                        travelR = (-Wheels[i].RightWheel.Colldier.transform.InverseTransformPoint(rightWheelhit.point).y - Wheels[i].RightWheel.Colldier.radius) / Wheels[i].RightWheel.Colldier.suspensionDistance;

                    var antiRollForce = (travelL - travelR) * AntiRollForce;

                    if (Wheels[i].LeftWheel.Colldier)
                    {
                        rigidBody.AddForceAtPosition(Wheels[i].LeftWheel.Colldier.transform.up * -antiRollForce, Wheels[i].LeftWheel.Colldier.transform.position);
                    }

                    if (Wheels[i].RightWheel.Colldier)
                    {
                        rigidBody.AddForceAtPosition(Wheels[i].RightWheel.Colldier.transform.up * antiRollForce, Wheels[i].RightWheel.Colldier.transform.position);
                    }
                }
            }

            if (SteerHelper > 0)
            {
                // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
                if (Mathf.Abs(oldRotation - transform.eulerAngles.y) < 10f)
                {
                    var turnadjust = (transform.eulerAngles.y - oldRotation) * SteerHelper;
                    Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
                    rigidBody.velocity = velRotation * rigidBody.velocity;
                }
                oldRotation = transform.eulerAngles.y;
            }
        }

        private void SetWheel(Wheel wheel)
        {
            if (!wheel.Colldier) return;

			wheel.Colldier.brakeTorque = HandBrake * MaxHandbrakeTorque;
            
            if (wheel.CanTurn)
            {
                wheel.Colldier.steerAngle = SteerAngle;
                wheel.Colldier.steerAngle = SteerAngle;
            }

            wheel.Colldier.motorTorque = Accel * (currentTorque / Wheels.Length);

            if (Speed > 5 && Vector3.Angle(transform.forward, rigidBody.velocity) < 50f)
            {
                wheel.Colldier.brakeTorque = BrakeTorque * Brake;
            }
            else if (Brake > 0)
            {
                wheel.Colldier.brakeTorque = 0f;
                wheel.Colldier.motorTorque = -ReverseTorque * Brake;
            }
            
            if (wheel.Transform)
            {
                Quaternion quat;
                Vector3 position;
                wheel.Colldier.GetWorldPose(out position, out quat);
                wheel.Transform.position = position;
                wheel.Transform.rotation = quat;
            }
        }

        private void AdjustTorque(float forwardSlip)
        {
            if (forwardSlip >= SlipLimit && currentTorque >= 0)
            {
                currentTorque -= 10 * TractionControl;
            }
            else
            {
                currentTorque += 10 * TractionControl;
                if (currentTorque > FullTorqueOverAllWheels)
                {
                    currentTorque = FullTorqueOverAllWheels;
                }
            }
        }
        
        private void AddDownForce()
        {
            rigidBody.AddForce(-transform.up * Downforce * rigidBody.velocity.magnitude);
        }
        
        private IEnumerator Boost()
        {
            PlayBoostSound();
            boosting = true;
            var start = Time.time;
            var end = start + BoostTime;

            while (Time.time < end)
            {
                boostCoolDownTimer = Utility.Interpolate(0, BoostCoolDownTime, start, end, Time.time);
                rigidBody.AddForce(Vector3.ProjectOnPlane(transform.forward, Vector3.up) * BoostForce * Time.deltaTime, ForceMode.Acceleration);
                yield return new WaitForEndOfFrame();
            }

            boostCoolDownTimer = BoostCoolDownTime;
            boosting = false;

            while (boostCoolDownTimer > 0)
            {
                boostCoolDownTimer -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        private void StartSound()
        {
            // setup the audio sources
            powerSource = SetUpAudioSource(null);
            accelSource = SetUpAudioSource(AccelerateClip);
            hitSource = SetUpAudioSource(null);
            signalSource = SetUpAudioSource(NoSignalClip);

            // flag that we have started the sounds playing
            startedSound = true;
        }
        
        private void StopSound()
        {
            //Destroy all audio sources on this object:
            foreach (var source in GetComponents<AudioSource>())
            {
                Destroy(source);
            }

            startedSound = false;
        }
        
        private void UpdateAudio()
        {
            if (!startedSound) return;

            powerSource.volume = 1;
            hitSource.volume = 1;
            
            accelSource.volume = Mathf.Clamp01(MoveSpeed);
            signalSource.volume = DistanceFromShipPercentage > 0.8f ?
                Utility.Interpolate(0f, 1f, 0.8f, 1f, DistanceFromShipPercentage) : 0f;

            powerSource.dopplerLevel = UseDoppler ? DopplerLevel : 0;
            accelSource.dopplerLevel = UseDoppler ? DopplerLevel : 0;
            hitSource.dopplerLevel = UseDoppler ? DopplerLevel : 0;
            signalSource.dopplerLevel = UseDoppler ? DopplerLevel : 0;
        }

        private void PlayPowerUpSound()
        {
            if (!startedSound) return;

            powerSource.clip = PowerUpClip;
            powerSource.loop = false;
            powerSource.Play();
        }

        private void PlayPowerDownSound()
        {
            if (!startedSound) return;

            powerSource.clip = PowerDownClip;
            powerSource.loop = false;
            powerSource.Play();
        }
        
        private void PlayBoostSound()
        {
            if (!startedSound) return;

            powerSource.clip = BoostClip;
            powerSource.loop = false;
            powerSource.Play();
        }

        private void PlayHitSound()
        {
            if (!startedSound || HitClips.Length < 1) return;

            hitSource.clip = HitClips[Random.Range(0, HitClips.Length)];
            hitSource.loop = false;
            hitSource.Play();
        }
        
        private static float ULerp(float from, float to, float value)
        {
            return (1.0f - value) * from + value * to;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + CenterOfMassOffset, 0.1f);
        }
        
        private void UnderWaterMove()
        {
            //MoveInput = Vector3.ProjectOnPlane(MoveInput, Vector3.up);
            //var accel = Vector3.Dot(transform.forward, MoveInput);
            //var steering = Vector3.Dot(transform.right, MoveInput);
            //var brake = -accel;

            //rigidBody.useGravity = false;
            //ApplyMove(UnderWaterSpeed);
        }
    }

    [System.Serializable]
    public class WheelPair
    {
        public Wheel LeftWheel;
        public Wheel RightWheel;
        public bool UseAntiRoll = true;
    }

    [System.Serializable]
    public class Wheel
    {
        public WheelCollider Colldier;
        public Transform Transform;
        public bool CanTurn = true;
    }
}