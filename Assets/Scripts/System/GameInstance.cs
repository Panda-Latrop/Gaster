using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

public  class GameInstance : Singleton<GameInstance>
{
    [System.Serializable]
    public struct GlobalStruct
    {
        public string key;
        public int value;
    }
    protected static bool applicationIsQuit;
    protected static bool changeScene;
    public static string language = "rus";
    public static bool ApplicationIsQuit => applicationIsQuit;
    public static bool ChangeScene => changeScene;

    protected SaveSystem saveSystem = new SaveSystem();
    protected PoolManager poolManager;
    protected GameStateBase gameState;
    protected PlayerStart playerStart;
    protected PlayerControllerBase playerController;
    protected Navigation navigation;
    protected UISystem uiSystem;
    protected DialogSystem dialog;
    protected CinemaSystem cinema;
    protected MusicSystem musicSystem;
    protected GameObject uiContainer;
    protected CameraActor mainCamera;

    protected Dictionary<string, int> global = new Dictionary<string, int>();

    protected string level;
    protected int levelEnter = 0;
    protected int levelLoadState = NONE;
    protected const int NONE = 0, LOAD = 1, NEXT = 2;

    public GameStateBase GameState  => gameState; 
    public PlayerControllerBase PlayerController => playerController; 
    public PoolManager PoolManager  => poolManager; 
    public Navigation Navigation => navigation;
    public UISystem UISystem => uiSystem;
    public DialogSystem Dialog => dialog;
    public CinemaSystem Cinema => cinema;
    public MusicSystem MusicSystem => musicSystem;
    public CameraActor Camera => mainCamera;
    public Dictionary<string, int> Global => global;

    public bool GlobalCheck(string key, int value)
    {
        if (global.ContainsKey(key))
            return global[key] == value;
        else
        {
            global.Add(key, 0);
            if (value == 0)
                return true;
            else
                return false;    
        }    
    }
    public bool GlobalCheck(GlobalStruct global)
    {
        return GlobalCheck(global.key, global.value);
    }
    public void GlobalSet(string key, int value)
    {
        if (global.ContainsKey(key))
            global[key] = value;
        else
            global.Add(key, value);
    }
    public void GlobalSet(GlobalStruct global)
    {
        GlobalSet(global.key, global.value);
    }
        public void GlobalAdd(string key, int value)
    {
        if (global.ContainsKey(key))
            global[key] = value + global[key];
        else
            global.Add(key, value);
    }
    public void GlobalAdd(GlobalStruct global)
    {
        GlobalAdd(global.key, global.value);
    }
    [ContextMenu("Log Global")]
    private void GlobalLog()
    {
        StringBuilder strb = new StringBuilder();
        strb.Append("\nGlobal\n");
        foreach (var item in global)
        {
            strb.Append("(Key: ").Append(item.Key).Append(", Value: ").Append(item.Value.ToString()).Append(")\n");
        }
        Debug.Log(strb.ToString());
    }
    protected override void Awake()
    {
        bool bindOnSceneLoaded = (Instance == null);
        base.Awake();
        if(bindOnSceneLoaded)
        SceneManager.sceneLoaded += OnSceneLoaded;

    }
    protected void Create()
    {

        GameObject gogm = GameObject.FindGameObjectWithTag("GameController");
        GameObject gops = GameObject.FindGameObjectWithTag("Respawn");
        if (gogm != null && gops != null)
        {
            GameModeBase gameMode = gogm.GetComponent<GameModeBase>();
            gameState = Instantiate(gameMode.GameState, gameMode.transform);

            playerStart = gops.GetComponent<PlayerStart>();
            playerController = Instantiate(gameMode.PlayerController, gameMode.transform);
            playerController.enabled = true;

            Pawn playerPawn = Instantiate(gameMode.PlayerPawn);
            playerController.Possess(playerPawn);

            navigation = gameMode.Navigation;

            dialog = gameMode.Dialog;

            cinema = gameMode.Cinema;

            uiSystem = gameMode.UI;

            mainCamera = gameMode.MainCamera;

            if (musicSystem == null)
            {
                GameObject goms = new GameObject("MusicSystem");
                musicSystem = goms.AddComponent(typeof(MusicSystem)) as MusicSystem;
                AudioSource audio = goms.AddComponent(typeof(AudioSource)) as AudioSource;
                musicSystem.SetAudioSource(audio, gameMode.MusicHolder.GetMixerGroup());
                musicSystem.SetMusicHolder(gameMode.MusicHolder);
                DontDestroyOnLoad(goms);
                goms.transform.SetParent(transform);
            }
            else
            {
                musicSystem.SetMusicHolder(gameMode.MusicHolder);
            }

            if (levelLoadState == NONE)
            {
                Vector3 position;Quaternion rotation;playerStart.GetPoint(0,out position,out rotation);
                playerController.SetStart(position, rotation);
                mainCamera.Movement.Teleport(position);
            }
        }
        else
        {
            if (gogm != null)
                Debug.LogError("GameMode Not Found");
            if (gops != null)
                Debug.LogError("PlayerStart Not Found");
            Debug.Break();
        }

        GameObject gopm = new GameObject("PoolManager");
        poolManager = gopm.AddComponent(typeof(PoolManager)) as PoolManager;
        gopm.transform.SetParent(gogm.transform);
    }
   
    [ContextMenu("Save")]
    public void InitiateSaveGame()
    {
        saveSystem.Data.save = level;
        saveSystem.Prepare(false).
                   SaveTo("system", true, "system").
                   SaveTo(level).
                   DataTo("save.sv").
                   Close();
    }
    [ContextMenu("Load")]
    public void InitiateLoadGame()
    {
        saveSystem.DataFrom("save.sv");
        LoadScene(saveSystem.Data.save, 0, LOAD);
        
    }
    public bool GetLoadedObject(string name, ref GameObject go)
    {
        return saveSystem.GetLoadedObject(name, ref go);
    }
    public void LoadScene(string name,int enter, int state = NEXT)
    {
        changeScene = true;
        levelLoadState = state;
        levelEnter = enter;
        switch (levelLoadState)
        {
            case NONE:
                break;
            case LOAD:
                break;
            case NEXT:
                saveSystem.Prepare(false).
                           SaveTo(level, true, "between").
                           SaveTo("next",true,"next").
                           Close();
                break;
            default:
                break;
        }       
        SceneManager.LoadScene(name.ToLower(), LoadSceneMode.Single);
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        level = scene.name.ToLower();
        changeScene = false;
        Create();
        switch (levelLoadState)
        {
            case NONE:
                break;
            case LOAD:
                saveSystem.Prepare(true).
                           LoadFrom("system", true, "system").
                           LoadFrom(level).
                           Close();
                mainCamera.Movement.Teleport(playerController.ControlledPawn.transform.position);
                break;
            case NEXT:
                saveSystem.Prepare(true).
                           LoadFrom(level, true,"between").
                           LoadFrom("next", true, "next").
                           Close();
                Vector3 position; Quaternion rotation; playerStart.GetPoint(levelEnter, out position, out rotation);
                playerController.SetStart(position, rotation);
                mainCamera.Movement.Teleport(position);
                break;
            default:
                break;
        }
    }



    protected void OnApplicationQuit()
    {
        applicationIsQuit = true;
    }
}
