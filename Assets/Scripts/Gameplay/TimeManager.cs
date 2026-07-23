using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [Header("Time Settings")]
    public float realSecondsPerGameMinute = 1f;

    public bool isTimePaused = false;

    private DateTime currentGameTime;
    private float timer = 0f;

    // Event for the Billing Manager
    public event Action OnHourPassed;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Start the OS clock at a specific date/time (e.g., Jan 1st, 2026 at 8:00 AM)
        currentGameTime = new DateTime(2026, 1, 1, 8, 0, 0);
    }

    private void Update()
    {
        if (isTimePaused) return;

        timer += Time.deltaTime;

        if (timer >= realSecondsPerGameMinute)
        {
            timer -= realSecondsPerGameMinute; // Better than resetting to 0 to prevent lost fractions of a second
            
            // Add 1 in-game minute
            currentGameTime = currentGameTime.AddMinutes(1);

            // If the minute is exactly 0, an hour has just passed!
            if (currentGameTime.Minute == 0)
            {
                OnHourPassed?.Invoke();
            }
        }
    }

    public string GetFormattedTime()
    {
        // We drop the 'ss' (seconds) here because the game clock moves by the minute!
        return currentGameTime.ToString("HH:mm\ndd-MM-yyyy");
    }

    public void PauseTime()
    {
        Debug.Log("Time Paused");
        isTimePaused = true;
    }

    public void ResumeTime()
    {
        Debug.Log("Time Resumed");
        isTimePaused = false;
    }

    public void ToggleTime()
    {
        Debug.Log("Time Toggled");
        isTimePaused = !isTimePaused;
    }
}