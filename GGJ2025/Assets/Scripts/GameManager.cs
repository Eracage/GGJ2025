using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager sInstance;

    public Vector3[] PlayerSpawnpoints;

    int NumberOfConnectedPlayers = 0;

    List<Player> Players = new List<Player>();
    public List<PlayerData> Playerdatas = new List<PlayerData>();

    List<PlayerController> PlayerControllers = new List<PlayerController>();

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
    }

    public void StartMap()
    {
        if (NumberOfConnectedPlayers > 0)
        {
            return;
        }
        int playerCount = 0;
        foreach (var controller in PlayerControllers)
        {
            playerCount += controller.Player1Active ? 1 : 0;
            playerCount += controller.Player2Active ? 1 : 0;
        }
        if (playerCount < 2)
        {
            return;
        }

        GetComponent<PlayerInputManager>().DisableJoining();
        SceneManager.LoadScene(1);

        SceneManager.sceneLoaded += GameSceneLoaded;
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
            DestroyImmediate(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        StartMap();
    }
}