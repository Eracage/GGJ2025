using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class DeviceDisplayer : MonoBehaviour
{
    public TextMeshProUGUI DeviceNameText;
    public Image P1_Image;
    public Image P2_Image;

    public PlayerInput playerInput;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ClearInput(bool right)
    {
        if (playerInput)
        {
            var control = playerInput.GetComponent<PlayerController>();
            if (right)
            {
                control.Player2Active = false;
            } else
            {
                control.Player1Active = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInput)
        {
            var control = playerInput.GetComponent<PlayerController>();
            int playerCount = 0;
            playerCount += control.Player1Active ? 1 : 0;
            playerCount += control.Player2Active ? 1 : 0;
            DeviceNameText.SetText("D-" + playerInput.playerIndex + ": " + playerInput.currentControlScheme + " - " + playerCount + " Players");
            P1_Image.color = control.Player1Active ? (control.Player1Ready ? Color.green : Color.red) : Color.gray;
            P2_Image.color = control.Player2Active ? (control.Player2Ready ? Color.green : Color.red) : Color.gray;
        }
    }
}
