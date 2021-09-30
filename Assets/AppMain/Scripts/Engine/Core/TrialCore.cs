using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFTAI.X.Trial.Step;
using FFTAICommunicationLib;

public class TrialCore : MonoBehaviour
{
    public XCore xCore;
    public static int TrialIndex
    {
        get
        {
            return m_TrialIndex;
        }
    }
    /// <summary>
    /// 试次序号增加1
    /// </summary>
    public static void TrialIndexGrow()
    {
        m_TrialIndex = m_TrialIndex + 1;
    }
    /// <summary>
    /// 试次序号重置
    /// </summary>
    public static void TrialIndexReset()
    {
        m_TrialIndex = 0;
    }
    private static int m_TrialIndex = 0;
    public static string onSet = "8";
    public static string offSet = "9";
    public static int TrialMode = 0; // 0 - X, 1 - RCT
    void Start()
    {
        Debug.Log($"TrialCore");

        Debug.Log($"Current RIDE SDK Version: {DynaLinkHS.RIDE.RIDEVersion}");
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            switch (XCore.CurAppState)
            {
                case ApplicationState.UNTRIALED:
                    SetScriptPath();
                    break;
                case ApplicationState.TRIALING:
                    if (TrialMode != 1)
                    {
                        DoTrialing();
                    }
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.Log($"{e.Message}\n{e.StackTrace}");
        }
    }
    void FixedUpdate()
    {
        try
        {
            switch (XCore.CurAppState)
            {
                case ApplicationState.TRIALING:
                    if (TrialMode == 1)
                    {
                        DoRCTTrialing();
                    }
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.Log($"{e.Message}\n{e.StackTrace}");
        }
    }
    private void DoRCTTrialing()
    {
        try
        {
            TrialState CurTrialState = (TrialState)Enum.Parse(typeof(TrialState), XCoreParameter.cdString[CDStringKeys.CurTrialState], true);
            switch (CurTrialState)
            {
                case TrialState.TRIAL_PARAM_LOADING:
                    CheckScriptLoadState();
                    break;
                case TrialState.START_POINT_MOVING:
                    Move2StartPosition();
                    break;
                case TrialState.END_POINT_DISPLAYING:
                    DisplayEndPoint();
                    break;
                case TrialState.TAKING_BREAK:
                    TakeBreak();
                    break;
                case TrialState.COMPLETING_TRIAL:
                    CompleteTrial();
                    break;
                case TrialState.QUITING_TRIAL:
                    xCore.QuitTrial();
                    break;
                default:
                    //Debug.Log($"CurTrialState: {CurTrialState}");
                    break;
            }
            switch (CurTrialState)
            {
                case TrialState.END_POINT_DISPLAYING:
                case TrialState.FREE_TRIALING:
                case TrialState.COUNTING_DOWN:
                    // string str = "";
                    // for (int i = 0; i < ParameterUtil.Resist[TrialIndex].Length; i++)
                    // {
                    //     str += ParameterUtil.Resist[TrialIndex][i].ToString("f6") + ",";
                    // }
                    // Debug.Log($"写入M2阻力: {str}");
                    var aaa = new float[10];
                    // ParameterUtil.Resist[TrialIndex].Take()

                    // aaa[1..^1]
                    xCore.ImposeResistanceWithMode(Convert.ToInt32(ParameterUtil.Resist[TrialIndex][0]), ParameterUtil.Resist[TrialIndex].Skip(1).Take(ParameterUtil.Resist[TrialIndex].Length - 1).ToArray());
                    break;
                default:
                    //Debug.Log($"CurTrialState: {CurTrialState}");
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.Log($"{e.Message}\n{e.StackTrace}");
        }
    }
    private void DoTrialing()
    {
        try
        {
            TrialState CurTrialState = (TrialState)Enum.Parse(typeof(TrialState), XCoreParameter.cdString[CDStringKeys.CurTrialState], true);
            switch (CurTrialState)
            {
                case TrialState.TRIAL_PARAM_LOADING:
                    CheckScriptLoadState();
                    break;
                case TrialState.START_POINT_MOVING:
                    Move2StartPosition();
                    break;
                case TrialState.END_POINT_DISPLAYING:
                    DisplayEndPoint();
                    break;
                case TrialState.TAKING_BREAK:
                    TakeBreak();
                    break;
                case TrialState.COMPLETING_TRIAL:
                    CompleteTrial();
                    break;
                case TrialState.QUITING_TRIAL:
                    ParameterUtil.Reset();
                    TrialIndexReset();
                    xCore.QuitTrial();
                    break;
                default:
                    //Debug.Log($"CurTrialState: {CurTrialState}");
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.Log($"{e.Message}\n{e.StackTrace}");
        }
    }
    private void SetScriptPath()
    {
        xCore.UpdateAppState(ApplicationState.TRIALING);
        // TODO 增加 OnDestroy 方法
        XCoreParameter.Init();
        EventDisruptor.Init();
        EventDisruptor.PublishTrialEvent(TrialEventType.Reveive_Trial_Script);
    }
    private void CheckScriptLoadState()
    {
        try
        {
            switch (ParameterUtil.StepState)
            {
                case TrialStepState.START_UP:
                    ParameterUtil.LoadTaskInfo();
                    break;
                case TrialStepState.DONE when IsDeviceReady():
                    EventDisruptor.PublishTrialEvent(TrialEventType.Trial_Script_Loaded);
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.Log($"{e.Message}\n{e.StackTrace}");
            ParameterUtil.Reset();
            xCore.UpdateAppState(ApplicationState.NONE);
        }
    }
    private bool IsDeviceReady()
    {
        return xCore.IsDeviceReady();
        // if (!Enum.Equals(XCore.DelsysEMGState, DeviceState.NONE))
        // {
        //     return (Enum.Equals(XCore.StickState, DeviceState.SERVING) && Enum.Equals(XCore.DelsysEMGState, DeviceState.SERVING));
        // }
        // return Enum.Equals(XCore.StickState, DeviceState.SERVING);
    }
    private void Move2StartPosition()
    {
        // 读取起点 TODO 需要修改比例
        float[] taskPointValue = ParameterUtil.startPointDic[TrialIndex];
        var m_StartPoint = new Vector2(taskPointValue[0], taskPointValue[1]);
        var distance = Vector2.Distance(MainUI.M2VecConvert2GameVec(m_StartPoint), xCore.HandPosition);
        // Debug.Log($"Distance: {distance}, StartPoint: {m_StartPoint} => {MainUI.M2VecConvert2GameVec(m_StartPoint)}, HandPosition: {xCore.HandPosition}");
        // var distance = 0.001f;
        if (distance <= MainUI.PanelRect.size.x * 0.007f)
        {
            EventDisruptor.PublishTrialEvent(TrialEventType.Stick_Start_Point_Arrived);
            onSet = ParameterUtil.markList[TrialIndex][0].ToString();
            offSet = ParameterUtil.markList[TrialIndex][1].ToString();
        }
        else
        {
            //Debug.Log($"Move stick to:{StartPoint}.");
            xCore.MoveStick(m_StartPoint);
        }
    }
    private void DisplayEndPoint()
    {
        var m_AimObject = GameObject.Find("AimObject");
        if (m_AimObject.GetComponent<Renderer>().enabled)
        {
            if (TrialMode != 1)
            {
                xCore.ImposeResistance(ParameterUtil.Resist[TrialIndex]);
            }

            EventDisruptor.PublishTrialEvent(TrialEventType.End_Point_Displayed);
        }
    }
    private void TakeBreak()
    {
        var stepTimer = XCoreParameter.GetDoubleValue(CDDoubleKeys.StepTimer) - Time.deltaTime;
        XCoreParameter.UpdateDoubleValue(CDDoubleKeys.StepTimer, stepTimer);
        if (stepTimer < 0)
        {
            EventDisruptor.PublishTrialEvent(TrialEventType.Had_Taken_Break);
        }
    }
    private void CompleteTrial()
    {
        var stepTimer = XCoreParameter.GetDoubleValue(CDDoubleKeys.StepTimer) - Time.deltaTime;
        XCoreParameter.UpdateDoubleValue(CDDoubleKeys.StepTimer, stepTimer);
        if (stepTimer < 0)
        {
            EventDisruptor.PublishTrialEvent(TrialEventType.Had_Completed_Trial);
        }
    }
}
