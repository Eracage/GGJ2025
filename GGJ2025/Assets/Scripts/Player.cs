using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    AudioSource audioSource;
    public PlayerData data;
    public Transform BubbleSpawnPosition;
    public List<Bubble> Bubbles = new List<Bubble>();
    bool isAttacking = false;
    bool isHoldingBubble = false;
    GameObject bubble;

    Vector3 lastframeposition;

    Vector2 PlayerMovement = Vector2.zero;
    public Vector2 PlayerMovementInput = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        lastframeposition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isHoldingBubble)
            GrowBubble();
    }

    private void LateUpdate()
    {
        lastframeposition = transform.position;
    }

    void FixedUpdate()
    {
        PlayerMovement = Vector2.Lerp(PlayerMovement, PlayerMovementInput, 0.1f);
        Vector3 movement = new Vector3(PlayerMovement.x, PlayerMovement.y, 0) * data.Speed * Time.fixedDeltaTime;
        transform.Translate(movement);

        if (isAttacking)
            AttackTick();
    }

    public void StartGrowingBubble()
    {
        isHoldingBubble = true;
        bubble = Instantiate(data.BubblePrefab, BubbleSpawnPosition);
    }


    void GrowBubble()
    {
        audioSource.pitch += Time.deltaTime;
        bubble.transform.localScale += bubble.transform.localScale * data.BubbleGrowRate * Time.deltaTime;
        bubble.transform.position =  transform.position + new Vector3(bubble.transform.localScale.x, 0, 0);
        if (bubble.transform.localScale.x > 3)
        {
            ReleaseBubble();
        }
                
    }

    public void ReleaseBubble()
    {
        if (!isHoldingBubble)
        {
            return;
        }
        audioSource.loop = false;
        audioSource.pitch = 1.0f;
        audioSource.Stop();
        bubble.transform.parent = null;
        bubble.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(transform.position - lastframeposition) * data.BubbleEjectionForce, ForceMode.Impulse);
        Bubble BubbleComponent = bubble.AddComponent<Bubble>();
        Bubbles.Add(BubbleComponent);
        BubbleComponent.playerIndex = data.index;
        BubbleComponent.BubblepopClips = data.BubblePopClips;
        bubble = null;
        isHoldingBubble = false;
        audioSource.PlayOneShot(data.BubblePopClips[Random.Range(0, data.BubblePopClips.Length)]);
    }

    public void StartAttack()
    {
        if (isHoldingBubble || isAttacking)
            return;

        isAttacking = true;
        audioSource.loop = true;
        audioSource.PlayOneShot(data.PlayerAttackSounds[Random.Range(0, data.PlayerAttackSounds.Length)]);
       //Animation loop
    }

    void AttackTick()
    {
        foreach (Bubble bubble in GameManager.sInstance.GetAllBubbles())
        {
            if(bubble.playerIndex != data.index)
            {
                if(Vector3.Distance(bubble.transform.position,transform.position)-(bubble.transform.localScale.x / 2.0f) < data.AttackRange)
                {
                    bubble.TakeDamage(data.Damage * Time.fixedDeltaTime);
                }
            }
        }
    }

    public void StopAttacking()
    {
        if (!isAttacking)
            return;
        isAttacking = false;
        audioSource.loop = false;
        audioSource.Stop();

        //Resume standard animation
    }
}