using System;
using NRules.Fluent.Dsl;
using NRules.RuleModel;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;
using FFTAI.X.Trial.Step;

namespace FFTAI.X.TrialRules.Parameter
{
    public class ReceiveScriptRule : Rule
    {
        public override void Define()
        {
            Debug.Log($"Load Rule ---- [{this.GetType().Name}]");
            TrialEvent trialEvent = default;

            When()
                .Match(() => trialEvent);

            Filter()
                .Where(() => trialEvent.Type == TrialEventType.Reveive_Trial_Script, () => string.Equals(XCoreParameter.cdString[CDStringKeys.CurTrialState], TrialState.TRIAL_SCRIPT_WAITING.ToString()))
                .OnChange(() => trialEvent.TimeMilliseconds, () => trialEvent.Type);

            Then()
                .Do(ctx => ctx.Update(trialEvent, DoSomething));
        }
        void DoSomething(TrialEvent trialEvent)
        {
            XCoreParameter.TryUpdateCurState(TrialState.TRIAL_PARAM_LOADING);
        }
    }
}