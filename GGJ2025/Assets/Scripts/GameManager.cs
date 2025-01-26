using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager sInstance;

    public Vector3[] PlayerSpawnpoints;

    int NumberOfConnectedPlayers = 0;

    public List<Player> Players = new List<Player>();
    public List<PlayerData> Playerdatas = new List<PlayerData>();

    List<PlayerController> PlayerControllers = new List<PlayerController>();
    public DeviceDisplayer deviceDisplayerPrefab;
    public GameObject deviceDisplayGrid;

    bool tryStartGame = false;

    public float MatchTime = 70;
    bool isGameActive = false;
    bool hasMusicfadeStarted = false;

    public float MinTimeInScene = 2;

    TextMeshProUGUI GameTimerText;
    TextMeshProUGUI GameOverText;
    GameObject GameOverTitleGrid;
    FadeMusic musicFader;

    public enum GameState
    {
        Undefined = -1,
        MainMenu = 0,
        PlayerSelect = 1,
        Game = 2,
        Highscore = 3,
        Credits = 4
    }

    public GameState currentGameState = GameState.Undefined;

    void Awake()
    {
        if (sInstance == null)
        {
            sInstance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
        OnSceneLoaded(SceneManager.GetActiveScene(), new LoadSceneMode());
    }

    void SpawnNewPlayer(PlayerData data)
    {
        GameObject current = Instantiate(data.PlayerPrefab, PlayerSpawnpoints[Players.Count], Quaternion.identity);
        data.index = Players.Count;
        Player currentPlayer = current.GetComponent<Player>();
        currentPlayer.data = data;
        Players.Add(currentPlayer);
    }

    public void OnPlayerJoined(PlayerController playerController)
    {
        playerController.transform.SetParent(transform);
        PlayerControllers.Add(playerController);
        var go = Instantiate(deviceDisplayerPrefab, deviceDisplayGrid.transform);
        go.GetComponent<DeviceDisplayer>().playerInput = playerController.GetComponent<PlayerInput>();
    }

    public void CheckForPlayersReady()
    {
        if (Time.timeSinceLevelLoad < MinTimeInScene)
            return;
        if (!tryStartGame)
            return;
        int playerCount = 0;
        int readyPlayerCount = 0;
        foreach (var controller in PlayerControllers)
        {
            playerCount += controller.Player1Active ? 1 : 0;
            readyPlayerCount += (controller.Player1Ready && controller.Player1Active) ? 1 : 0;
            playerCount += controller.Player2Active ? 1 : 0;
            readyPlayerCount += (controller.Player2Ready && controller.Player2Active) ? 1 : 0;
        }
        if (playerCount < 2 || readyPlayerCount < playerCount)
        {
            return;
        }
        tryStartGame = false;
        GetComponent<PlayerInputManager>().DisableJoining();
        LoadSceneTimed(GameState.Game,0);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.buildIndex)
        {
            case 0:
                // MainMenu
                break;
            case 1:
                currentGameState = GameState.PlayerSelect;
                PlayerSelectLoaded();
                break;
            case 2:
                currentGameState = GameState.Game;
                GameSceneLoaded();
                break;
            case 3:
                currentGameState = GameState.Highscore;
                HighscoreSceneLoaded();
                break;
            case 4:
                // Credits
                break;
            default:
                break;
        }
    }

    void PlayerSelectLoaded()
    {
        var grid = GameObject.FindWithTag("DeviceGrid");
        if (grid)
            deviceDisplayGrid = grid;
        foreach (var control in PlayerControllers)
        {
            var go = Instantiate(deviceDisplayerPrefab, deviceDisplayGrid.transform);
            go.GetComponent<DeviceDisplayer>().playerInput = control.GetComponent<PlayerInput>();
        }
        GetComponent<PlayerInputManager>().EnableJoining();
        tryStartGame = true;
    }

    void GameSceneLoaded()
    {
        Players = new List<Player>();
        MatchTime = 70;
        SetInputToMenuOrGame(true);
        NumberOfConnectedPlayers = 0;
        foreach (var controller in PlayerControllers)
        {
            if (controller.Player1Active)
            {
                SpawnNewPlayer(Playerdatas[NumberOfConnectedPlayers]);
                controller.controlledPlayer1 = Players[NumberOfConnectedPlayers];
                if (!controller.Player2Active)
                {
                    controller.controlledPlayer2 = Players[NumberOfConnectedPlayers];
                }
                NumberOfConnectedPlayers++;
            }
            if (controller.Player2Active)
            {
                SpawnNewPlayer(Playerdatas[NumberOfConnectedPlayers]);
                controller.controlledPlayer2 = Players[NumberOfConnectedPlayers];
                if (!controller.Player1Active)
                {
                    controller.controlledPlayer1 = Players[NumberOfConnectedPlayers];
                }
                NumberOfConnectedPlayers++;
            }
            if (!controller.Player1Active && !controller.Player2Active)
            {
                controller.controlledPlayer1 = null;
                controller.controlledPlayer2 = null;
            }
        }

        GameTimerText = GameObject.FindWithTag("TimerText").GetComponent<TextMeshProUGUI>();
        GameOverText = GameObject.FindWithTag("GameOverText").GetComponent<TextMeshProUGUI>();
        musicFader = GameObject.FindWithTag("GameMusicController").GetComponent<FadeMusic>();

        isGameActive = true;
    }

    void HighscoreSceneLoaded()
    {
        GameOverTitleGrid = GameObject.FindWithTag("PlayerTitleGrid");
        foreach (Player p in Players.OrderByDescending(p => p.score))
        {
            GameObject go = Instantiate(p.data.GameoverSceneName, GameOverTitleGrid.transform);
            go.GetComponent<TextMeshProUGUI>().text += "  -  " + p.score;
        }
    }


    void FixedUpdate()
    {
        switch (currentGameState)
        {
            case GameState.PlayerSelect:
                CheckForPlayersReady();
                break;
            case GameState.Game:
                if (isGameActive)
                {
                    UpdateGameUI();
                    ManageGameState();
                }
                break;
            case GameState.Highscore:
                NewGame();
                break;
            default:
                break;
        }
    }

    void NewGame()
    {
        if (Time.timeSinceLevelLoad < MinTimeInScene)
            return;

        int activePlayerCount = 0;
        foreach (var controller in PlayerControllers)
        {
            if (controller.Player1Active)
            {
                activePlayerCount++;
            }
            if (controller.Player2Active)
            {
                activePlayerCount++;
            }
        }

        if (activePlayerCount >= 2)
        {
            LoadSceneTimed(GameState.PlayerSelect, 0);
        }
    }

    void UpdateGameUI()
    {
        MatchTime -= Time.deltaTime;

        if(MatchTime>10.0f)
        {
            int time = (int)MatchTime;
            GameTimerText.text = time.ToString();

            if (MatchTime > 60.0f)
            {
                float displaytime = 10.0f - (70.0f - MatchTime);
                GameTimerText.color = Color.green;
                GameTimerText.text = "-"+displaytime.ToString("#.#");
            }
            else
            {
                if(!hasMusicfadeStarted)
                {
                    musicFader.StartFade(true);
                    hasMusicfadeStarted = true;
                }
                GameTimerText.color = Color.white;
                foreach (Player p in Players)
                {
                    p.isAttackAllowed = true;
                }
            }

        }
        else
        {
            GameTimerText.color = Color.red;
            GameTimerText.text = MatchTime.ToString("#.#");
        }
        if (MatchTime <= 0)
        {
            GameTimerText.gameObject.SetActive(false);
            GameOverText.text = "GAME OVER";
            isGameActive = false;
        }
    }

    void ManageGameState()
    {
        if(MatchTime <= 0)
        {
            SetInputToMenuOrGame(false);

            LoadSceneTimed(GameState.Highscore, 3.0f);
        }
    }

    void SetInputToMenuOrGame(bool game)
    {
        foreach (var controller in PlayerControllers)
        {
            controller.GetComponent<PlayerInput>().SwitchCurrentActionMap(game ? "BubbleControl" : "PlayerSelect");
        }
    }


    public List<Bubble> GetAllBubbles()
    {
        List<Bubble> bubbles = new List<Bubble>();

        foreach(Player p in Players)
        {
            bubbles.AddRange(p.Bubbles);
        }
        return bubbles;
    }

    void LoadSceneTimed(GameState gameState, float t)
    {
        StartCoroutine(CoroutineLoadSceneTimed((int)gameState, t));
    }

    IEnumerator CoroutineLoadSceneTimed(int index, float t)
    {
        yield return new WaitForSecondsRealtime(t);
        SceneManager.LoadScene(index);
    }
}