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

        float hor = Input.GetAxis("Horizontal") * data.Speed * Time.deltaTime;
        float ver = Input.GetAxis("Vertical") * data.Speed * Time.deltaTime;
        transform.Translate(new Vector3(hor, ver, 0));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isHoldingBubble = true;
            bubble = Instantiate(data.BubblePrefab, BubbleSpawnPosition);
        }
        if (Input.GetKeyUp(KeyCode.Space) && isHoldingBubble)
        {
            ReleaseBubble();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            //attack
        }
    }

    void ReleaseBubble()
    {
        bubble.transform.parent = null;
        bubble.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(transform.position - lastframeposition) * data.BubbleEjectionForce, ForceMode.Impulse);
        bubble.AddComponent<Bubble>().playerIndex = data.index;
        bubble = null;
        isHoldingBubble = false;
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
}
