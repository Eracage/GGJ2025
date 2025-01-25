using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    AudioSource audioSource;
    public PlayerData data;
    public Transform BubbleSpawnPosition;

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
        {
            GrowBubble();
        }
        else 
        {
            audioSource.loop = false;
            audioSource.pitch = 1.0f;
        }
        DebugControls();
    }

    private void LateUpdate()
    {
        lastframeposition = transform.position;
    }

    void DebugControls()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PlayerMovement = Vector2.Lerp(PlayerMovement, PlayerMovementInput, 0.1f);
        Vector3 movement = new Vector3(PlayerMovement.x, PlayerMovement.y, 0) * data.Speed * Time.fixedDeltaTime;
        transform.Translate(movement);
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
        audioSource.Stop();
        data.Bubbles.Add(bubble);
        bubble.transform.parent = null;
        bubble.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(transform.position - lastframeposition) * data.BubbleEjectionForce, ForceMode.Impulse);
        Bubble BubbleComponent = bubble.AddComponent<Bubble>();
        BubbleComponent.playerIndex = data.index;
        BubbleComponent.BubblepopClips = data.BubblePopClips;
        bubble = null;
        isHoldingBubble = false;
        audioSource.PlayOneShot(data.BubblePopClips[Random.Range(0, 3)]);
    }

    public void Attack()
    {
        // if cooldown etc
        // attack
    }
}
