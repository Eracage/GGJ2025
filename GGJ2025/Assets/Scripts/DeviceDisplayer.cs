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

    // Update is called once per frame
    void Update()
    {
        if (playerInput)
        {
            DeviceNameText.SetText(playerInput.currentControlScheme + " " + playerInput.playerIndex);
            var control = playerInput.GetComponent<PlayerController>();
            P1_Image.color = control.Player1Active ? (control.Player1Ready ? Color.green : Color.red) : Color.gray;
            P2_Image.color = control.Player2Active ? (control.Player2Ready ? Color.green : Color.red) : Color.gray;
        }
    }
}
