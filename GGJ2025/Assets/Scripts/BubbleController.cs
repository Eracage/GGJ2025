using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleController : MonoBehaviour
{
    public static BubbleController sInstance;


    public List<GameObject> RedBubbles = new List<GameObject>();
    public List<GameObject> GreenBubbles = new List<GameObject>();
    public GameObject GreenBubblePrefab;
    public GameObject RedBubblePrefab;
    public GameObject Player1;
    public GameObject Player2;
    public float PlayerSpeed;
    public float PlayerDamage;
    public float PlayerAttackRange = 1.3f;
    Vector3 player1lastpos;
    Vector3 player2lastpos;
    float spawnforce = 3.0f;

    bool PlayerState = true;

    private void Start()
    {
        if(sInstance == null)
        {
            sInstance = this;
        }

        player1lastpos = Player1.transform.position;
        player2lastpos = Player2.transform.position;
    }

    void DebugControls()
    {

        float hor = Input.GetAxis("Horizontal") * PlayerSpeed * Time.deltaTime;
        float ver = Input.GetAxis("Vertical") * PlayerSpeed * Time.deltaTime;
        if (PlayerState)
        {
            Player1.transform.Translate(new Vector3(hor, ver, 0));
        }
        else
        {
            Player2.transform.Translate(new Vector3(hor, ver, 0));
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerState = !PlayerState;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnBubble(PlayerState, PlayerState ? Player1.transform.position : Player2.transform.position);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            AttackBubbles(PlayerState);
        }
    }

    void Update()
    {
        DebugControls();
    }

    private void LateUpdate()
    {
        player1lastpos = Player1.transform.position;
        player2lastpos = Player2.transform.position;
    }

    public void SpawnBubble(bool isPlayer1, Vector3 position)
    {
        if(isPlayer1)
        {
            GameObject current = Instantiate(GreenBubblePrefab, position, Quaternion.identity);
            GreenBubbles.Add(current);
            current.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(Player1.transform.position - player1lastpos) * spawnforce, ForceMode.Impulse);
        }
        else
        {
            GameObject current = Instantiate(RedBubblePrefab, position, Quaternion.identity);
            RedBubbles.Add(current);
            current.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(Player2.transform.position - player2lastpos)*spawnforce, ForceMode.Impulse);
        }
    }

    public void AttackBubbles(bool isPlayer1)
    {
        foreach (GameObject go in isPlayer1 ? RedBubbles : GreenBubbles)
        {
            if (!go)
                continue;
            if (Vector3.Distance(go.transform.position, isPlayer1 ? Player1.transform.position : Player2.transform.position) < PlayerAttackRange)
            {
                go.GetComponent<Bubble>().TakeDamage(PlayerDamage);
            }
        }
    }

    public void RemoveBubbleFromList(GameObject bubble)
    {
        if (GreenBubbles.Contains(bubble))
        {
            GreenBubbles.Remove(bubble);
        }
        else
        {
            RedBubbles.Remove(bubble);
        }
    }
}
