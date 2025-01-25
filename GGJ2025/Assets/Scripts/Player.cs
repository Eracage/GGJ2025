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

        float hor = Input.GetAxis("Horizontal") * data.Speed * Time.deltaTime;
        float ver = Input.GetAxis("Vertical") * data.Speed * Time.deltaTime;
        transform.Translate(new Vector3(hor, ver, 0));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isHoldingBubble = true;
            bubble = Instantiate(data.BubblePrefab, BubbleSpawnPosition);

            audioSource.pitch = Random.Range(1.2f,1.6f);
            audioSource.loop = true;
            audioSource.PlayOneShot(data.BubbleFillSound);
        }
        if (Input.GetKeyUp(KeyCode.Space) && isHoldingBubble)
        {
            ReleaseBubble();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && !isHoldingBubble)
        {
            Attack();
        }
    }

    void Attack()
    {
        audioSource.PlayOneShot(data.PlayerAttackSounds[Random.Range(0, 3)]);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.right, out hit, data.AttackRange))
        {
            Bubble BubbleComponent; 

            if(hit.transform.gameObject.TryGetComponent(out BubbleComponent))
            {
                if(BubbleComponent.playerIndex != data.index)
                {
                    BubbleComponent.TakeDamage(data.Damage);
                }
            }
        }
    }

    void ReleaseBubble()
    {
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
}
