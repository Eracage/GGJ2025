using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    public int index;
    public int Score = 0;
    public float Damage = 0.5f;
    public float Speed = 3.5f;
    public float BubbleEjectionForce = 3.0f;
    public float BubbleGrowRate = 1.5f;
    public GameObject PlayerPrefab;
    public GameObject BubblePrefab;
    public List<GameObject> Bubbles = new List<GameObject>();
}