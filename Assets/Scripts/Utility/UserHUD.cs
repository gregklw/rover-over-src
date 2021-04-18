using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PII;
using UIAnimation;

namespace PII.Utilities
{
    [RequireComponent(typeof(Canvas))]
    public class UserHUD : MonoBehaviour
    {
        [SerializeField] private Font MainFont;
        [SerializeField] private float MessageDisplayTime = 1;
        [SerializeField] private RectTransform ItemsUIParent;
        [SerializeField] private RectTransform ItemsUIPrefab;

        [Header("Pause UI")]
        [SerializeField] private RectTransform PausePanel;
        [SerializeField] private Text PauseText;
        [SerializeField] private Button RestartButton;
        [SerializeField] private Button EndGameButton;

        [Header("Ship UI")]
        [SerializeField] private RectTransform ShipPanel;
        [SerializeField] private Text ShipCounterText;
        [SerializeField] private Text ShipMessageText;

        [Header("Drone UI")]
        [SerializeField] private RectTransform DronePanel;
        [SerializeField] private Slider BatterBar;
        [SerializeField] private Slider BoostBar;
        [SerializeField] private SpriteAnimation BoostAnimation;
        [SerializeField] private Image[] DamageImages;
        [SerializeField] private Image HitImage;
        [SerializeField] private Color HitColor = Color.red;
        [SerializeField] private LayerMask MiniMapMask = 9;
        [SerializeField] private RawImage MiniMapImage;
        [SerializeField] private SpriteAnimation NoSignalAnimation;
        [SerializeField] private float MiniMapHeightOffset = 20;
        [SerializeField] private float MiniMapCameraSize = 5;
        [SerializeField] private Text ItemsText;
        [SerializeField] private Text DroneMessageText;

        private Canvas canvas;
        private Canvas miniMapCanvas;
        private List<RectTransform> panels = new List<RectTransform>();
        private List<RectTransform> itemPanels = new List<RectTransform>();
        private RenderTexture miniMapTexture;
        private Camera miniMapCamera;
        private Camera userCamera;
        private Character userCharacter;
        private float messageTimer = 0;
		private int noSignalIndex;
		private float noSignalTimer;

        public void SetUserCamera(Camera camera)
        {
            userCamera = camera;
        }

        public void SetUserCharacter(Character character)
        {
            userCharacter = character;

            if (character)
                userCharacter.RegisterHitCallback(Hit);
        }

        private void Start()
        {
            canvas = GetComponent<Canvas>();
            RegisterPanel(ShipPanel);
            RegisterPanel(PausePanel);
            RegisterPanel(DronePanel);

            SetButtons();

            foreach (var item in Ship.Instance.ItemsNeeded)
            {
                itemPanels.Add(NewItemLabel(item.Name, item.UI));
            }
        }
        
        private void RegisterPanel(RectTransform panel)
        {
            if (!panel) return;

            panels.Add(panel);
        }

