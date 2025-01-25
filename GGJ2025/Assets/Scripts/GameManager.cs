using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager sInstance;

    public Vector3[] PlayerSpawnpoints;

    List<GameObject> Players = new List<GameObject>();
    public List<PlayerData> Playerdatas = new List<PlayerData>();

    void SpawnNewPlayer(PlayerData data)
    {
        GameObject current = Instantiate(data.PlayerPrefab, PlayerSpawnpoints[Players.Count], Quaternion.identity);
        data.index = Players.Count;
        current.GetComponent<Player>().data = data;
        Players.Add(current);
    }

    public void RespawnPlayer(int index)
    {
        Players[index].transform.position = PlayerSpawnpoints[index];
    }


    // Start is called before the first frame update
    void Start()
    {
        if(sInstance == null)
        {
            sInstance = this;
        }

        foreach(PlayerData data in Playerdatas)
        {
            SpawnNewPlayer(data);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}