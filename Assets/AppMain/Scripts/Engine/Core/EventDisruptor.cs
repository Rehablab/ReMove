using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using NRules;
using NRules.Diagnostics;
using NRules.Fluent;
using NRules.Fluent.Dsl;
using Disruptor;
using Disruptor.Dsl;
using static XCoreParameter;

public class EventDisruptor
{
    private static Disruptor<TrialEvent> m_TrialDisruptor;
    public static void Init()
    {
        try
        {
            TrialEventHandler.Init();
            // m_TrialDisruptor = new Disruptor<TrialEvent>(() => new TrialEvent(), 256, TaskScheduler.Default);
            m_TrialDisruptor = new Disruptor<TrialEvent>(() => new TrialEvent(), ringBufferSize: 256);
            m_TrialDisruptor.HandleEventsWith(new TrialEventHandler());

            m_TrialDisruptor.Start();
        }
        catch (Exception e)
        {
            Debug.LogError($"{e.Message}\n{e.StackTrace}");
        }
    }
    public static void PublishTrialEvent(TrialEventType Type)
    {
        // Debug.Log($"GetRemainingCapacity: {GameDisruptor.RingBuffer.GetRemainingCapacity()}");
        using (var scope = m_TrialDisruptor.PublishEvent())
        {
            var trialEvent = scope.Event();
            trialEvent.Type = Type;
            trialEvent.TimeMilliseconds = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        }
    }
}
public class TrialEventHandler : IEventHandler<TrialEvent>
{
    private static ISession session;
    public static void Init()
    {
        //Load rules
        var repository = new RuleRepository();
        repository.Load(x => x.From(Assembly.GetExecutingAssembly()));

        //Compile rules
        var factory = repository.Compile();

        //Create a working session
        session = factory.CreateSession();

        // session.Events.FactInsertingEvent += EventProviderOnFactInsertingEvent;
        // session.Events.FactUpdatingEvent += EventProviderOnFactUpdatingEvent;
        // session.Events.FactRetractingEvent += EventProviderOnFactRetractingEvent;
        // session.Events.ActivationCreatedEvent += EventProviderOnActivationCreatedEvent;
        // session.Events.ActivationUpdatedEvent += EventProviderOnActivationUpdatedEvent;
        // session.Events.ActivationDeletedEvent += EventProviderOnActivationDeletedEvent;
        // session.Events.RuleFiringEvent += EventProviderOnRuleFiringEvent;

        // var resuleNum = session.Fire();
        // Debug.Log($"Result is : {resuleNum}");
        session.Insert(XCoreParameter.cdString);
    }
    private static void TryInsertFact(TrialEvent gameEvent)
    {
        try
        {
            session.Insert(gameEvent);
        }
        catch (Exception e)
        {
            Debug.LogError($"{e.Message}");
        }
    }
    private static void RuleEngnieFire()
    {
        try
        {
            var num = session.Fire();
            // Debug.Log($"命中了 {num} 条规则");
        }
        catch (Exception e)
        {
            Debug.LogError($"{e.Message}");
        }
    }
    public void OnEvent(TrialEvent data, long sequence, bool endOfBatch)
    {
        // Debug.Log($"Event: {data.Type} => {data.TimeMilliseconds}, sequence:{sequence},endOfBatch:{endOfBatch}");
        try
        {
            bool flag = false;
            switch (data.Type)
            {
                case TrialEventType.Reveive_Trial_Script:
                case TrialEventType.Trial_Script_Loaded:
                case TrialEventType.Stick_Start_Point_Arrived:
                case TrialEventType.End_Point_Displayed:
                case TrialEventType.End_Point_Arrived:
                case TrialEventType.End_Point_Exited:
                case TrialEventType.End_Point_Hovered:
                case TrialEventType.Had_Taken_Break:
                case TrialEventType.Had_Completed_Trial:
                    flag = NormalInsertFact(data);
                    break;
                default: break;
            }
            if (flag)
            {
                RuleEngnieFire();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"{e.Message}");
            Debug.LogError($"Type => {data.Type} , TimeMilliseconds => {data.TimeMilliseconds}");
        }
    }
    private bool NormalInsertFact(TrialEvent trialEvent)
    {
        var newEvent = new TrialEvent();
        newEvent.Type = trialEvent.Type;
        newEvent.TimeMilliseconds = trialEvent.TimeMilliseconds;
        TryInsertFact(newEvent);
        return true;
    }
}
