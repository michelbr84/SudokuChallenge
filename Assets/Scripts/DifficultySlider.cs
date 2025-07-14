using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script manages the difficulty slider and updates the PlayerSettings difficulty accordingly.
/// </summary>
public class DifficultySlider : MonoBehaviour
{
    // Reference to the UI Slider component. Exposed in the Inspector for easy assignment.
    [SerializeField, Tooltip("UI Slider to adjust the difficulty level")]
    private Slider difficultySlider;

    /// <summary>
    /// Start is called before the first frame update.
    /// It sets the initial difficulty based on the slider value and adds a listener for changes.
    /// </summary>
    private void Start()
    {
        // Check if the slider is assigned in the Inspector
        if (difficultySlider == null)
        {
            Debug.LogError("Difficulty Slider is not assigned in the Inspector.");
            return;
        }

        // Set the initial difficulty based on the current slider value.
        PlayerSettings.difficulty = (int)difficultySlider.value;

        // Add a listener to call SliderChange() whenever the slider value changes.
        difficultySlider.onValueChanged.AddListener(delegate { SliderChange(); });
    }

    /// <summary>
    /// This method is called whenever the slider's value changes.
    /// It updates the difficulty in PlayerSettings based on the slider value.
    /// </summary>
    public void SliderChange()
    {
        PlayerSettings.difficulty = (int)difficultySlider.value;
    }
}
