using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleGameLoop : MonoBehaviour
{
    public enum BubbleGameState
    {
        Bubble,
        GameOver,
        BlowBubble
    }

    public BubbleGameState state = BubbleGameState.Bubble;

    public GameObject bubble;
    public GameObject gameOver;

    float blowStartTime;
    public float blowTime = 2;
    public float bubbleSize = 3;
    public float bubbleSpeed = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        bubble.transform.localScale = new Vector3(bubbleSize, bubbleSize, bubbleSize);
        gameOver.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case BubbleGameState.Bubble:
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var hits = Physics.RaycastAll(ray);
                foreach (var hit in hits)
                {
                    if (hit.transform.tag == "Player")
                    {
                        state = BubbleGameState.GameOver;
                        bubble.SetActive(false);
                        gameOver.SetActive(true);
                    }
                }
                bubble.transform.localPosition = Vector3.MoveTowards(bubble.transform.localPosition, Random.insideUnitSphere * 2, bubbleSpeed);
                break;
            case BubbleGameState.GameOver:
                if (Input.GetMouseButtonDown(0))
                {
                    state = BubbleGameState.BlowBubble;
                    gameOver.SetActive(false);
                    bubble.SetActive(true);
                    bubble.transform.localScale = new Vector3(0, 0, 0);
                    blowStartTime = Time.time;
                }
                break;
            case BubbleGameState.BlowBubble:
                if (Time.time < blowStartTime + blowTime)
                {
                    float size = ((Time.time - blowStartTime) / blowTime) * bubbleSize;
                    bubble.transform.localScale = new Vector3(size, size, size);
                } else
                {
                    bubble.transform.localScale = new Vector3(bubbleSize, bubbleSize, bubbleSize);
                    state = BubbleGameState.Bubble;
                }
                break;
        }
    }
}
