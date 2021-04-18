using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using PII.Utilities;

namespace PII
{
    [RequireComponent(typeof(Rigidbody))]
    public class Item : MonoBehaviour
    {
        public string Name;

        private static List<Item> Items = new List<Item>();
        
        [SerializeField] private Sprite UISprite;
        [SerializeField] private Vector3 PickUpCenter = Vector3.zero;
        [SerializeField] private Vector3 PickUpSize = Vector3.one;
        [SerializeField] protected AudioMixerGroup Mixer;
        [SerializeField] private AudioClip PickUpClip;
        [SerializeField] private AudioClip DropClip;

        private bool shipItem = false;
        private AudioSource audioSource;
        private Character owner;
        private Rigidbody rigidBody;
        private Collider[] colliders;
        private Renderer[] renderers;

        private Bounds pickUpBounds { get { return new Bounds(transform.position + PickUpCenter, PickUpSize); } }
        public Character Owner { get { return owner; } }
        public Sprite UI { get { return UISprite; } }

        public static Item[] ItemsInScene { get { return Items.ToArray(); } }

        private void Awake()
        {
            if (!Items.Contains(this)) Items.Add(this);
        }

        private void Start()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            rigidBody = GetComponent<Rigidbody>();
            colliders = GetComponentsInChildren<Collider>();
            renderers = GetComponentsInChildren<Renderer>();
        }

        private void FixedUpdate()
        {
            audioSource.outputAudioMixerGroup = Mixer;
            audioSource.spatialBlend = 0;

            var value = shipItem || owner;
            rigidBody.isKinematic = value;

            if (colliders != null)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    colliders[i].enabled = !value;
                }
            }

            if (renderers != null)
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].enabled = !value;
                }
            }

            if (shipItem)
            {
                transform.position = Ship.Instance.transform.position;
            }
            else if (owner)
            {
                transform.position = owner.transform.position;
                if (!owner.Alive) owner.Drop(this);
            }
        }

        private void OnPickUp(Character character)
        {
            owner = character;
            shipItem = false;
            PlayPickUpSound();
        }

        private void OnDrop()
        {
            owner = null;
            shipItem = false;
            PlayDropSound();
        }

        private void OnShip()
        {
            owner = null;
            shipItem = true;
        }

        private void PlayPickUpSound()
        {
            if (!audioSource) return;

            audioSource.clip = PickUpClip;
            audioSource.loop = false;
            audioSource.Play();
        }

        private void PlayDropSound()
        {
            if (!audioSource) return;

            audioSource.clip = DropClip;
            audioSource.loop = false;
            audioSource.Play();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(pickUpBounds.center, pickUpBounds.size);
        }

        public static bool NearItem(Vector3 position, out Item item)
        {
            item = null;

            if (Items != null)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].pickUpBounds.Contains(position) && !Items[i].owner && !Items[i].shipItem)
                    {
                        item = Items[i];
                        return true;
                    }
                }
            }

            return false;
        }

        private void OnDestroy()
        {
            if (Items.Contains(this))
                Items.Remove(this);
        }

        public static void DestroyAllItems()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Destroy(Items[i].gameObject);
            }
        }
    }
}