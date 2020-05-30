using UnityEngine;
using System.Collections.Generic;
using ClickThroughFix;

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
			// I don't know what to put for id so I'll just use random ints
			windowPosition = trackers[currentTracker].DrawWindow(windowPosition, 1234508710);
			windowPosition.DrawWindowWidgets(224508712, GUI.skin.window, StatGUIUtility.PadlessButton, new string[] { "X" }, () => { Close(GameScenes.LOADING); });
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
