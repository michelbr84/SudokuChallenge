using UnityEngine;

public static class BestTimeManager
{
	private static string Key(int difficulty)
	{
		return $"BestTime_Diff_{difficulty}";
	}

	public static void SaveIfBest(int difficulty, float elapsedSeconds)
	{
		float currentBest = PlayerPrefs.GetFloat(Key(difficulty), -1f);
		if (currentBest < 0f || elapsedSeconds < currentBest)
		{
			PlayerPrefs.SetFloat(Key(difficulty), elapsedSeconds);
			PlayerPrefs.Save();
		}
	}

	public static float? GetBestSeconds(int difficulty)
	{
		float v = PlayerPrefs.GetFloat(Key(difficulty), -1f);
		if (v < 0f)
		{
			return null;
		}
		return v;
	}

	public static string FormatTime(float seconds)
	{
		int s = Mathf.FloorToInt(seconds % 60f);
		int m = Mathf.FloorToInt(seconds / 60f);
		return string.Format("{0:00}:{1:00}", m, s);
	}
}


