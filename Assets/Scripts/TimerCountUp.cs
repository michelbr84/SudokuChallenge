using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField, Tooltip("TMP Text component that displays the timer (optional).")]
    private TMP_Text timerTMP;

    /// <summary>
    /// Initialization: Attempts to get the Text component if not manually assigned.
    /// </summary>
    private void Start()
    {
        // Try to find Text/TMP_Text if not assigned
        if (timerText == null) timerText = GetComponentInChildren<Text>();
        if (timerTMP == null) timerTMP = GetComponentInChildren<TMP_Text>();

        if (timerText == null && timerTMP == null)
        {
            Debug.LogError("TimerCountUp: No Text or TMP_Text component found! Please assign one in the Inspector.");
        }
    }

    /// <summary>
    /// Update is called once per frame.
    /// Calculates the elapsed time since the level was loaded and updates the timer text.
    /// </summary>
    private void Update()
    {
        // If neither text is set, exit early.
        if (timerText == null && timerTMP == null)
        {
            return;
        }

        // Calculate elapsed time since level load
        float t = Time.timeSinceLevelLoad;

        // Calculate seconds and minutes.
        int seconds = (int)(t % 60);
        int minutes = (int)(t / 60);

        // Update the timer text in MM:SS format.
        string formatted = string.Format("{0:00}:{1:00}", minutes, seconds);
        if (timerText != null) timerText.text = formatted;
        if (timerTMP != null) timerTMP.text = formatted;
    }
}
