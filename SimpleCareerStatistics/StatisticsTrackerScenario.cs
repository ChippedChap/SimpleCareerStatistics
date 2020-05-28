using KSP.UI.Screens;
using UnityEngine;
using ToolbarControl_NS;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace SimpleCareerStatistics
{
	[KSPScenario(ScenarioCreationOptions.AddToExistingCareerGames, new GameScenes[] {
		GameScenes.SPACECENTER,
		GameScenes.EDITOR,
		GameScenes.FLIGHT,
		GameScenes.TRACKSTATION,
	})]
	class StatisticsTrackerScenario : ScenarioModule
	{
		internal const string modNamespace = "StatisticsTrackerScenario";
		internal const string modName = "Simple Statistics Tracker";

		public static ToolbarControl toolbar;
		private StatisticsGUI guiDrawer;

		private readonly List<StatisticsChanges> changes = new List<StatisticsChanges>();

		#region monobehaviour callbacks
		public override void OnAwake()
		{
			SetupTrackers();
			foreach (StatisticsChanges tracker in changes) tracker.RegisterEvents();
			guiDrawer = gameObject.AddComponent<StatisticsGUI>();
			guiDrawer.trackers = changes;
			if(!toolbar) CreateButton();
		}

		public void OnDestroy()
		{
			foreach (StatisticsChanges tracker in changes) tracker.DeregisterEvents();
		}

		public override void OnSave(ConfigNode node)
		{
			base.OnSave(node);
			foreach (StatisticsChanges change in changes) node.AddNode(change.ToNode());
		}

		public override void OnLoad(ConfigNode node)
		{
			base.OnLoad(node);
			foreach (StatisticsChanges change in changes) change.FromNode(node);
		}
		#endregion

		private void CreateButton()
		{
			toolbar = gameObject.AddComponent<ToolbarControl>();
			toolbar.AddToAllToolbars(() => { guiDrawer.Open(); }, () => { guiDrawer.Close(); },
				ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.TRACKSTATION,
				modNamespace,
				"careerStatisticsTrackerButton",
				"SimpleCareerStatistics/PluginData/ShowTotalsTooltip",
				"SimpleCareerStatistics/PluginData/ShowTotalsTooltip",
				modName
				);
		}

		private void SetupTrackers()
		{
			foreach (Type tracker in ModInitialization.ChangesTypes)
				changes.Add((StatisticsChanges)Activator.CreateInstance(tracker));
		}
	}
}