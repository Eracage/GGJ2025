using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public float Health;
    public int playerIndex;
    Rigidbody rb;

    List<GameObject> touchingballs = new List<GameObject>();


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Health = transform.localScale.magnitude;
    }
    public void TakeDamage(float damage)
    {
        //play animation

        Health -= damage;
        if(Health <= 0)
        {
            PopBubble();
        }
    }

    private void FixedUpdate()
    {
        List<GameObject> objstoremove = new List<GameObject>();

        for (int i = touchingballs.Count - 1; i >= 0; --i)
        {
            if (!touchingballs[i])
            {
                touchingballs.RemoveAt(i);
                continue;
            }
            if (Vector3.Distance(gameObject.transform.position, touchingballs[i].transform.position) > transform.localScale.x + go.transform.localScale.x)
            {
                touchingballs.RemoveAt(i);
                continue;
            }
            rb.AddForce(Vector3.Normalize(touchingballs[i].transform.position - transform.position) * 0.3f, ForceMode.Acceleration);
        }
    }

    void PopBubble()
    {
        GameManager.sInstance.Playerdatas[playerIndex].Bubbles.Remove(gameObject);
        //Play animation
        Destroy(gameObject);
    }


    private void OnCollisionEnter(Collision collision)
    {
        Bubble collidingBubble;
        if (!collision.gameObject.TryGetComponent(out collidingBubble))
            return;
        if(playerIndex == collidingBubble.playerIndex)
        {
            touchingballs.Add(collision.gameObject);
        }
    }
}
