using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using PII.Utilities;

namespace PII
{
    public enum CharacterType
    {
        Base,
        Drone,
        Human
    }
    
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public class Character : HealthObject
    {
        public const float WaterDamage = 15.25f;
        
        [SerializeField] protected float GroundCheckDistance = 0.5f;
        [SerializeField] protected Vector3 GroundCheckOffset;
        [SerializeField] protected LayerMask GroundMask = -1;
        [Range(1f, 4f)] [SerializeField] protected float GravityMultiplier = 2f;
        [SerializeField] protected float MinHeightToDamage = 5f;
        [SerializeField] protected float MinHeightDamage = 5f;
        [SerializeField] protected int MaxItems = 5;
        [SerializeField] protected List<Item> Items = new List<Item>();
        [SerializeField] private AISettings AI;
        [SerializeField] protected AudioMixerGroup Mixer;
        [SerializeField] protected float MinRolloffDistance = 2;
        [SerializeField] protected float MaxRolloffDistance = 500;
        [SerializeField] protected float DopplerLevel = 1;
        [SerializeField] protected bool UseDoppler = true;

        protected CharacterType charaterType = CharacterType.Base;

        private bool grounded;
        private float distanceToground;
        private Item closestItem;

        private Controller controller;
        protected Animator animator;
        protected Rigidbody rigidBody;

        public Vector3 MoveInput { get; set; }
        public Vector3 LookDirection { get; set; }
        public bool PickUpInput { get; set; }

        public CharacterType Type { get { return charaterType; } }
        public float MoveSpeed { get { return rigidBody.velocity.magnitude * MoveInput.magnitude; } }
        public bool Grounded { get { return grounded; } }
        public bool InWater { get { return Water.InWater(transform.position); } }
        public bool NearItem { get { return Item.NearItem(transform.position, out closestItem); } }
        public AISettings AISettings { get { return AI; } }
        public Controller Authority { get { return controller; } }
        public Item[] Possession { get { return Items.ToArray(); } }
        
        private static List<Character> Characters = new List<Character>();
        public static Character[] CharactersInScene { get { return Characters.ToArray(); } }

        public void PickUp(Item item)
        {
            if (!item || Items.Count >= MaxItems)
                return;

            if (item.Owner)
            {
                item.Owner.Drop(item);
            }
            
            if (!Items.Contains(item))
            {
                item.SendMessage("OnPickUp", this);
                Items.Add(item);
            }
        }

        public void PickUp(Item[] items)
        {
            if (items == null)
                return;

            for (int i = 0; i < items.Length; i++)
            {
                PickUp(items[i]);
            }
        }

        public void Drop(Item item)
        {
            if (!item)
                return;

            if (Items.Contains(item))
            {
                item.SendMessage("OnDrop");
                Items.Remove(item);
            }
        }

        public void Drop(Item[] items)
        {
            if (items == null)
                return;

            for (int i = 0; i < items.Length; i++)
            {
                Drop(items[i]);
            }
        }

        public void DropAllItems()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Drop(Items[i]);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (!Characters.Contains(this)) Characters.Add(this);

            animator = GetComponent<Animator>();
            rigidBody = GetComponent<Rigidbody>();

            PickUp(Items.ToArray());
        }

        protected override void Update()
        {
            base.Update();
            Animate(animator);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (InWater) Damage(WaterDamage);

            if (NearItem && PickUpInput) PickUp(closestItem); 
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (Characters.Contains(this)) Characters.Remove(this);
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            DropAllItems();
        }

        protected void ApplyMove(float speed)
        {
            if (!Alive)
                return;

            if (LookDirection != Vector3.zero)
                rigidBody.MoveRotation(Quaternion.LookRotation(LookDirection));

            if (MoveInput.magnitude > 1)
                MoveInput.Normalize();

            rigidBody.MovePosition(transform.position + MoveInput * speed * Time.deltaTime);
        }

        protected void CheckGroundStatus()
        {
            grounded = rigidBody.useGravity;

            if (rigidBody.useGravity)
            {
                RaycastHit hitInfo;
                var origin = transform.position + GroundCheckOffset;
                grounded = Physics.Raycast(origin, Vector3.down, out hitInfo, GroundCheckDistance, GroundMask, QueryTriggerInteraction.Ignore);
                animator.applyRootMotion = grounded;

                if (grounded)
                {
                    Debug.DrawLine(origin, hitInfo.point, Color.green);

                    if (distanceToground >= MinHeightToDamage)
                    {
                        var ratio = distanceToground / MinHeightToDamage;
                        Damage(MinHeightDamage * ratio);
                    }

                    distanceToground = 0;
                }
                else
                {
                    Debug.DrawLine(origin, origin + (Vector3.down * GroundCheckDistance), Color.red);

                    distanceToground += Mathf.Sqrt(Mathf.Pow(rigidBody.velocity.y, 2)) * Time.deltaTime;

                    Vector3 extraGravityForce = (Physics.gravity * GravityMultiplier) - Physics.gravity;
                    rigidBody.AddForce(extraGravityForce);
                }
            }
        }

        protected AudioSource SetUpAudioSource(AudioClip clip)
        {
            // create the new audio source component on the game object and set up its properties
            AudioSource source = Utility.SetUpAudioSource(gameObject, Mixer, clip);

            source.minDistance = MinRolloffDistance;
            source.maxDistance = MaxRolloffDistance;
            source.spatialBlend = 1;
            source.dopplerLevel = UseDoppler ? DopplerLevel : 0;

            return source;
        }

        protected virtual void Animate(Animator animator){}
        
        private void OnPossess(Controller controller)
        {
            this.controller = controller;
        }

        private void OnUnPossess()
        {
            controller = null;
        }

        public static void DestroyAllCharacters()
        {
            for (int i = 0; i < Characters.Count; i++)
            {
                Destroy(Characters[i].gameObject);
            }
        }
    }
}