using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerSpeedControl : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created[SerializeField] private ContinuousMoveProviderBase dynamicMoveProvider;

    // Use a public property or a separate private variable for the current speed
   [SerializeField] private ContinuousMoveProviderBase dynamicMoveProvider;

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
        if (Input.GetButtonDown("XRI_Right_A_Button"))
        {
            currentMoveSpeed += 0.1f;
            currentMoveSpeed = Mathf.Clamp(currentMoveSpeed, MinSpeed, MaxSpeed);
        }

        // Example: Using the "B" button on the right controller to decrease speed
        if (Input.GetButtonDown("XRI_Right_B_Button"))
        {
            currentMoveSpeed -= 0.1f;
            currentMoveSpeed = Mathf.Clamp(currentMoveSpeed, MinSpeed, MaxSpeed);
        }
    }
}
