using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Player controlledPlayer1;
    public bool Player1Active = false;

    public Player controlledPlayer2;
    public bool Player2Active = false;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindWithTag("GameController").GetComponent<GameManager>().OnPlayerJoined(this);
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (!controlledPlayer1)
        {
            return;
        }
        controlledPlayer1.PlayerMovementInput = context.ReadValue<Vector2>();
    }

    public void OnAction(InputAction.CallbackContext context)
    {
        if (!controlledPlayer1)
        {
            return;
        }
        if (context.performed)
        {
            float action = context.ReadValue<float>();
            if (action < 0)
            {
                controlledPlayer1.Attack();
            }
            else if (action > 0)
            {
                controlledPlayer1.StartGrowingBubble();
            }
        }
        else if (context.canceled)
        {
            controlledPlayer1.ReleaseBubble();
        }
    }

    public void Attack()
    {

    }

    public void Create()
    {

    }
}
