using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PII.Utilities
{
    public class ItemSpawnArea : SpawnArea
    {
        [System.Serializable]
        public struct ItemProfile
        {
            public Item Item;
            public int Count;
        }

        [SerializeField] ItemProfile[] Items;

        protected override void Awake()
        {
            base.Awake();
            GameManager.StartGameCallback += SpawnItems;
        }

        private void SpawnItems()
        {
            for (int i = 0; i < Items.Length; i++)
            {
                for (int j = 0; j < Items[i].Count; j++)
                {
                    Instantiate(Items[i].Item, GetRandomSpawnPoint(), Quaternion.identity);
                }
            }
        }
    }
}