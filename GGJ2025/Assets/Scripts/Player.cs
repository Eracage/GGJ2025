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

    float lastBubbleCreationTime;
    GameObject bubble;

    public int score;
    public Vector3 lookatrotation;
    public Transform modelHolder;
    public bool isAttackAllowed = false;
    Vector3 lastframeposition;

    Vector2 PlayerMovement = Vector2.zero;
    public Vector2 PlayerMovementInput = Vector2.zero;

    Animator squidAnimator;

    // Start is called before the first frame update
    void Start()
    {
        squidAnimator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        lastframeposition = transform.position;
        lastBubbleCreationTime = Time.time;
        lookatrotation = transform.right;
    }

    // Update is called once per frame
    void Update()
    {
        if (isHoldingBubble)
            GrowBubble();

        UpdateAnimationStates();
    }

    private void LateUpdate()
    {
        lastframeposition = transform.position;
    }

    void FixedUpdate()
    {
        PlayerMovement = Vector2.Lerp(PlayerMovement, PlayerMovementInput, 0.1f);
        RotateModel();
        Vector3 movement = new Vector3(PlayerMovement.x, PlayerMovement.y, 0) * data.Speed * Time.fixedDeltaTime;
        transform.Translate(movement);

        UpdateScore();

        if (isAttacking)
            AttackTick();
    }

    void UpdateAnimationStates()
    {
        if (isAttacking)
        {

            squidAnimator.SetBool("IsSpinning", true);
        }
        else
        {
            squidAnimator.SetBool("IsSpinning", false);
        }
        if(PlayerMovementInput.magnitude>0)
        {
            squidAnimator.SetBool("IsSwimming", true);
        }
        else
        {
            squidAnimator.SetBool("IsSwimming", false);
        }
    }

    void RotateModel()
    {
        if(!isAttacking && PlayerMovementInput.magnitude>0)
        {
            modelHolder.transform.rotation = Quaternion.Slerp(modelHolder.transform.rotation, Quaternion.LookRotation(new Vector3(PlayerMovement.x,PlayerMovement.y,0.0f)), Time.deltaTime * 10f);
        }else
        {
            modelHolder.transform.rotation = Quaternion.Slerp(modelHolder.rotation, Quaternion.LookRotation(Vector3.up), Time.deltaTime * 10f);
        }
        if(!isAttacking && PlayerMovementInput.magnitude <= 0)
        {
            modelHolder.transform.rotation = Quaternion.Slerp(modelHolder.transform.rotation, Quaternion.LookRotation(Vector3.right), Time.deltaTime * 10f);
        }
    }

    void UpdateScore()
    {
        float currentscore = 0;
        foreach(Bubble b in Bubbles)
        {
            currentscore += b.transform.localScale.x;
        }
        score = (int)currentscore;
    }

    public void StartGrowingBubble()
    {
        if (isHoldingBubble)
            return;

        if(Time.time - lastBubbleCreationTime > data.BubbleCreationCooldown)
        {
            isHoldingBubble = true;
            bubble = Instantiate(data.BubblePrefab, BubbleSpawnPosition);
        }

    }


    void GrowBubble()
    {
        //audioSource.pitch += Time.deltaTime;
        bubble.transform.localScale += bubble.transform.localScale * data.BubbleGrowRate * Time.deltaTime;
        bubble.transform.position =  BubbleSpawnPosition.position + BubbleSpawnPosition.forward * bubble.transform.localScale.x;
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
        //audioSource.loop = false;
        //audioSource.pitch = 1.0f;
        //audioSource.Stop();
        bubble.transform.parent = null;
        bubble.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(BubbleSpawnPosition.forward) * data.BubbleEjectionForce, ForceMode.Impulse);
        Bubble BubbleComponent = bubble.AddComponent<Bubble>();
        Bubbles.Add(BubbleComponent);
        BubbleComponent.playerIndex = data.index;
        BubbleComponent.BubblepopClips = data.BubblePopClips;
        BubbleComponent.PopParticles = data.BubblePopPrefab;
        bubble = null;
        isHoldingBubble = false;
        audioSource.PlayOneShot(data.BubblePopClips[Random.Range(0, data.BubblePopClips.Length)]);
        lastBubbleCreationTime = Time.time;
    }

    public void StartAttack()
    {
        if (isHoldingBubble || isAttacking || !isAttackAllowed)
            return;

        isAttacking = true;
        audioSource.loop = true;
        audioSource.clip = data.PlayerAttackSounds[0];
        audioSource.Play();
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