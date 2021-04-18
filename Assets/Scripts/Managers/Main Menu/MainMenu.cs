using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using PII.Utilities;
using PadInput;
using UIAnimation;

namespace PII
{
    [RequireComponent(typeof(Canvas))]
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Font MainFont;
        [SerializeField] private RectTransform NamePanel;
        [SerializeField] private RectTransform LoadingPanel;
        [SerializeField] private RectTransform MessengerPanel;
        [SerializeField] private RectTransform IntroTextPanel;
        [SerializeField] private TextAnimation IntroTextAnimation;
        [SerializeField] private AudioMixerGroup Mixer;
        [SerializeField] private AudioClip ButtonHighlightedClip;
        [SerializeField] private AudioClip ButtonClickClip;

        [Header("Main Panel")]
        [SerializeField]private RectTransform MainPanel;
        [SerializeField] private Button PlayButton;
        [SerializeField] private Button OptionsButton;
        [SerializeField] private Button CreditButton;
        [SerializeField] private Button QuitButton;

        [Header("Options Panel")]
        [SerializeField] private RectTransform OptionsPanel;
        [SerializeField] private RectTransform OptionsControlsPanel;
        [SerializeField] private Button OptionsControlsButton;
        [SerializeField] private RectTransform ControlsNamesPanel;
        [SerializeField] private RectTransform ControlsKMPanel;
        [SerializeField] private RectTransform ControlsCPanel;
        [SerializeField] private Button ControlsDefaultButton;
        [SerializeField] private RectTransform OptionsGraphicsPanel;
        [SerializeField] private Button OptionsGraphicsButton;
        [SerializeField] private SlideMenu GraphicsWindowMenu;
        [SerializeField] private SlideMenu GraphicsRefreshRateMenu;
        [SerializeField] private SlideMenu GraphicsResolutionMenu;
        [SerializeField] private SlideMenu GraphicsQualityMenu;
        [SerializeField] private Button GraphicsApplyButton;
        [SerializeField] private RectTransform OptionsAudioPanel;
        [SerializeField] private Button OptionsAudioButton;
        [SerializeField] private Slider MasterVolumeSlider;
        [SerializeField] private Slider EnvironmentVolumeSlider;
        [SerializeField] private Slider MusicVolumeSlider;
        [SerializeField] private Button BackOptionsButton;

        [Header("Credit Panel")]
        [SerializeField] private RectTransform CreditPanel;
        [SerializeField] private Button BackCreditButton;

        [Header("Prefabs")]
        [SerializeField] private RectTransform LabelPrefab;
        [SerializeField] private Button ButtonPrefab;

        private List<RectTransform> mainPanels = new List<RectTransform>();
        private List<RectTransform> optionsPanels = new List<RectTransform>();
        private StandaloneInputModule inputModule;
        private AudioSource audioSource;

        private bool showMenu;
        private bool playIntro;
        private bool listeningForInput;
        private bool KMorC;
        private string input;
        private int axis;
        private float timer;
        
        public void PlayButtonHighlightedClip()
        {
            if (!ButtonHighlightedClip) return;

            audioSource.clip = ButtonHighlightedClip;
            audioSource.loop = false;
            audioSource.Play();
        }

        public void PlayButtonClickClip()
        {
            if (!ButtonClickClip) return;

            audioSource.clip = ButtonClickClip;
            audioSource.loop = false;
            audioSource.Play();
        }

        private void Start()
        {
            GetAudioSource();

            RegisterMainPanel(MainPanel);
            RegisterMainPanel(OptionsPanel);
            RegisterMainPanel(CreditPanel);

            RegisterOptionsPanel(OptionsControlsPanel);
            RegisterOptionsPanel(OptionsGraphicsPanel);
            RegisterOptionsPanel(OptionsAudioPanel);

            EnableMainPanel(null);
            showMenu = false;
            playIntro = false;
            listeningForInput = false;
            InputSystem.onButtonChanged += ShowOptionsControlsPanel;

            MasterVolumeSlider.minValue = 0;
            MasterVolumeSlider.maxValue = 1;
            MasterVolumeSlider.value = AudioListener.volume;

            EnvironmentVolumeSlider.minValue = 0;
            EnvironmentVolumeSlider.maxValue = 1;
            EnvironmentVolumeSlider.value = GameManager.EnvironmentVolume;

            MusicVolumeSlider.minValue = 0;
            MusicVolumeSlider.maxValue = 1;
            MusicVolumeSlider.value = GameManager.MusicVolume;
        }
        
