using System;
using NRules.Fluent.Dsl;
using NRules.RuleModel;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;
using FFTAI.X.Trial.Step;

namespace FFTAI.X.TrialRules.Stick
{
    public class EndPointHoveredRule : Rule
    {
        public override void Define()
        {
            Debug.Log($"Load Rule ---- [{this.GetType().Name}]");
            TrialEvent trialEvent = default;

            When()
                .Match(() => trialEvent);

            Filter()
                .Where(() => trialEvent.Type == TrialEventType.End_Point_Hovered, () => string.Equals(XCoreParameter.cdString[CDStringKeys.CurTrialState], TrialState.COUNTING_DOWN.ToString()))
                .OnChange(() => trialEvent.TimeMilliseconds, () => trialEvent.Type);

            Then()
                .Do(ctx => ctx.Update(trialEvent, DoSomething));
        }
        void DoSomething(TrialEvent trialEvent)
        {
            XCoreParameter.UpdateDoubleValue(CDDoubleKeys.StepTimer, XCoreParameter.GetDoubleValue(CDDoubleKeys.TakeBreakTime));
            XCoreParameter.TryUpdateCurState(TrialState.TAKING_BREAK);
        }
    }
}