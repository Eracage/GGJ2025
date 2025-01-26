using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    [HideInInspector]
    public int index;
    [HideInInspector]
    public int Score = 0;
    [Space]
    [Header("Stats")]
    public string Name;
    public float AttackRange = 2.5f;
    public float Damage = 0.5f;
    public float Speed = 3.5f;
    public float BubbleCreationCooldown = 0.5f;
    public float BubbleEjectionForce = 3.0f;
    public float BubbleGrowRate = 1.5f;
    [Space]
    [Header("Audio clips")]
    public AudioClip BubbleFillSound;
    public AudioClip[] PlayerDeathSounds;
    public AudioClip[] PlayerAttackSounds;
    public AudioClip[] PlayerCelebrationSounds;
    public AudioClip[] BubblePopClips;
    [Space]
    [Header("Prefabs")]
    public GameObject GameoverSceneName;
    public GameObject PlayerPrefab;
    public GameObject BubblePrefab;
    public GameObject BubblePopPrefab;
}