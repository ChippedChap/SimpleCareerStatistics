using System;
using UnityEngine;

namespace SimpleCareerStatistics
{
	struct StatisticsChange
	{
		private static readonly double threshold = 10E-4;

		public double Time { get; private set; }
		public float Change { get; private set; }
		public float After { get; private set; }
		public TransactionReasons Reason { get; private set; }

		public StatisticsChange(double time, float change, float after, TransactionReasons reason)
		{
			Time = time;
			Change = change;
			After = after;
			Reason = reason;
		}

		public StatisticsChange(string nodeKey)
		{
			Vector4 vec = ConfigNode.ParseVector4(nodeKey);
			Time = vec.x;
			Change = vec.y;
			After = vec.z;
			Reason = (TransactionReasons)vec.w;
		}

		public void AddSelfToNode(ConfigNode addee)
		{
			addee.AddValue("v", new Vector4((float)Time, Change, After, (int)Reason));
		}

		public bool CanMerge(StatisticsChange other)
		{
			return Math.Abs(other.Time - Time) < threshold &&
				other.Reason == Reason && 
				Mathf.Sign(Change) == Mathf.Sign(other.Change);
		}

		public StatisticsChange MergeWith(StatisticsChange other)
		{
			return new StatisticsChange(Math.Max(Time, other.Time), 
				Change + other.Change, 
				other.Time > Time ? other.After : After, 
				Reason);
		}
	}
}