        private void EnablePanel(RectTransform panel)
        {
            if (!panel) return;

            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].gameObject.SetActive(panel == panels[i]);
            }
        }

        private void CreateMiniMap()
        {
            miniMapCamera = new GameObject("Mini Map Camera").AddComponent<Camera>();
            miniMapCamera.transform.SetParent(transform);
            miniMapCamera.targetTexture = miniMapTexture = new RenderTexture(500,500,24);
        }

        private void Update()
        {
            foreach (var text in GetComponentsInChildren<Text>())
            {
                text.font = MainFont;
            }

            MiniMapManager.MiniMapCamera = miniMapCamera;
            Cursor.lockState = GameManager.GamePaused ? CursorLockMode.Confined : CursorLockMode.Locked;
            Cursor.visible = GameManager.GamePaused;

            SetCanvas();
            
            if (GameManager.GamePaused) PauseHUD();

            if (GameManager.GameStarted)
            {
                CharacterHUD();
                ShipHUD();
            }
            
            if (messageTimer > 0)
            {
                messageTimer -= Time.deltaTime;
            }
            else
            {
                PrintMessage("");
            }
            
            for (int i = 0; i < itemPanels.Count; i++)
            {
                if (Ship.Instance.CollectedItem(i)) itemPanels[i].GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0.1f);
            }

            SetMiniMap();
        }

        private void PauseHUD()
        {
            EnablePanel(PausePanel);

            PauseText.text = "";
            switch (GameManager.UserProgress)
            {
                case Progress.Playing:
                    PauseText.text = "Find These Items";
                    break;
                case Progress.Won:
                    PauseText.text = "Mission Completed";
                    break;
                case Progress.Lost:
                    PauseText.text = "Mission Failed";
                    break;
            }
        }

        private void CharacterHUD()
        {
            if (userCharacter && !GameManager.GamePaused)
            {
                EnablePanel(DronePanel);
                if (userCharacter.Type == CharacterType.Drone)
                {
                    var drone = userCharacter.GetComponent<Drone>();

                    if (drone.Active)
                    {
                        if (BatterBar)
                        {
                            BatterBar.fillRect.gameObject.SetActive(drone.ChargePercentage > 0);

                            BatterBar.maxValue = 1;
                            BatterBar.minValue = 0;
                            BatterBar.value = drone.ChargePercentage;
                        }

                        if (BoostBar)
                        {
                            BoostBar.maxValue = 1;
                            BoostBar.minValue = 0;
                            BoostBar.value = drone.BoostCoolDownPercentage;
                        }
                        
                        BoostAnimation.SetImageAlpha(drone.Boosting ? 1 : 0);
                        if (drone.Boosting) BoostAnimation.Play();
                    }
                    else
                    {
                        var button = GameManager.InputManager.GetPadAxisButtonName(InputSystem.DroneAxes[5]);
                        PrintMessage("Press " + button + " to turn on the drone in " + drone.PowerDownTimer);
                    }

                    NoSignalAnimation.SetImageAlpha(0);
                    if (drone.DistanceFromShipPercentage > 0.8f)
                    {
                        PrintMessage("Warning Losing Connection");
                        var a = Utility.Interpolate(0, 1, 0.8f, 1, drone.DistanceFromShipPercentage);
                        NoSignalAnimation.SetImageAlpha(a);
                        NoSignalAnimation.Play();
                    }    
                }

                if (HitImage)
                {
                    HitImage.color = Color.Lerp(HitImage.color, Color.clear, 5 * Time.deltaTime);
                }

                if (DamageImages != null)
                {
                    if (DamageImages.Length > 0)
                    {
                        var ratio = 1 / (float)DamageImages.Length;
                        var lastStop = 1f;
                        for (int i = 0; i < DamageImages.Length; i++)
                        {
                            if (DamageImages[i])
                            {
                                var color = DamageImages[i].color;
                                var stop = 1 - (ratio * (i + 0.5f));
                                color.a = Utility.Interpolate(0, 1, lastStop, stop, userCharacter.HealthPercentage);
                                DamageImages[i].color = color;
                                lastStop = stop;
                            }
                        }
                    }
                }

                if (!userCharacter.Alive) userCharacter.UnRegisterHitCallback(Hit);

                if (Ship.Instance.DroneInPickUp && userCharacter.Possession.Length > 0)
                {
                    var button = GameManager.InputManager.GetPadAxisButtonName(InputSystem.ShipAxes[1]);
                    PrintMessage("Press " + button + " to move items into the ship");
                }
                else if (userCharacter.NearItem)
                {
                    var button = GameManager.InputManager.GetPadAxisButtonName(InputSystem.DroneAxes[6]);
                    PrintMessage("Press " + button + " to pick up the item");
                }

                if (ItemsText)
                {
                    ItemsText.text = "Ship has " + Ship.Instance.CollectedItems() + "/" + Ship.Instance.ItemsNeeded.Length + " Items" + "\n";
                    ItemsText.text += "Drone has " + userCharacter.Possession.Length + " Items";
                }
            }
        }

        private void ShipHUD()
        {
            if (!userCharacter && !GameManager.GamePaused)
            {
                EnablePanel(ShipPanel);

                var counter = Ship.Instance.DronesLeft;

                if (ShipCounterText)
                {
                    ShipCounterText.text = counter + " Drone(s) Remaining";
                }

                if (counter > 0)
                {
                    var button = GameManager.InputManager.GetPadAxisButtonName(InputSystem.ShipAxes[0]);
                    PrintMessage("Press " + button + " to deploy a drone");
                }
            }
        }

        private void SetMiniMap()
        {
            if (!miniMapCamera) CreateMiniMap();

            if (userCharacter)
            {
                miniMapCamera.transform.position = Vector3.Lerp(miniMapCamera.transform.position, userCharacter.transform.position + Vector3.up * MiniMapHeightOffset, 20 * Time.deltaTime);
                miniMapCamera.transform.rotation = Quaternion.LookRotation(Vector3.down);
                if (userCharacter.Authority) miniMapCamera.transform.SetParent(userCharacter.Authority.transform);
            }

            miniMapCamera.cullingMask = MiniMapMask;
            miniMapCamera.orthographic = true;
            miniMapCamera.orthographicSize = MiniMapCameraSize;

            if (MiniMapImage)
            {
                MiniMapImage.texture = miniMapTexture;
            }
        }

        private void SetCanvas()
        {
            if (userCamera)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = userCamera;
                canvas.planeDistance = userCamera.nearClipPlane + 0.001f;
            }
        }

        private void SetButtons()
        {
            RestartButton.onClick.RemoveAllListeners();
            EndGameButton.onClick.RemoveAllListeners();

            RestartButton.onClick.AddListener(GameManager.RestartGame);
            EndGameButton.onClick.AddListener(delegate { GameManager.EndGame(); GameManager.LoadMainMenu(); });
        }

        private void Hit()
        {
            if (HitImage)
            {
                HitImage.color = HitColor;
            }
        }

        private void PrintMessage(string message)
        {
            if (DroneMessageText)
            {
                DroneMessageText.enabled = message != "";
                DroneMessageText.text = message;
            }

            if (ShipMessageText)
            {
                ShipMessageText.enabled = message != "";
                ShipMessageText.text = message;
            }
            
            messageTimer = message == "" ? 0 : MessageDisplayTime;
        }
        
        private RectTransform NewItemLabel(string labelText, Sprite sprite)
        {
            var label = Instantiate(ItemsUIPrefab, ItemsUIParent);
            label.gameObject.SetActive(true);
            label.GetComponentInChildren<Text>().text = labelText;
            label.GetComponentInChildren<Image>().sprite = sprite;
            return label;
        }
    }
}