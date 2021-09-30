using System;
using NRules.Fluent.Dsl;
using NRules.RuleModel;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;
using FFTAI.X.Trial.Step;

namespace FFTAI.X.TrialRules.Parameter
{
    public class ScriptLoadedRule : Rule
    {
        public override void Define()
        {
            Debug.Log($"Load Rule ---- [{this.GetType().Name}]");
            TrialEvent trialEvent = default;

            When()
                .Match(() => trialEvent);

            Filter()
                .Where(() => trialEvent.Type == TrialEventType.Trial_Script_Loaded, () => string.Equals(XCoreParameter.cdString[CDStringKeys.CurTrialState], TrialState.TRIAL_PARAM_LOADING.ToString()))
                .OnChange(() => trialEvent.TimeMilliseconds, () => trialEvent.Type);

            Then()
                .Do(ctx => ctx.Update(trialEvent, DoSomething));
        }
        void DoSomething(TrialEvent trialEvent)
        {
            XCoreParameter.UpdateLongValue(CDLongKeys.trialStartTime, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds());
            XCoreParameter.TryUpdateCurState(TrialState.START_POINT_MOVING);
        }
    }
}