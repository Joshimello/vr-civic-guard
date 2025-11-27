using UnityEngine;

public class WaitQuest : Quest
{
    [Header("Wait Quest Settings")]
    [SerializeField] private float waitDuration = 5f;
    [SerializeField] private bool showCountdown = true;

    private float timeElapsed = 0f;
    private float timeRemaining = 0f;

    protected override void Start()
    {
        base.Start();
        timeRemaining = waitDuration;

        // Set default quest description if not customized
        if (questDescription == "Complete this quest")
        {
            questDescription = showCountdown ?
                $"Wait {waitDuration:F0} seconds" :
                "Please wait...";
        }
    }

    protected override void Update()
    {
        base.Update();

        if (IsActive && !IsCompleted)
        {
            timeElapsed += Time.deltaTime;
            timeRemaining = Mathf.Max(0f, waitDuration - timeElapsed);

            // Update description with countdown if enabled
            if (showCountdown)
            {
                questDescription = timeRemaining > 0 ?
                    $"Wait {timeRemaining:F1} seconds" :
                    "Wait complete!";
            }
        }
    }

    protected override bool CheckQuestCompletion()
    {
        return timeElapsed >= waitDuration;
    }

    protected override void OnQuestStart()
    {
        base.OnQuestStart();
        timeElapsed = 0f;
        timeRemaining = waitDuration;

        Debug.Log($"WaitQuest '{questName}' started - waiting for {waitDuration} seconds");
    }

    protected override void OnQuestComplete()
    {
        base.OnQuestComplete();

        if (showCountdown)
        {
            questDescription = "Wait complete!";
        }

        Debug.Log($"WaitQuest '{questName}' completed after {timeElapsed:F1} seconds");
    }

    public override float GetQuestProgress()
    {
        if (IsCompleted) return 1f;
        if (!IsActive) return 0f;

        return Mathf.Clamp01(timeElapsed / waitDuration);
    }

    public override void ResetQuest()
    {
        base.ResetQuest();
        timeElapsed = 0f;
        timeRemaining = waitDuration;

        // Reset description
        questDescription = showCountdown ?
            $"Wait {waitDuration:F0} seconds" :
            "Please wait...";
    }

    // Public properties for external access
    public float WaitDuration => waitDuration;
    public float TimeElapsed => timeElapsed;
    public float TimeRemaining => timeRemaining;

    // Utility method to set wait duration at runtime
    public void SetWaitDuration(float newDuration)
    {
        if (!IsActive)
        {
            waitDuration = newDuration;
            timeRemaining = waitDuration;

            if (showCountdown)
            {
                questDescription = $"Wait {waitDuration:F0} seconds";
            }
        }
        else
        {
            Debug.LogWarning("Cannot change wait duration while quest is active");
        }
    }
}
