using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the input button UI for updating a selected Sudoku cell with a player's number choice.
/// Implements a singleton pattern for easy access from other scripts.
/// </summary>
public class InputButton : MonoBehaviour
{
    // Singleton instance for easy access.
    public static InputButton instance;

    // Reference to the last selected Sudoku cell.
    private SudokuCell lastCell;

    // Pencil mode toggle
    public static bool IsPencilMode = false;
    [Header("Pencil Mode UI")]
    public Button pencilModeButton;
    [SerializeField] private Color pencilNormalColor = Color.white;
    [SerializeField] private Color pencilActiveColor = new Color(0.7f, 0.8f, 1f);

    // UI GameObject to display a wrong input message.
    // Exposed in the Inspector for easy assignment.
    [SerializeField, Tooltip("UI GameObject to display wrong input message.")]
    private GameObject wrongText;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// Sets up the singleton instance and checks for duplicate instances.
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of InputButton detected. Only one instance should exist.");
        }
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// Deactivates the input button UI on start.
    /// </summary>
    private void Start()
    {
        // Deactivate the input button UI when the game starts.
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Activates the input button UI for the specified Sudoku cell.
    /// </summary>
    /// <param name="cell">The SudokuCell that will receive the input.</param>
    public void ActivateInputButton(SudokuCell cell)
    {
        gameObject.SetActive(true);
        lastCell = cell;
    }

    /// <summary>
    /// Called when a number button is clicked.
    /// Updates the last selected cell's value, hides any wrong input message, and deactivates the UI.
    /// </summary>
    /// <param name="num">The number input provided by the player.</param>
    public void ClickedButton(int num)
    {
        // Check if a cell was selected before updating its value.
        if (lastCell != null)
        {
            if (IsPencilMode)
            {
                lastCell.AddOrRemovePencilMark(num);
            }
            else
            {
                lastCell.UpdateValue(num);
            }
        }
        else
        {
            Debug.LogWarning("InputButton: lastCell is null. Ensure a cell is selected before providing input.");
        }

        // Hide the wrong input message if it is assigned.
        if (wrongText != null)
        {
            wrongText.SetActive(false);
        }
        else
        {
            Debug.LogWarning("InputButton: wrongText GameObject is not assigned in the Inspector.");
        }

        // Deactivate the input button UI after processing the input.
        gameObject.SetActive(false);
    }

    // Toggle Pencil mode (can be called from a UI button)
    public void TogglePencilMode()
    {
        IsPencilMode = !IsPencilMode;
        if (pencilModeButton != null)
        {
            var colors = pencilModeButton.colors;
            colors.normalColor = IsPencilMode ? pencilActiveColor : pencilNormalColor;
            colors.selectedColor = colors.normalColor;
            pencilModeButton.colors = colors;
        }
    }
}
