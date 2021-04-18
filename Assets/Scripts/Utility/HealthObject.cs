using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PII
{
    public class HealthObject : MonoBehaviour
    {
        [SerializeField] private float MaxHealth = 100;
        [SerializeField] private bool RegenerateAutomatically = true;
        [SerializeField] private float RegenerateRate = 6;

        private float health;
        private OnHit hitCallback;
        
        public float Health { get { return health; } }
        public float HealthPercentage { get { return health / MaxHealth; } }
        public bool Alive { get { return health > 0; } }
        
        public void Damage(float damage)
        {
            health = Mathf.Clamp((health - (damage * Time.deltaTime)), 0, MaxHealth);

            if (Alive)
            {
                if (hitCallback != null) hitCallback();
            }
            else
            {
                OnDeath();
            }
        }

        public void Kill()
        {
            health = 0;
            Damage(0);
        }
        
        public void RegisterHitCallback(OnHit callback)
        {
            hitCallback += callback;
        }

        public void UnRegisterHitCallback()
        {
            hitCallback = null;
        }

        public void UnRegisterHitCallback(OnHit callback)
        {
            hitCallback -= callback;
        }

        protected void InitializeHealth()
        {
            health = MaxHealth;
        }
        
        protected virtual void Awake()
        {
            InitializeHealth();
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {

        }

        protected virtual void FixedUpdate()
        {
            if (RegenerateAutomatically)
                Regenerate();
        }
        
        protected virtual void OnDestroy()
        {

        }

        protected virtual void OnDeath()
        {
            UnRegisterHitCallback();
        }

        private void Regenerate()
        {
            if (!Alive)
                return;

            health = Mathf.Clamp((health + (RegenerateRate * Time.deltaTime)), 0, MaxHealth);
        }

        public delegate void OnHit();
    }
}