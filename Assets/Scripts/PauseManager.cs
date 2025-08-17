using UnityEngine;

public class PauseManager : MonoBehaviour
{
	[SerializeField, Tooltip("UI panel shown when the game is paused")] private GameObject pauseMenu;
	[SerializeField, Tooltip("Pause when Back/Escape is pressed")] private bool enableBackButtonPause = true;

	private bool isPaused = false;

	private void Awake()
	{
		// Ensure game starts unpaused
		Time.timeScale = 1f;
		if (pauseMenu != null)
		{
			pauseMenu.SetActive(false);
		}
	}

	private void Update()
	{
		if (!enableBackButtonPause)
		{
			return;
		}

		// Android back button maps to Escape
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			TogglePause();
		}
	}

	public void TogglePause()
	{
		isPaused = !isPaused;
		Time.timeScale = isPaused ? 0f : 1f;
		if (pauseMenu != null)
		{
			pauseMenu.SetActive(isPaused);
		}
	}

	public void Resume()
	{
		isPaused = false;
		Time.timeScale = 1f;
		if (pauseMenu != null)
		{
			pauseMenu.SetActive(false);
		}
	}
}


