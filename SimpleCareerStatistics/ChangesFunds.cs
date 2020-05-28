using UnityEngine;

namespace SimpleCareerStatistics
{
	class ChangesFunds : StatisticsChanges
	{
		public override string SaveName => "funds";

		public override string DisplayName => "Funding Report";

		public override float CurrentValue()
		{
			return (float)Funding.Instance.Funds;
		}

		public override void RegisterEvents()
		{
			GameEvents.OnFundsChanged.Add(FundsChanged);
		}

		public override void DeregisterEvents()
		{
			GameEvents.OnFundsChanged.Remove(FundsChanged);
		}

		private void FundsChanged(double finalValue, TransactionReasons reason)
		{
			AddValue(Planetarium.GetUniversalTime(), (float)finalValue, reason);
		}
	}
}