        private void GetAudioSource()
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = false;
            audioSource.spatialBlend = 0;
            audioSource.outputAudioMixerGroup = Mixer;
        }

        private void RegisterMainPanel(RectTransform panel)
        {
            if (!panel) return;

            mainPanels.Add(panel);
        }

        private void EnableMainPanel(RectTransform panel)
        {
            for (int i = 0; i < mainPanels.Count; i++)
            {
                mainPanels[i].gameObject.SetActive(panel == mainPanels[i]);
            }
        }

        private void RegisterOptionsPanel(RectTransform panel)
        {
            if (!panel) return;

            optionsPanels.Add(panel);
        }

        private void EnableOptionsPanel(RectTransform panel)
        {
            for (int i = 0; i < optionsPanels.Count; i++)
            {
                optionsPanels[i].gameObject.SetActive(panel == optionsPanels[i]);
            }
        }

        private void Update()
        {
            foreach (var text in GetComponentsInChildren<Text>())
            {
                text.font = MainFont;
            }

            if (GameManager.Loading)
            {
                EnableMainPanel(null);
                NamePanel.gameObject.SetActive(false);
                IntroTextPanel.gameObject.SetActive(false);
                LoadingPanel.gameObject.SetActive(true);
                PrintMessage("");
                return;
            }

            IntroTextPanel.gameObject.SetActive(playIntro);
            NamePanel.gameObject.SetActive(!playIntro);
            LoadingPanel.gameObject.SetActive(false);
            
            if (!showMenu)
            {
                PrintMessage("Press Any Button");

                if (Input.anyKey)
                {
                    ShowMainPanel();
                    showMenu = true;
                }
            }
            
            Listen();

            SetAudio();

            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                PrintMessage("");
            }

            if (playIntro)
            {
                EnableMainPanel(null);
                if (!IntroTextAnimation.Playing || Pad.GetKeyboardPressed(KeyboardCode.Escape) || Pad.GetControllerPressed(ControllerCode.Start, ControllerIndex.Any)) GameManager.LoadMainScene();
            }
        }

        private void ShowMainPanel()
        {
            // show panel
            EnableMainPanel(MainPanel);
            
            // clear handlers
            PlayButton.onClick.RemoveAllListeners();
            OptionsButton.onClick.RemoveAllListeners();
            OptionsButton.onClick.RemoveAllListeners();
            QuitButton.onClick.RemoveAllListeners();

            // register handlers
            PlayButton.onClick.AddListener(delegate { StartCoroutine(IntroTextAnimation.Play()); playIntro = true;  PlayButtonClickClip(); });
            OptionsButton.onClick.AddListener(delegate { ShowOptionsPanel(); PlayButtonClickClip(); });
            CreditButton.onClick.AddListener(delegate { ShowCreditPanel(); PlayButtonClickClip(); });
            QuitButton.onClick.AddListener(delegate { GameManager.Quit(); PlayButtonClickClip(); });

            EventSystem.current.firstSelectedGameObject = PlayButton.gameObject;
        }
        
        private void ShowOptionsPanel()
        {
            // show panel
            EnableMainPanel(OptionsPanel);

            // clear handlers
            OptionsControlsButton.onClick.RemoveAllListeners();
            OptionsGraphicsButton.onClick.RemoveAllListeners();
            OptionsAudioButton.onClick.RemoveAllListeners();
            BackOptionsButton.onClick.RemoveAllListeners();
            // register handlers
            OptionsControlsButton.onClick.AddListener(delegate { ShowOptionsControlsPanel(); PlayButtonClickClip(); });
            OptionsGraphicsButton.onClick.AddListener(delegate { ShowOptionsGraphicsPanel(); PlayButtonClickClip(); });
            OptionsAudioButton.onClick.AddListener(delegate { ShowOptionsAudioPanel(); PlayButtonClickClip(); });
            BackOptionsButton.onClick.AddListener(delegate { ShowMainPanel(); PlayButtonClickClip(); });

            ShowOptionsControlsPanel();

            EventSystem.current.firstSelectedGameObject = OptionsControlsButton.gameObject;
        }

        private void ShowOptionsControlsPanel()
        {
            EnableOptionsPanel(OptionsControlsPanel);
            RemoveAllChildren(ControlsNamesPanel);
            RemoveAllChildren(ControlsKMPanel);
            RemoveAllChildren(ControlsCPanel);

            NewLabel(ControlsNamesPanel, "Pause");
            NewLabel(ControlsKMPanel, "Escape");
            NewLabel(ControlsCPanel, "Start");

            var kCameraButtons = InputSystem.GetKMCameraButtons();
            var cCameraButtons = InputSystem.GetCCameraButtons();
            for (int i = 0; i < InputSystem.CameraAxes.Length; i++)
            {
                NewLabel(ControlsNamesPanel, InputSystem.CameraAxes[i]);
                var kmButton = NewButton(ControlsKMPanel, Pad.GetCodeName(kCameraButtons[i]));
                var cButton = NewButton(ControlsCPanel, Pad.GetCodeName(cCameraButtons[i]));
                var index = i;
                kmButton.onClick.AddListener(delegate { StartListeningForInput("Camera", index, true, kmButton.transform); PlayButtonClickClip(); });
                cButton.onClick.AddListener(delegate { StartListeningForInput("Camera", index, false, cButton.transform); PlayButtonClickClip(); });
            }

            var kDroneButtons = InputSystem.GetKMDroneButtons();
            var cDroneButtons = InputSystem.GetCDroneButtons();
            for (int i = 0; i < InputSystem.DroneAxes.Length; i++)
            {
                NewLabel(ControlsNamesPanel, InputSystem.DroneAxes[i]);
                var kmButton = NewButton(ControlsKMPanel, Pad.GetCodeName(kDroneButtons[i]));
                var cButton = NewButton(ControlsCPanel, Pad.GetCodeName(cDroneButtons[i]));
                var index = i;
                kmButton.onClick.AddListener(delegate { StartListeningForInput("Drone", index, true, kmButton.transform); PlayButtonClickClip(); });
                cButton.onClick.AddListener(delegate { StartListeningForInput("Drone", index, false, cButton.transform); PlayButtonClickClip(); });
            }

            var kShipButtons = InputSystem.GetKMShipButtons();
            var cShipButtons = InputSystem.GetCShipButtons();
            for (int i = 0; i < InputSystem.ShipAxes.Length; i++)
            {
                NewLabel(ControlsNamesPanel, InputSystem.ShipAxes[i]);
                var kmButton = NewButton(ControlsKMPanel, Pad.GetCodeName(kShipButtons[i]));
                var cButton = NewButton(ControlsCPanel, Pad.GetCodeName(cShipButtons[i]));
                var index = i;
                kmButton.onClick.AddListener(delegate { StartListeningForInput("Ship", index, true, kmButton.transform); PlayButtonClickClip(); });
                cButton.onClick.AddListener(delegate { StartListeningForInput("Ship", index, false, cButton.transform); PlayButtonClickClip(); });
            }

            ControlsDefaultButton.onClick.RemoveAllListeners();
            ControlsDefaultButton.onClick.AddListener(delegate{ InputSystem.SetAllButtonsToDefaults(); PlayButtonClickClip(); });
        }

        private void ShowOptionsGraphicsPanel()
        {
            EnableOptionsPanel(OptionsGraphicsPanel);

            GraphicsApplyButton.onClick.RemoveAllListeners();
            GraphicsWindowMenu.UnRegisterHandlers();
            GraphicsRefreshRateMenu.UnRegisterHandlers();
            GraphicsResolutionMenu.UnRegisterHandlers();
            GraphicsQualityMenu.UnRegisterHandlers();

            GraphicsWindowMenu.RegisterHandlers(PlayButtonClickClip);
            GraphicsRefreshRateMenu.RegisterHandlers(PlayButtonClickClip);
            GraphicsResolutionMenu.RegisterHandlers(PlayButtonClickClip);
            GraphicsQualityMenu.RegisterHandlers(PlayButtonClickClip);

            var window = new string[] { "FullScreen", "Windowed" };
            GraphicsWindowMenu.SetValues(window);
            GraphicsWindowMenu.SetValue(Screen.fullScreen ? 0 : 1);

            var rate = new string[] { Screen.currentResolution.refreshRate.ToString() + " Hz" };
            GraphicsRefreshRateMenu.SetValues(rate);
            GraphicsRefreshRateMenu.SetValue(0);

            var reses = Screen.resolutions;
            var resolutions = new string[reses.Length];
            var resolution = 0;

            for (int i = 0; i < reses.Length; i++)
            {
                resolutions[i] = reses[i].width + "x" + reses[i].height;
                resolution = ResolutionEquals(Screen.currentResolution, reses[i]) ? i : resolution; 
            }

            GraphicsResolutionMenu.SetValues(resolutions);
            GraphicsResolutionMenu.SetValue(resolution);
            
            GraphicsQualityMenu.SetValues(QualitySettings.names);
            GraphicsQualityMenu.SetValue(QualitySettings.GetQualityLevel());

            GraphicsWindowMenu.SetText();
            GraphicsRefreshRateMenu.SetText();
            GraphicsResolutionMenu.SetText();
            GraphicsQualityMenu.SetText();

            GraphicsApplyButton.onClick.AddListener(delegate { ApplyGraphics(); PlayButtonClickClip(); });
        }

        private void ApplyGraphics()
        {
            QualitySettings.SetQualityLevel(GraphicsQualityMenu.Value, true);

            var fullscreen = GraphicsWindowMenu.Value == 0;
            var resolution = Screen.resolutions[GraphicsResolutionMenu.Value];
            Screen.SetResolution(resolution.width, resolution.height,fullscreen);
        }

        private void ShowOptionsAudioPanel()
        {
            EnableOptionsPanel(OptionsAudioPanel);
        }

        private void SetAudio()
        {
            MasterVolumeSlider.minValue = 0;
            MasterVolumeSlider.maxValue = 1;
            AudioListener.volume = MasterVolumeSlider.value;

            EnvironmentVolumeSlider.minValue = 0;
            EnvironmentVolumeSlider.maxValue = 1;
            GameManager.EnvironmentVolume = EnvironmentVolumeSlider.value;

            MusicVolumeSlider.minValue = 0;
            MusicVolumeSlider.maxValue = 1;
            GameManager.MusicVolume = MusicVolumeSlider.value;
        }

        private void ShowCreditPanel()
        {
            // show panel
            EnableMainPanel(CreditPanel);

            // clear handlers
            BackCreditButton.onClick.RemoveAllListeners();
            // register handlers
            //delegate { (); PlayButtonClickClip(); }
            BackCreditButton.onClick.AddListener(delegate { ShowMainPanel(); PlayButtonClickClip(); });
            
            EventSystem.current.firstSelectedGameObject = BackCreditButton.gameObject;
        }

        private void PrintMessage(string message)
        {
            MessengerPanel.gameObject.SetActive(message != "");
            MessengerPanel.GetComponentInChildren<Text>().text = message;
            timer = message == "" ? 0 : 0.1f;
        }
        
        private void RemoveAllChildren(RectTransform panel)
        {
            for (int i = 0; i < panel.childCount; i++)
            {
                Destroy(panel.GetChild(i).gameObject);
            }
        }

        private RectTransform NewLabel(RectTransform parent, string labelText)
        {
            var label = Instantiate(LabelPrefab, parent);
            label.gameObject.SetActive(true);
            label.GetComponentInChildren<Text>().text = labelText;
            return label;
        }

        private Button NewButton(RectTransform parent, string labelText)
        {
            var button = Instantiate(ButtonPrefab, parent);
            button.gameObject.SetActive(true);
            button.GetComponentInChildren<Text>().text = labelText;
            return button;
        }

        private void StartListeningForInput(string input, int axis, bool KMorC, Transform transform)
        {
            if (listeningForInput)
                return;

            listeningForInput = true;
            this.input = input;
            this.axis = axis;
            this.KMorC = KMorC;
            RemoveAllChildren(transform.GetComponent<RectTransform>());
        }

        private void Listen()
        {
            if (listeningForInput)
            {
                if (Pad.GetKeyboardPressed(KeyboardCode.Escape) || Pad.GetControllerPressed(ControllerCode.Start, ControllerIndex.Any))
                {
                    listeningForInput = false;
                    ShowOptionsControlsPanel();
                    return;
                }

                var extra1 = input == "Camera" ? InputSystem.CameraAxes[axis] : (input == "Drone" ? InputSystem.DroneAxes[axis] : (input == "Ship" ? InputSystem.ShipAxes[axis] : ""));
                var extra2 = KMorC ? "Keyboard/Mouse" : "Controller";
                PrintMessage("Assign " + extra2 + " button to " + extra1);
                PadCode code;

                if (KMorC)
                {
                    if (Pad.ListenForInputFrom(InputSource.Keyboard, out code) || Pad.ListenForInputFrom(InputSource.Mouse, out code))
                    {
                        switch (input)
                        {
                            case "Camera":
                                InputSystem.ChangeKMCameraButton(axis, code);
                                break;
                            case "Drone":
                                InputSystem.ChangeKMDroneButton(axis, code);
                                break;
                            case "Ship":
                                InputSystem.ChangeKMShipButton(axis, code);
                                break;
                        }
                        ResetListening();
                    }
                }
                else
                {
                    if (Pad.ListenForInputFromController(ControllerIndex.Any, out code))
                    {
                        switch (input)
                        {
                            case "Camera":
                                InputSystem.ChangeCCameraButton(axis, code);
                                break;
                            case "Drone":
                                InputSystem.ChangeCDroneButton(axis, code);
                                break;
                            case "Ship":
                                InputSystem.ChangeCShipButton(axis, code);
                                break;
                        }
                        ResetListening();
                    }
                }
            }
        }

        private void ResetListening()
        {
            listeningForInput = false;
            input = "";
            axis = -1;
            KMorC = true;
        }

        [System.Serializable]
        public class SlideMenu
        {
            public Text Text;
            public Button LeftButton;
            public Button RightButton;

            private string[] values;
            private int index;

            public int Value { get { return index; } }

            public void SetValues(string[] values)
            {
                this.values = values;
            }
            
            public void SetValue(int value)
            {
                index = value;
            }

            public void SetText()
            {
                if (Text)
                {
                    Text.text = values != null ? values[index] : "";
                }
            }

            public void RegisterHandlers(Callback callback = null)
            {
                if (LeftButton)
                {
                    LeftButton.onClick.AddListener(delegate { OnLeftClick(); if (callback != null)callback(); });
                }

                if (RightButton)
                {
                    RightButton.onClick.AddListener(delegate { OnRightClick(); if (callback != null) callback(); });
                }
            }

            public void UnRegisterHandlers()
            {
                if (LeftButton)
                {
                    LeftButton.onClick.RemoveAllListeners();
                }

                if (RightButton)
                {
                    RightButton.onClick.RemoveAllListeners();
                }
            }

            private void OnLeftClick()
            {
                if (values != null)
                {
                    index = index > 0 ? index - 1 : values.Length - 1;
                }

                SetText();
            }

            private void OnRightClick()
            {
                if (values != null)
                {
                    index = index < values.Length - 1 ? index + 1 : 0;
                }

                SetText();
            }

            public delegate void Callback();
        }

        private static bool ResolutionEquals(Resolution a, Resolution b)
        {
            return a.width == b.width && a.height == b.height;
        }
    }
}