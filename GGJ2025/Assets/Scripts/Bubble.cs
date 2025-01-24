using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public float Health;

    Rigidbody rb;

    List<GameObject> touchingballs = new List<GameObject>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Health = transform.localScale.magnitude;
    }
    public void TakeDamage(float damage)
    {
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
            if (Vector3.Distance(gameObject.transform.position, go.transform.position) < 2.2f)
            {
                rb.AddForce(Vector3.Normalize(go.transform.position - transform.position) * 0.3f, ForceMode.Acceleration);
            }
            else
            {
                touchingballs.Remove(go);
            }   
        }
    }

    void PopBubble()
    {
        //Add animation and sound effects here!
        BubbleController.sInstance.RemoveBubbleFromList(gameObject);
        Destroy(gameObject);
    }


    private void OnCollisionEnter(Collision collision)
    {
        
        if(BubbleController.sInstance.GreenBubbles.Contains(gameObject) && BubbleController.sInstance.GreenBubbles.Contains(collision.gameObject))
        {
            touchingballs.Add(collision.gameObject);
        }else if (BubbleController.sInstance.RedBubbles.Contains(gameObject) && BubbleController.sInstance.RedBubbles.Contains(collision.gameObject))
        {
            touchingballs.Add(collision.gameObject);
        }
    }

}
