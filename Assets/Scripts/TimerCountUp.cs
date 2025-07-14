using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// TimerCountUp displays the elapsed time since the level load in a UI Text component.
/// The time is shown in minutes and seconds format (MM:SS).
/// </summary>
public class TimerCountUp : MonoBehaviour
{
    // Reference to the UI Text component for displaying the timer.
    // Exposed in the Inspector for easy assignment.
    [SerializeField, Tooltip("UI Text component that displays the timer.")]
    private Text timerText;

    /// <summary>
    /// Initialization: Attempts to get the Text component if not manually assigned.
    /// </summary>
    private void Start()
    {
        // If timerText is not assigned in the Inspector, try to find one in the children.
        if (timerText == null)
        {
            timerText = GetComponentInChildren<Text>();
        }

        // Check if the timerText was successfully assigned.
        if (timerText == null)
        {
            Debug.LogError("TimerCountUp: No Text component found! Please assign one in the Inspector.");
        }
    }

    /// <summary>
    /// Update is called once per frame.
    /// Calculates the elapsed time since the level was loaded and updates the timer text.
    /// </summary>
    private void Update()
    {
        // If timerText is not set, exit early.
        if (timerText == null)
        {
            return;
        }

        // Calculate elapsed time since level load
        float t = Time.timeSinceLevelLoad;

        // Calculate seconds and minutes.
        int seconds = (int)(t % 60);
        int minutes = (int)(t / 60);

        // Update the timer text in MM:SS format.
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
