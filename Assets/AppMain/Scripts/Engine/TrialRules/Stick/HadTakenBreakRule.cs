using System;
using NRules.Fluent.Dsl;
using NRules.RuleModel;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;
using FFTAI.X.Trial.Step;

namespace FFTAI.X.TrialRules.Stick
{
    public class HadTakenBreakRule : Rule
    {
        public override void Define()
        {
            Debug.Log($"Load Rule ---- [{this.GetType().Name}]");
            TrialEvent trialEvent = default;

            When()
                .Match(() => trialEvent);

            Filter()
                .Where(() => trialEvent.Type == TrialEventType.Had_Taken_Break, () => string.Equals(XCoreParameter.cdString[CDStringKeys.CurTrialState], TrialState.TAKING_BREAK.ToString()))
                .OnChange(() => trialEvent.TimeMilliseconds, () => trialEvent.Type);

            Then()
                .Do(ctx => ctx.Update(trialEvent, DoSomething));
        }
        void DoSomething(TrialEvent trialEvent)
        {
            if (TrialCore.TrialIndex >= ParameterUtil.trialNum - 1)
            {
                XCoreParameter.UpdateDoubleValue(CDDoubleKeys.StepTimer, XCoreParameter.GetDoubleValue(CDDoubleKeys.TakeBreakTime));
                XCoreParameter.TryUpdateCurState(TrialState.COMPLETING_TRIAL);
            }
            else
            {
                TrialCore.TrialIndexGrow();
                XCoreParameter.UpdateDoubleValue(CDDoubleKeys.CircleTimer, XCoreParameter.GetDoubleValue(CDDoubleKeys.CircleTime));
                XCoreParameter.TryUpdateCurState(TrialState.START_POINT_MOVING);
            }
        }
    }
}