using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class PllayerSpeedControl : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created[SerializeField] private ContinuousMoveProviderBase dynamicMoveProvider;

    // Use a public property or a separate private variable for the current speed
   [SerializeField] private DynamicMoveProvider dynamicMoveProvider;
    public InputActionReference pressA;
    public InputActionReference pressB;
    public TextMeshProUGUI speedDisplay;
    // Use a public property or a separate private variable for the current speed
    private float currentMoveSpeed = 1.4f;
    private const float MinSpeed = 1f;
    private const float MaxSpeed = 2.5f;

    void Update()
    {
        // 1. Check for button input to toggle speed
        HandleInput();

        // 2. Apply the current speed every frame
        dynamicMoveProvider.moveSpeed = currentMoveSpeed;
    }
    
    // New function to handle button presses
    private void HandleInput()
    {
        // Example: Using the "A" button on the right controller to increase speed
        if (pressA.action.WasPressedThisFrame())
        {
            currentMoveSpeed += 0.1f;
            currentMoveSpeed = Mathf.Clamp(currentMoveSpeed, MinSpeed, MaxSpeed);
            Debug.Log("Speed Increased: " + currentMoveSpeed);
            //show the speed on the text mesh for 1 second
            speedDisplay.text = "MoveSpeed: " + currentMoveSpeed.ToString("F1");
            Invoke("ClearSpeedDisplay", 1f);
        }

        // Example: Using the "B" button on the right controller to decrease speed
        if (pressB.action.WasPressedThisFrame())
        {
            currentMoveSpeed -= 0.1f;
            currentMoveSpeed = Mathf.Clamp(currentMoveSpeed, MinSpeed, MaxSpeed);
            Debug.Log("Speed Decreased: " + currentMoveSpeed);
            speedDisplay.text = "MoveSpeed: " + currentMoveSpeed.ToString("F1");
            Invoke("ClearSpeedDisplay", 1f);
        }
    }
    private void ClearSpeedDisplay()
    {
        speedDisplay.text = "";
    }
}
