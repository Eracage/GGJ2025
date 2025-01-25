using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
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
        lastframeposition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isHoldingBubble)
            GrowBubble();
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
        bubble.transform.parent = null;
        bubble.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(transform.position - lastframeposition) * data.BubbleEjectionForce, ForceMode.Impulse);
        bubble.AddComponent<Bubble>().playerIndex = data.index;
        bubble = null;
        isHoldingBubble = false;
    }

    public void Attack()
    {
        // if cooldown etc
        // attack
    }
}
