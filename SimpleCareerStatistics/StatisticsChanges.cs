using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClickThroughFix;
using System;

namespace SimpleCareerStatistics
{
	abstract class StatisticsChanges : IEnumerable
	{
		private float previousValue;
		private readonly List<StatisticsChange> changes = new List<StatisticsChange>();

		private Vector2 scrollPosition;
		private float cachedChange;
		private int _sumCutoffIndex;

		private int SumCutoffIndex
		{
			get { return _sumCutoffIndex; }
			set
			{
				cachedChange = ChangeSum(value);
				_sumCutoffIndex = value;
			}
		}

		protected string FormatString => "N0";

		public abstract string DisplayName { get; }

		public abstract string SaveName { get; }

		public abstract void RegisterEvents();

		public abstract void DeregisterEvents();

		public abstract float CurrentValue();

		IEnumerator IEnumerable.GetEnumerator()
		{
			return changes.GetEnumerator();
		}

		public StatisticsChanges()
		{
			previousValue = CurrentValue();
		}

		public void AddValue(double time, float value, TransactionReasons reason)
		{
			var newChange = new StatisticsChange(time, value - previousValue, value, reason);
			previousValue = CurrentValue();
			if (changes.Count > 0 && changes[changes.Count - 1].CanMerge(newChange))
				changes[changes.Count - 1] = changes[changes.Count - 1].MergeWith(newChange);
			else
				changes.Add(newChange);
			SumCutoffIndex = SumCutoffIndex;
		}

		public ConfigNode ToNode()
		{
			var parent = new ConfigNode(SaveName);
			foreach (StatisticsChange change in changes) change.AddSelfToNode(parent); 
			return parent;
		}

		public void FromNode(ConfigNode node)
		{
			changes.Clear();
			ConfigNode statNode = null;
			if(node.TryGetNode(SaveName, ref statNode))
				foreach(string s in statNode.GetValuesList("v"))
					changes.Add(new StatisticsChange(s));
			SumCutoffIndex = SumCutoffIndex;
			OrderCheck();
		}

		public Rect DrawWindow(Rect original)
		{
			// I don't know what to put for id so I'll just use a random int
			return ClickThruBlocker.GUILayoutWindow(1234508710, original, (int id) => { DrawWindowContents(); }, DisplayName);
		}

		private void DrawWindowContents()
		{
			
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			GUILayout.BeginHorizontal();
			// Date and Time
			StatGUIUtility.DrawDataColumn(0, changes.Count - 1,
				(int i) => { return KSPUtil.dateTimeFormatter.PrintDateCompact(changes[i].Time, true); },
				(int i) => { return StatGUIUtility.DateText; },
				RowClick, PreRow
				);
			// Reason
			StatGUIUtility.DrawDataColumn(0, changes.Count - 1,
				(int i) => { return changes[i].Reason.ToString(); },
				(int i) => { return StatGUIUtility.DateText; },
				RowClick, PreRow
				);
			// Amount
			StatGUIUtility.DrawDataColumn(0, changes.Count - 1,
				(int i) => { return changes[i].After.ToString(FormatString); },
				(int i) => { return StatGUIUtility.DateText; },
				RowClick, PreRow
				);
			// Change
			StatGUIUtility.DrawDataColumn(0, changes.Count - 1,
				(int i) => { return changes[i].Change.ToString(StatGUIUtility.alwaysShowSign); },
				(int i) => { return StatGUIUtility.PosNegStyle(changes[i].Change); },
				RowClick, PreRow
				);
			GUILayout.EndHorizontal();
			GUILayout.EndScrollView();
			NetRow();
			GUI.DragWindow();
		}

		private void NetRow()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Marker Time:", StatGUIUtility.DateText, GUILayout.ExpandWidth(true));
			GUILayout.Label(changes.Count > SumCutoffIndex ? changes[SumCutoffIndex].Time.ToDate() : "No Marker", StatGUIUtility.DateTextField, GUILayout.ExpandWidth(true));
			GUILayout.Label("Net Change from Marker:", StatGUIUtility.DateText, GUILayout.ExpandWidth(true));
			GUILayout.Label(cachedChange.ToString(StatGUIUtility.alwaysShowSign), StatGUIUtility.PosNegStyle(cachedChange, true), GUILayout.ExpandWidth(true));
			GUILayout.EndHorizontal();
		}

		private void PreRow(int i)
		{
			if (i > 0 && i == SumCutoffIndex) GUILayout.Label("- - -", StatGUIUtility.DateText, GUILayout.ExpandWidth(true));
		}

		private void RowClick(int i)
		{
			SumCutoffIndex = i;
		}

		private float ChangeSum(int since)
		{
			float change = 0;
			for (int i = since; i < changes.Count; i++)
				change += changes[i].Change;
			return change;
		}

		private void OrderCheck()
		{
			for(int i = 0; i < changes.Count - 1; i++)
				if(changes[i+1].Time < changes[i].Time)
				{
					Debug.LogWarning("[SCS] Entries for " + SaveName + " is out of order");
					return;
				}
		}
	}
}
