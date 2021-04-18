using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using PII.Utilities;

namespace PII
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class Human : Character
    {
        [SerializeField] private float WalkSpeed = 5;
        [SerializeField] private float SprintSpeed = 10;
        [Range(0f, 2f)] [SerializeField] private float AirMoveSpeed = 0.1f;
        [SerializeField] private float JumpForce = 12f;
        [SerializeField] private bool CanJump = true;
        [SerializeField] private float Drag = 1f;
        [SerializeField] private float AttackDamage = 10f;
        [SerializeField] private float AttackRadius = 1;
        [SerializeField] private float AttackAngle = 20f;
        [SerializeField] private float AttackCoolDownTime = 1.2f;
        [SerializeField] private float FootStepTime = 1f;
        [SerializeField] private AudioClip[] FootStepClips;
        [SerializeField] private AudioClip JumpClip;
        [SerializeField] private AudioClip LandClip;
        [SerializeField] private AudioClip AttackClip;
        
        private bool attacking = false;
        private bool startedSound = false;
        private bool wasGrounded = false;
        private float timer;

        private AudioSource footStepsSource;
        private AudioSource jumpAndLandSource;
        private AudioSource attackSource;

		public bool Strafe { get; set; }
        public bool SprintInput { get; set; }
        public bool JumpInput { get; set; }

        public void Attack()
        {
            if (attacking)
                return;

            StartCoroutine(StartAttack());
        }

        protected override void Awake()
        {
            base.Awake();
            charaterType = CharacterType.Human;
        }

        protected override void Update()
        {
            base.Update();

            if (startedSound && !Alive)
            {
                StopSound();
            }

            if (!startedSound && Alive)
            {
                StartSound();
            }

            UpdateAudio();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            
            rigidBody.drag = Drag;

            BaseMove();

            CheckGroundStatus();
        }

        private void BaseMove()
        {
            rigidBody.useGravity = true;
            MoveInput = Vector3.ProjectOnPlane(MoveInput, Vector3.up);
            LookDirection = Vector3.ProjectOnPlane(LookDirection, Vector3.up);

            if (MoveInput.magnitude > 1) MoveInput.Normalize();

            if (Grounded)
            {
                var speed = SprintInput ? SprintSpeed : WalkSpeed;
                ApplyMove(speed);

                if (JumpInput && CanJump)
                {
                    rigidBody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
                    PlayJumpSound();
                }

                wasGrounded = true;
            }
            else
            {
                ApplyMove(AirMoveSpeed);
            }

            if (wasGrounded && Grounded)
            {
                PlayLandSound();
                wasGrounded = false;
            }
        }

        private void SwimMove()
        {
            //rigidBody.useGravity = false;
            //ApplyMove(SwimSpeed);
        }

        protected override void Animate(Animator animator)
        {
            var forward = Mathf.Clamp(Vector3.Dot(MoveInput, transform.forward), -1, 1);
            var turn = Mathf.Clamp(Vector3.Dot(MoveInput, transform.right), -1, 1);

			animator.SetFloat ("Forward", forward);
			animator.SetFloat ("Turn", turn);
			animator.SetBool ("Sprint", SprintInput);
			animator.SetBool ("Attack", attacking);
			animator.SetBool ("Strafe", Strafe);
        }

        private IEnumerator StartAttack()
        {
            attacking = true;

            yield return new WaitForSeconds(AttackCoolDownTime);

            var colliders = Physics.OverlapSphere(transform.position, AttackRadius, -1, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < colliders.Length; i++)
            {
                var direction = (colliders[i].transform.position - transform.position).normalized;
                var angle = Vector3.Angle(direction, transform.forward);

                if (angle < AttackAngle * 0.5f)
                {
                    var health = colliders[i].attachedRigidbody ?
                    colliders[i].attachedRigidbody.GetComponent<HealthObject>() :
                    colliders[i].GetComponent<HealthObject>();

                    if (health)
                    {
                        if (health != this)
                            health.Damage(AttackDamage);
                    }
                }
            }
            PlayAttackSound();
            attacking = false;
        }
        
        private void StartSound()
        {
            // setup the audio sources
            footStepsSource = SetUpAudioSource(null);
            jumpAndLandSource = SetUpAudioSource(null);
            attackSource = SetUpAudioSource(null);

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

            footStepsSource.volume = 1;
            footStepsSource.volume = 1;
            attackSource.volume = 1;

            footStepsSource.dopplerLevel = UseDoppler ? DopplerLevel : 0;
            footStepsSource.dopplerLevel = UseDoppler ? DopplerLevel : 0;
            attackSource.dopplerLevel = UseDoppler ? DopplerLevel : 0;

            if (MoveInput.magnitude > 0)
            {
                timer += Time.deltaTime;
                var time = SprintInput ? FootStepTime * (WalkSpeed / SprintSpeed) : FootStepTime;

                if (timer >= time * MoveInput.magnitude)
                {
                    PlayFootStepSound();
                    timer = 0;
                }
            }
            else
            {
                timer = 0;
            }
        }
        
        private void PlayFootStepSound()
        {
            if (!startedSound || FootStepClips.Length < 1) return;

            footStepsSource.clip = FootStepClips[Random.Range(0, FootStepClips.Length)];
            footStepsSource.loop = false;
            footStepsSource.Play();
        }

        private void PlayJumpSound()
        {
            if (!startedSound) return;

            jumpAndLandSource.clip = JumpClip;
            jumpAndLandSource.loop = false;
            jumpAndLandSource.Play();
        }

        private void PlayLandSound()
        {
            if (!startedSound) return;

            jumpAndLandSource.clip = LandClip;
            jumpAndLandSource.loop = false;
            jumpAndLandSource.Play();
        }

        private void PlayAttackSound()
        {
            if (!startedSound) return;

            attackSource.clip = AttackClip;
            attackSource.loop = false;
            attackSource.Play();
        }
    }
}