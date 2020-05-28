using UnityEngine;
using System.Collections.Generic;

namespace SimpleCareerStatistics
{
	class StatisticsGUI : MonoBehaviour
	{
		public bool show = false;
		public List<StatisticsChanges> trackers = new List<StatisticsChanges>();
		public int currentTracker = 0;

		private Rect windowPosition;

		public void Start()
		{
			ResetWindowPosition();
			GameEvents.onGameSceneLoadRequested.Add(Close);
		}

		public void OnDestroy()
		{
			GameEvents.onGameSceneLoadRequested.Remove(Close);
		}

		public void OnGUI()
		{
			if (!show) return;
			GUI.skin = StatGUIUtility.BaseSkin;
			windowPosition = trackers[currentTracker].DrawWindow(windowPosition);
		}

		public void Open()
		{
			ResetWindowPosition();
			show = true;
		}

		public void Close(GameScenes data)
		{
			StatisticsTrackerScenario.toolbar.SetFalse(true);
		}

		public void Close()
		{
			show = false;
		}

		public void Toggle()
		{
			if (show)
				Close();
			else
				Open();
		}

		private void ResetWindowPosition()
		{
			windowPosition = StatGUIUtility.GetNormalizedRect(0.5f, 0.5f, 0.40f, 0.50f, 0.5f, 0.5f);
		}
	}
}
