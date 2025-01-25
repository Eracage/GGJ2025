using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public float Health;
    public int playerIndex;
    Rigidbody rb;

    public AudioClip[] BubblepopClips;

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

        foreach(GameObject go in touchingballs)
        {
            if (Vector3.Distance(gameObject.transform.position, go.transform.position) < transform.localScale.x + go.transform.localScale.x)
            {
                rb.AddForce(Vector3.Normalize(go.transform.position - transform.position) * 0.3f, ForceMode.Acceleration);
            }
            else
            {
                objstoremove.Add(go);
            }   
        }
        touchingballs = touchingballs.Except(objstoremove).ToList();
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
