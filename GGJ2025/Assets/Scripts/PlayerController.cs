using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public int PlayerID = 0;
    public float PlayerSpeed;
    public float PlayerDamage;
    public float PlayerAttackRange = 1.3f;
    public float PlayerSpawnForce = 3.0f;
    private Vector3 PlayerLastPosition = Vector3.zero;
    private Vector2 PlayerMovement = Vector2.zero;
    private Vector2 PlayerMovementInput = Vector2.zero;
    public bool PlayerAttackActive = false;
    public bool PlayerCreateActive = false;

    // Start is called before the first frame update
    void Start()
    {
        PlayerID = 2;
        PlayerLastPosition = transform.position;
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        PlayerMovementInput = context.ReadValue<Vector2>();
    }

    public void OnAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float action = context.ReadValue<float>();
            Debug.Log(action);
            if (action < 0)
            {
                PlayerAttackActive = true;
                PlayerCreateActive = false;
            }
            else if (action > 0)
            {
                PlayerAttackActive = false;
                PlayerCreateActive = true;
            }
        }
        else if (context.canceled)
        {
            PlayerAttackActive = false;
            PlayerCreateActive = false;
        }
    }

    public void Attack()
    {

    }

    public void Create()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PlayerMovement = Vector2.Lerp(PlayerMovement, PlayerMovementInput, 0.1f);
        Vector3 movement = new Vector3(PlayerMovement.x, PlayerMovement.y, 0) * PlayerSpeed * Time.fixedDeltaTime;
        transform.Translate(movement);
    }
}
