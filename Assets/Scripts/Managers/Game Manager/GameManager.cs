using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using PadInput;
using PII.Utilities;

namespace PII
{
    public enum Progress
    {
        none,
        Playing,
        Lost,
        Won
    }

    [RequireComponent(typeof(MusicManager))]
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GamePrefabs Prefabs;
        [SerializeField] private AudioMixer EnvironmentMixer;
        [SerializeField] private AudioMixer MusicMixer;

        private static bool loading;
        private static bool gameStarted;
        private static bool gamePaused;
        private static GameManager instance;
        private static PadManager inputManager;
        private static MusicManager musicManager;
        private static UserController user;

        public static bool Loading { get { return loading; } }
        public static bool GameStarted { get { return gameStarted; } }
        public static bool GamePaused { get { return gamePaused; } }
        public static PadManager InputManager
        {
            get
            {
                if (!inputManager)
                {
                    GetNewInputManager();
                }

                return inputManager;
            }
        }
        public static UserController User { get { return user; } }
        public static float EnvironmentVolume { get; set; }
        public static float MusicVolume { get; set; }
        public static Callback StartGameCallback { get; set; }
        public static Callback PauseCallback { get; set; }
        public static Callback UnPauseCallback { get; set; }
        public static Callback EndGameCallback { get; set; }
        public static float SceneLoadProgess { get; private set; }
        public static Progress UserProgress { get; private set; }

        private void Awake()
        {
            if (!instance)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                musicManager = GetComponent<MusicManager>();
            }
            
            if (this != instance) DestroyImmediate(this);

            EnvironmentVolume = 1;
            MusicVolume = 0.8f;
        }

        private void Update()
        {
            EnvironmentMixer.SetFloat("MasterVolume", GetDBVolume(EnvironmentVolume));
            MusicMixer.SetFloat("MasterVolume", GetDBVolume(MusicVolume));

            if (GameStarted)
            {
                if (Ship.Instance.CollectedAllItems())
                {
                    UserProgress = Progress.Won;
                    PauseGame();
                }
                else if(!Ship.Instance.HasDrone)
                {
                    UserProgress = Progress.Lost;
                    PauseGame();
                }
                else
                {
                    UserProgress = Progress.Playing;
                }

                if (InputManager.GetAxisPressed(InputSystem.PauseAxes) && UserProgress == Progress.Playing)
                {
                    if (GamePaused)
                    {
                        UnPauseGame();
                    }
                    else
                    {
                        PauseGame();
                    }
                }
            }
        }

        private float GetDBVolume(float scale)
        {
            return Utility.Interpolate(-80, 0, 0, 1, scale);
        }

        public static void LoadMainScene()
        {
            instance.StartCoroutine(LoadScene(1, StartGame));
        }

        public static void LoadMainMenu()
        {
            instance.StartCoroutine(LoadScene(0, null));
        }

        /// <summary>
        /// Starts the game
        /// </summary>
        public static void StartGame()
        {
            gameStarted = true;
            UserProgress = Progress.Playing;

            UnPauseGame();

            InputSystem.onButtonChanged += GetNewInputManager;
            if (StartGameCallback != null) StartGameCallback();

            SpawnUser();
        }

        public static void RestartGame()
        {
            if (!GameStarted) return;

            instance.StartCoroutine(Restart());
        }

        public static void PauseGame()
        {
            Time.timeScale = 0;
            gamePaused = true;

            if (PauseCallback != null) PauseCallback();
        }

        public static void UnPauseGame()
        {
            Time.timeScale = 1;
            gamePaused = false;

            if (UnPauseCallback != null) UnPauseCallback();
            
            musicManager.PlayRandomClip();
        }

        /// <summary>
        /// Ends the game
        /// </summary>
        public static void EndGame()
        {
            gameStarted = false;
            UserProgress = Progress.none;

            Controller.DestroyAllControllers();
            Character.DestroyAllCharacters();
            Item.DestroyAllItems();

            UnPauseGame();
            GetNewInputManager();
            
            if (EndGameCallback != null) EndGameCallback();
        }
        
        /// <summary>
        /// Quits the game application
        /// </summary>
        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif

            Application.Quit();
        }

        /// <summary>
        /// Creates a new user if there isn't a user in the scene
        /// </summary>
        /// <returns></returns>
        public static UserController SpawnUser()
        {
            if (user || !GameStarted) return null;
            
            return user = Instantiate(instance.Prefabs.User);
        }
        
        /// <summary>
        /// Creates a new AI controller
        /// </summary>
        /// <returns></returns>
        public static AIController SpawnAI()
        {
            if (!GameStarted) return null;

            return Instantiate(instance.Prefabs.AI);
        }
        
        /// <summary>
        /// Creates a new drone in the scene
        /// </summary>
        /// <param name="position">The position of the drone</param>
        /// <returns></returns>
        public static Drone SpawnDrone(Vector3 position)
        {
            if (!GameStarted) return null;

            return Instantiate(instance.Prefabs.Drone, position, Quaternion.identity);
        }

        /// <summary>
        /// Creates a new Human in the scene
        /// </summary>
        /// <param name="position">The position of the human</param>
        /// <param name="index">Which human you want</param>
        /// <returns></returns>
        public static Human SpawnHuman(Vector3 position)
        {
            if (!GameStarted || instance.Prefabs.Humans.Length < 1) return null;

            var index = Random.Range(0, instance.Prefabs.Humans.Length - 1);
            return Instantiate(instance.Prefabs.Humans[index], position, Quaternion.identity);
        }
        
        private static IEnumerator LoadScene(int sceneIndex, Callback callback)
        {
            AsyncOperation asyn = SceneManager.LoadSceneAsync(sceneIndex);
            loading = true;
            musicManager.StopPlaying();
            while (!asyn.isDone)
            {
                SceneLoadProgess = asyn.progress;
                yield return new WaitForEndOfFrame();
            }
            loading = false;
            musicManager.PlayRandomClip();
            SceneLoadProgess = 0;

            var timer = 0f;
            while (timer < 5)
            {
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            if (callback != null) callback(); 
        }
        
        private static IEnumerator Restart()
        {
            EndGame();

            var timer = 0f;
            while (timer < 3)
            {
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            StartGame();
        }

        private static void GetNewInputManager()
        {
            if (inputManager)
            {
                Destroy(inputManager.gameObject);
                inputManager = null;
            }

            if (gameStarted)
            {
                inputManager = InputSystem.GetInputManager();
                inputManager.gameObject.transform.SetParent(instance.gameObject.transform);
            }
        }

        public delegate void Callback();

        public GameManager()
        {
            
        }
    }

    [System.Serializable]
    public struct GamePrefabs
    {
        public UserController User;
        public AIController AI;
        public Drone Drone;
        public Human[] Humans;
    }
}