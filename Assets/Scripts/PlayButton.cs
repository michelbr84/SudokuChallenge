using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class handles button click events to load different scenes.
/// </summary>
public class PlayButton : MonoBehaviour
{
    // The name of the scene to load when the Start button is clicked.
    // Exposed in the Inspector for easy configuration.
    [SerializeField, Tooltip("Scene name for Play mode (e.g., 'SudokuPlay')")]
    private string playSceneName = "SudokuPlay";

    // The name of the scene to load when the Main Menu button is clicked.
    // Exposed in the Inspector for easy configuration.
    [SerializeField, Tooltip("Scene name for Main Menu (e.g., 'MainMenu')")]
    private string mainMenuSceneName = "MainMenu";

    /// <summary>
    /// Called when the Start button is clicked.
    /// Loads the play scene based on the inspector variable.
    /// </summary>
    public void StartButtonClicked()
    {
        // Check if the play scene name is set
        if (string.IsNullOrEmpty(playSceneName))
        {
            Debug.LogError("Play scene name is not set in the inspector.");
            return;
        }
        
        // Load the scene for play mode
        SceneManager.LoadScene(playSceneName);
    }

    /// <summary>
    /// Called when the Main Menu button is clicked.
    /// Loads the main menu scene based on the inspector variable.
    /// </summary>
    public void MainMenuButtonClicked()
    {
        // Check if the main menu scene name is set
        if (string.IsNullOrEmpty(mainMenuSceneName))
        {
            Debug.LogError("Main menu scene name is not set in the inspector.");
            return;
        }
        
        // Load the scene for the main menu
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
