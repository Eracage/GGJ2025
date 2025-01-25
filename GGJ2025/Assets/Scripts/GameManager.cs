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


    public float MatchTime = 60;
    bool isGameActive = false;

    TextMeshProUGUI GameTimerText;
    TextMeshProUGUI GameOverText;
    GameObject GameOverTitleGrid;

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

    public void StartMap()
    {
        if (NumberOfConnectedPlayers > 0)
        {
            return;
        }
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
        GetComponent<PlayerInputManager>().DisableJoining();
        foreach (var controller in PlayerControllers)
        {
            controller.GetComponent<PlayerInput>().SwitchCurrentActionMap("BubbleControl");
        }
        SceneManager.LoadScene(1);

        SceneManager.sceneLoaded += GameSceneLoaded;
        SceneManager.sceneLoaded += OnGameOverSceneLoaded;
    }

    public void GameSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != 1)
        {
            return;
        }
        NumberOfConnectedPlayers = 0;
        foreach (var controller in PlayerControllers)
        {
            if (controller.Player1Active)
            {
                SpawnNewPlayer(Playerdatas[NumberOfConnectedPlayers]);
                controller.controlledPlayer1 = Players[NumberOfConnectedPlayers];
                NumberOfConnectedPlayers++;
            }
            if (controller.Player2Active)
            {
                SpawnNewPlayer(Playerdatas[NumberOfConnectedPlayers]);
                controller.controlledPlayer2 = Players[NumberOfConnectedPlayers];
                NumberOfConnectedPlayers++;
            }
        }

        GameTimerText = GameObject.FindWithTag("TimerText").GetComponent<TextMeshProUGUI>();
        GameOverText = GameObject.FindWithTag("GameOverText").GetComponent<TextMeshProUGUI>();

        isGameActive = true;
    }

    public void RespawnPlayer(int index)
    {
        Players[index].transform.position = PlayerSpawnpoints[index];
    }

    void Awake()
    {
        if(sInstance == null)
        {
            sInstance = this;
        } else
        {
            DestroyImmediate(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    void FixedUpdate()
    {
        if(isGameActive)
        {
            UpdateGameUI();
            ManageGameState();
        }
        else
        {
            StartMap();
        }
    }

    void UpdateGameUI()
    {
        MatchTime -= Time.deltaTime;
        if(MatchTime>10.0f)
        {
            int time = (int)MatchTime;
            GameTimerText.text = time.ToString();
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
            foreach (var device in PlayerControllers)
            {
                device.GetComponent<PlayerInput>().DeactivateInput();
            }

            StartCoroutine(LoadSceneTimed(2, 3.0f));
        }
    }

    void OnGameOverSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != 2)
            return;

        GameOverTitleGrid = GameObject.FindWithTag("PlayerTitleGrid");
        foreach(Player p in Players.OrderByDescending(p => p.Bubbles.Count))
        {
            GameObject go = Instantiate(p.data.GameoverSceneName, GameOverTitleGrid.transform);
            go.GetComponent<TextMeshProUGUI>().text += "  -  " + p.Bubbles.Count;
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

    IEnumerator LoadSceneTimed(int index, float t)
    {
        yield return new WaitForSecondsRealtime(t);
        SceneManager.LoadScene(index);
    }
}