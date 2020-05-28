using UnityEngine;
using ToolbarControl_NS;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace SimpleCareerStatistics
{
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	class ModInitialization : MonoBehaviour
	{
		public static List<Type> ChangesTypes { get; private set; }

		public void Start()
		{
			ToolbarControl.RegisterMod(StatisticsTrackerScenario.modNamespace, StatisticsTrackerScenario.modName);
			ChangesTypes = GetAllChangeClasses();
			StatGUIUtility.CreateStyles(HighLogic.Skin);
		}

		private List<Type> GetAllChangeClasses()
		{
			var types = new List<Type>();
			foreach (Assembly assy in AppDomain.CurrentDomain.GetAssemblies())
				foreach (Type type in assy.GetTypes())
					if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(StatisticsChanges)))
						types.Add(type);
			return types;
		}
	}
}
