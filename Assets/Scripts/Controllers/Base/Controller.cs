using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PII
{
    public class Controller : MonoBehaviour
    {
        private Character character;

        public Character Body { get { return character; } }
        public Vector3 BodyPosition { get { return character.transform.position; } }
        
        private static List<Controller> Controllers = new List<Controller>();
        public static Controller[] ControllersInScene { get { return Controllers.ToArray(); } }

        public virtual void Possess(Character character)
        {
            if (!character)
                return;

            if (character.Authority)
            {
                character.Authority.UnPossess();
            }

            UnPossess();

            this.character = character;
            this.character.SendMessage("OnPossess", this);
        }

        public virtual void UnPossess()
        {
            if (!character)
                return;

            character.SendMessage("OnUnPossess");
            character = null;
        }

        protected virtual void ControlDrone(Drone drone){}

        protected virtual void ControlHuman(Human human){}

        protected virtual void Awake()
        {
            if (!Controllers.Contains(this)) Controllers.Add(this);
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {

        }

        protected virtual void LateUpdate()
        {
            
        }

        protected virtual void FixedUpdate()
        {
            if (GameManager.GameStarted && !GameManager.GamePaused && character)
            {
                if (character.Alive)
                {
                    switch (character.Type)
                    {
                        case CharacterType.Drone:
                            ControlDrone((Drone)Body);
                            break;
                        case CharacterType.Human:
                            ControlHuman((Human)Body);
                            break;
                    }
                }
                else
                {
                    UnPossess();
                }
            }
        }

        private void OnDestroy()
        {
            if (Controllers.Contains(this)) Controllers.Remove(this);
        }

        public static void DestroyAllControllers()
        {
            for (int i = 0; i < Controllers.Count; i++)
            {
                Destroy(Controllers[i].gameObject);
            }
        }
    }
}