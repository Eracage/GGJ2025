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

    public GameObject PopParticles;

    List<GameObject> touchingballs = new List<GameObject>();

    Material material;

    float currentAttackAnimationValue = 0.0f;

    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        rb = GetComponent<Rigidbody>();
        Health = transform.localScale.magnitude;
    }
    public void TakeDamage(float damage)
    { 
        currentAttackAnimationValue = 0.2f;
        Health -= damage;
        if(Health <= 0)
        {
            PopBubble();
        }
    }

    private void Update()
    {
        currentAttackAnimationValue = Mathf.Lerp(currentAttackAnimationValue, 0, currentAttackAnimationValue - Time.deltaTime);
        material.SetFloat("_isUnderAttack", currentAttackAnimationValue);
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
            if (Vector3.Distance(gameObject.transform.position, touchingballs[i].transform.position) > transform.localScale.x + touchingballs[i].transform.localScale.x)
            {
                touchingballs.RemoveAt(i);
                continue;
            }
            rb.AddForce(Vector3.Normalize(touchingballs[i].transform.position - transform.position) * 0.3f, ForceMode.Acceleration);
        }
    }

    void PopBubble()
    {
        GameManager.sInstance.Players[playerIndex].Bubbles.Remove(this);
        Destroy(Instantiate(PopParticles, transform.position, Quaternion.identity), 3.0f);
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
