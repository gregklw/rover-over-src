using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using PII.Utilities;

namespace PII
{
    public class AIController : Controller
    {
        public const float TimeWithoutCharacter = 5;

        public enum State
        {
            Nothing,
            Wondering,
            Routeing,
            Attacking
        }
        [SerializeField] private State CharacterState;

        // ai settings from the character
        private AISettings settings;
        // timers
        private float waitTimer;
        private float timer;
        // navigation variables
        private Vector3 target;
        private Vector3 lastTarget;
        private Character enemy;
        // inputs for the character
        private Vector3 move;
        private bool sprint;
        private bool attack;

        private float distanceToTarget { get { return Vector3.Distance(target, BodyPosition); } }
        private bool reachedTarget { get { return distanceToTarget <= settings.StopDistance; } }
        
        private State characterState { get { return CharacterState; } }

        public void SetTarget(Vector3 target)
        {
            this.target = target;
        }
        
        protected override void Start()
        {

        }

        protected override void FixedUpdate()
        {
            if (Body)
            {
                timer = TimeWithoutCharacter;
                settings = Body.AISettings;

                if (settings != null)
                {
                    CheckSurrounding();
                    SetState();
                    SetNavAgent();
                    SetMove();
                    ResetTimers();
                }
            }
            else
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    Destroy(gameObject);
                }
            }

            base.FixedUpdate();
        }

        protected override void ControlHuman(Human human)
        {
            if (!human)
                return;

            human.MoveInput = move;
            human.LookDirection = move;
            human.SprintInput = sprint;
			human.Strafe = enemy;

            if (attack)
            {
                human.Attack();
            }
        }

        private void CheckSurrounding()
        {
            var vision = settings.Vision;
            vision.transform.position = BodyPosition;

            if (vision.EnemyInSight && !enemy)
            {
                enemy = vision.GetClosestEnemy().GetComponent<Character>();
                if (enemy)
                {
                    if (enemy.Type == CharacterType.Drone)
                    {
                        var drone = enemy.GetComponent<Drone>();
                        if (!drone.Active)
                        {
                            enemy = null;
                        }
                    }
                }
            }
        }
        
        private void SetState()
        {
            if (enemy)
            {
                var enemyPosition = enemy.transform.position;
                Route(enemyPosition);

                if (reachedTarget)
                {
                    Attack();
                }

                if (!enemy.Alive || distanceToTarget >= settings.GiveUpDistance)
                {
                    enemy = null;
                }
            }
            else
            {
                Wonder();
            }
        }
        
        private void Wonder()
        {
            CharacterState = State.Wondering;
            sprint = false;
            attack = false;

            if (reachedTarget)
            {
                waitTimer += Time.deltaTime;
                if (waitTimer >= settings.WaitTime)
                {
                    target = Utility.RandomWorldPointOnNavMesh(BodyPosition, settings.WonderRadius);
                    waitTimer = 0;
                }
            }
        }

        private void Route(Vector3 to)
        {
            CharacterState = State.Routeing;
            attack = false;

            target = to;
            sprint = true;
        }

        private void Attack()
        {
            CharacterState = State.Attacking;
            sprint = false;

            attack = reachedTarget && enemy;
        }

        private void SetNavAgent()
        {
            var navAgent = settings.NavAgent;
            navAgent.enabled = true;

            navAgent.speed = 1;
            navAgent.angularSpeed = 0;
            navAgent.acceleration = 1;
            navAgent.stoppingDistance = 0;
            navAgent.autoBraking = false;

            navAgent.transform.position = Body.transform.position;
            navAgent.transform.rotation = Body.transform.rotation;
            
            navAgent.SetDestination(target);
        }

        private void SetMove()
        {
            move = reachedTarget ? Vector3.zero : settings.NavAgent.desiredVelocity;
        }

        public void ResetTimers()
        {
            if (!reachedTarget)
                waitTimer = 0;
        }

        private void OnDrawGizmos()
        {
            /*
            if (patrolRoute != null)
            {
                for (int i = 0; i < patrolRoute.Length; i++)
                {
                    Gizmos.color = i == patrolIndex ? Color.cyan : Color.yellow;
                    Gizmos.DrawWireCube(patrolRoute[i], Vector3.one);
                }
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(target, 1);
            */
        }
    }

    [System.Serializable]
    public class AISettings
    {
        public float WaitTime = 2.5f;
        public float StopDistance = 1.2f;
        public float GiveUpDistance = 10f;
        public float WonderRadius = 20f;
        public float PatrolRadius = 50f;
        public int PatrolLenght = 10;
        public NavMeshAgent NavAgent;
        public Perception Vision;
    }
}