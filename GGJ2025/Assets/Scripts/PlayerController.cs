using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Player controlledPlayer1;
    public bool Player1Active = false;
    public bool Player1Ready = false;

    public Player controlledPlayer2;
    public bool Player2Active = false;
    public bool Player2Ready = false;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindWithTag("GameController").GetComponent<GameManager>().OnPlayerJoined(this);
    }

    public void OnMovement1(InputAction.CallbackContext context)
    {
        if (!controlledPlayer1)
        {
            return;
        }
        controlledPlayer1.PlayerMovementInput = context.ReadValue<Vector2>();
    }

    public void OnAction1(InputAction.CallbackContext context)
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

    public void OnMovement2(InputAction.CallbackContext context)
    {
        if (!controlledPlayer2)
        {
            return;
        }
        controlledPlayer2.PlayerMovementInput = context.ReadValue<Vector2>();
    }

    public void OnAction2(InputAction.CallbackContext context)
    {
        if (!controlledPlayer2)
        {
            return;
        }
        if (context.performed)
        {
            float action = context.ReadValue<float>();
            if (action < 0)
            {
                controlledPlayer2.Attack();
            }
            else if (action > 0)
            {
                controlledPlayer2.StartGrowingBubble();
            }
        }
        else if (context.canceled)
        {
            controlledPlayer2.ReleaseBubble();
        }
    }

    public void OnSelect1(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float action = context.ReadValue<float>();
            if (action < 0)
            {
                Player1Ready = true;
                Player1Active = true;
            }
            else if (action > 0)
            {
                Player1Ready = false;
                Player1Active = false;
            }
        }
        else if (context.canceled)
        {
            Player1Ready = false;
        }
    }
    public void OnSelect2(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float action = context.ReadValue<float>();
            if (action < 0)
            {
                Player2Ready = true;
                Player2Active = true;
            }
            else if (action > 0)
            {
                Player2Ready = false;
                Player2Active = false;
            }
        }
        else if (context.canceled)
        {
            Player2Ready = false;
        }
    }
}
