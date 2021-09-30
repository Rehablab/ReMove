using System;
using NRules.Fluent.Dsl;
using NRules.RuleModel;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;
using FFTAI.X.Trial.Step;

namespace FFTAI.X.TrialRules.Stick
{
	public class EndPointArrivedRule : Rule
	{
		public override void Define()
		{
			Debug.Log($"Load Rule ---- [{this.GetType().Name}]");
			TrialEvent trialEvent = default;

			When()
				.Match(() => trialEvent);

			Filter()
				.Where(() => trialEvent.Type == TrialEventType.End_Point_Arrived, () => string.Equals(XCoreParameter.cdString[CDStringKeys.CurTrialState], TrialState.FREE_TRIALING.ToString()))
				.OnChange(() => trialEvent.TimeMilliseconds, () => trialEvent.Type);

			Then()
				.Do(ctx => ctx.Update(trialEvent, DoSomething));
		}
		void DoSomething(TrialEvent trialEvent)
		{
			XCoreParameter.UpdateDoubleValue(CDDoubleKeys.CircleTimer, XCoreParameter.GetDoubleValue(CDDoubleKeys.CircleTime));
			XCoreParameter.TryUpdateCurState(TrialState.COUNTING_DOWN);
		}
	}
}