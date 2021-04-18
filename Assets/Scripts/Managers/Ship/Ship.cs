using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PII.Utilities;
using PadInput;

namespace PII
{
    [RequireComponent(typeof(UserSpawnArea))]
    public class Ship : MonoBehaviour
    {
        public const float MaxDroneDistance = 900;
        
        [SerializeField] private int MaxDrones = 3;
        [SerializeField] private Transform[] CameraPivots;
        [SerializeField] private Vector3 ItemDropCenter;
        [SerializeField] private Vector3 ItemDropSize;
        [SerializeField] [Range(0,1)] private float ItemsNeededPercentage = 0.5f;

        private List<Item> itemsList = new List<Item>();
        private List<Item> collectedItems = new List<Item>();

        private int dronesLeft;
        private Drone currentDrone;
        private static Ship instance;
        private int currentCameraPivotIndex;
        private Vector3 currentCameraPivot;

        public bool HasDrone { get { return dronesLeft > 0 || currentDrone; } }
        public Bounds ItemDropBounds { get { return new Bounds((transform.position + ItemDropCenter), ItemDropSize); } }
        public int DronesLeft { get { return dronesLeft; } }
        public static Ship Instance { get { return instance; } }
        public Vector3 CameraPivot { get { return currentCameraPivot; } }
        public bool DroneInPickUp { get { return currentDrone ? ItemDropBounds.Contains(currentDrone.transform.position) : false; } }

        public Item[] ItemsNeeded { get { return itemsList.ToArray(); } }
        public Item[] ItemsCollected { get { return collectedItems.ToArray(); } }

        private void Awake()
        {
            if (instance)
                DestroyImmediate(this);
            else
                instance = this;
        }
        
        private void Start()
        {
            currentCameraPivotIndex = 0;
            dronesLeft = MaxDrones;

            GameManager.StartGameCallback += delegate { dronesLeft = MaxDrones; GetItemsList(); };
        }
        
        private void GetItemsList()
        {
            itemsList.Clear();
            var items = Item.ItemsInScene;

            if (items.Length > 0)
            {
                var number = (int)(items.Length * ItemsNeededPercentage);
                var indexes = new List<int>();
                var index = 0;
                for (int i = 0; i < number; i++)
                {
                    do { index = Random.Range(0, items.Length - 1); } while (indexes.Contains(index));
                    indexes.Add(index);
                    itemsList.Add(items[index]);
                }
            }
        }
        
        private void Update()
        {
            if (GameManager.GameStarted)
            {
                if (currentDrone)
                {
                    CollectItemsFromDrone();

                    if (!currentDrone.Alive || currentDrone.isBatteryDead || currentDrone.DistanceFromShipPercentage > 1)
                    {
                        currentDrone = null;
                        GameManager.User.UnPossess();
                    }
                }
                else if (GameManager.InputManager)
                {
                    GetDrone();
                    SwitchCamera();
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, MaxDroneDistance);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(ItemDropBounds.center, ItemDropBounds.size);
        }

        private void GetDrone()
        {
            if (GameManager.InputManager.GetAxisPressed(InputSystem.ShipAxes[0]) && dronesLeft > 0)
            {
                currentDrone = GameManager.SpawnDrone(UserSpawnArea.SpawnPoint);
                if (currentDrone)
                {
                    GameManager.User.Possess(currentDrone);
                    dronesLeft--;
                }
            }
        }

        private void CollectItemsFromDrone()
        {
            if (currentDrone)
            {
                if (DroneInPickUp && GameManager.InputManager.GetAxisPressed(InputSystem.ShipAxes[1]))
                {
                    var items = currentDrone.Possession;
                    currentDrone.DropAllItems();
                    for (int i = 0; i < items.Length; i++)
                    {
                        items[i].SendMessage("OnShip");
                    }
                    collectedItems.AddRange(items);
                }
            }
        }

        private void SwitchCamera()
        {
            if (GameManager.InputManager.GetAxisPressed(InputSystem.ShipAxes[2]) && CameraPivots.Length > 0)
            {
                currentCameraPivotIndex = currentCameraPivotIndex < CameraPivots.Length - 1 ? currentCameraPivotIndex + 1 : 0;
            }
            
            currentCameraPivot = CameraPivots.Length > 0 ? CameraPivots[currentCameraPivotIndex].localPosition : Vector3.zero;
        }

        public int CollectedItems()
        {
            var count = 0;

            for (int i = 0; i < itemsList.Count; i++)
            {
                if (CollectedItem(i)) count++;
            }

            return count;
        }

        public bool CollectedItem(int index)
        {
            if (index > itemsList.Count - 1) return false;

            return collectedItems.Contains(itemsList[index]);
        }

        public bool CollectedAllItems()
        {
            for (int i = 0; i < itemsList.Count; i++)
            {
                if (!CollectedItem(i)) return false;
            }

            return true;
        }
    }
}