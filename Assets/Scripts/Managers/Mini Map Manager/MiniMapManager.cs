using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PII
{
    [RequireComponent(typeof(Canvas))]
    public class MiniMapManager : MonoBehaviour
    {
        [SerializeField] private RectTransform ShipPrefab;
        [SerializeField] private RectTransform UserCharacterPrefab;
        [SerializeField] private RectTransform DronePrefab;
        [SerializeField] private RectTransform HumanPrefab;
        [SerializeField] private RectTransform ItemPrefab;

        private Canvas canvas;
        private RectTransform ship;
        private RectTransform user;
        private List<RectTransform> characters = new List<RectTransform>();
        private List<RectTransform> items = new List<RectTransform>();

        public static Camera MiniMapCamera { get; set; }

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            
            GameManager.EndGameCallback += DestroyAllElements;
        }

        private void DestroyAllElements()
        {
            Destroy(ship);
            Destroy(user);

            DestroyAll(characters.ToArray());
            characters.Clear();

            DestroyAll(items.ToArray());
            items.Clear();
        }

        private void LateUpdate()
        {
            if (!GameManager.GameStarted) return;

            SetShip();
            SetUserCharacter();
            SetCharacters();
            SetItems();
        }

        private void SetShip()
        {
            if (!ship) ship = Instantiate(ShipPrefab, transform);

            SetIconClamped(Ship.Instance.transform, ship, 1, 0.4f);
        }

        private void SetUserCharacter()
        {
            if (!GameManager.User) return;
            if (!GameManager.User.Body) return;

            if (!user) user = Instantiate(UserCharacterPrefab, transform);
            SetIcon(GameManager.User.Body.transform, user, 2, 0.4f);
        }

        private void SetCharacters()
        {
            if (characters == null) return;

            if (characters.Count != Character.CharactersInScene.Length - 1)
            {
                DestroyAll(characters.ToArray());
                characters.Clear();

                for (int i = 0; i < Character.CharactersInScene.Length; i++)
                {
                    switch (Character.CharactersInScene[i].Type)
                    {
                        case CharacterType.Drone:
                            characters.Add(Instantiate(DronePrefab, transform));
                            break;
                        case CharacterType.Human:
                            characters.Add(Instantiate(HumanPrefab, transform));
                            break;
                    }
                }
            }

            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i])
                {
                    characters[i].gameObject.SetActive(!(Character.CharactersInScene[i] == GameManager.User.Body));
                    SetIcon(Character.CharactersInScene[i].transform, characters[i], 1.5f);
                }
            }
        }

        private void SetItems()
        {
            if (items == null) return;

            if (items.Count != Item.ItemsInScene.Length)
            {
                DestroyAll(items.ToArray());
                items.Clear();

                for (int i = 0; i < Item.ItemsInScene.Length; i++)
                {
                    items.Add(Instantiate(ItemPrefab, transform));
                }
            }

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i]) SetIconClamped(Item.ItemsInScene[i].transform, items[i], 0.5f);
            }
        }
        
        private void SetIcon(Transform transform, RectTransform rectTransfrom, float height = 0.1f, float size = 0.2f)
        {
            rectTransfrom.position = new Vector3(transform.position.x, this.transform.position.y + height, transform.position.z);
            rectTransfrom.rotation = Quaternion.Euler(90, transform.eulerAngles.y, 0);
            rectTransfrom.localScale = Vector3.one * size;
        }

        private void SetIconClamped(Transform transform, RectTransform rectTransfrom, float height = 0.1f, float size = 0.2f)
        {
            if (MiniMapCamera)
            {
                var clamp = MiniMapCamera.orthographicSize - 0.1f;
                var x = Clamp(transform.position.x, MiniMapCamera.transform.position.x - clamp, MiniMapCamera.transform.position.x + clamp);
                var z = Clamp(transform.position.z, MiniMapCamera.transform.position.z - clamp, MiniMapCamera.transform.position.z + clamp);

                rectTransfrom.position = new Vector3(x, this.transform.position.y + height, z);
                rectTransfrom.rotation = Quaternion.Euler(90, transform.eulerAngles.y, 0);
                rectTransfrom.localScale = Vector3.one * size;
                return;
            }

            SetIcon(transform, rectTransfrom, height, size);
        }

        private float Clamp(float value, float min, float max)
        {
            return value > max ? max : (value < min ? min : value);
        }

        private void DestroyAll(RectTransform[] transforms)
        {
            if (transforms == null) return;

            for (int i = 0; i < transforms.Length; i++)
            {
                if (transforms[i]) Destroy(transforms[i].gameObject);
            }
        }
    }
}