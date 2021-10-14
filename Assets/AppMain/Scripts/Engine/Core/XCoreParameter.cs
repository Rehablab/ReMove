using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;

public class XCoreParameter
{
    public static ConcurrentDictionary<string, string> cdString = new ConcurrentDictionary<string, string>();
    public static ConcurrentDictionary<string, int> cdInt = new ConcurrentDictionary<string, int>();
    public static ConcurrentDictionary<string, double> cdDouble = new ConcurrentDictionary<string, double>();
    public static ConcurrentDictionary<string, long> cdLong = new ConcurrentDictionary<string, long>();
    public static ConcurrentDictionary<string, float> cdFloat = new ConcurrentDictionary<string, float>();
    public static void Init()
    {
        cdString[CDStringKeys.CurTrialState] = TrialState.TRIAL_SCRIPT_WAITING.ToString();

        cdDouble[CDDoubleKeys.CircleTime] = 1.2d; // 圆形进度条等待时间
        cdDouble[CDDoubleKeys.TakeBreakTime] = 2; // 上流程结束后 Break等待的时间
    }
    public static void TryUpdateCurState(TrialState trialState)
    {
        Debug.Log($"CurTrialState From [{cdString[CDStringKeys.CurTrialState]}] => [{trialState}]");
        cdString[CDStringKeys.CurTrialState] = trialState.ToString();
    }
    public static double GetDoubleValue(string tKey)
    {
        return cdDouble[tKey];
    }
    public static void UpdateDoubleValue(string tKey, double tValue)
    {
        cdDouble[tKey] = tValue;
    }
    public static long GetLongValue(string tKey)
    {
        return cdLong[tKey];
    }
    public static void UpdateLongValue(string tKey, long tValue)
    {
        cdLong[tKey] = tValue;
    }
}
public enum TrialEventType
{
    Reveive_Trial_Script, // 收到实验脚本
    Trial_Script_Loaded,
    Stick_Start_Point_Arrived,
    End_Point_Displayed,
    End_Point_Arrived,
    End_Point_Exited,
    End_Point_Hovered,
    Had_Taken_Break,
    Had_Completed_Trial,
}
public class TrialEvent
{
    public TrialEventType Type { get; set; }
    public long TimeMilliseconds { get; set; }
}
public enum ApplicationState
{
    NONE, // 刚打开，等待收到后续指令
    UNTRIALED, // 未做实验，准备做实验
    TRIALING, // 正在做实验
}
public enum TrialState
{
    TRIAL_SCRIPT_WAITING,

    TRIAL_PARAM_LOADING,

    START_POINT_MOVING,

    END_POINT_DISPLAYING,

    FREE_TRIALING,

    COUNTING_DOWN,

    TAKING_BREAK,

    COMPLETING_TRIAL,

    QUITING_TRIAL,
}
public enum TrialStepState
{
    START_UP,
    PENDING,
    DONE,
}
public class CDStringKeys
{
    public static string CurTrialState = "TrialState";
}
public class CDDoubleKeys
{
    public static string CircleTime = "CircleTime";
    public static string CircleTimer = "CircleTimer";
    public static string TakeBreakTime = "TakeBreakTime";
    public static string StepTimer = "StepTimer";
}
public class CDLongKeys
{
    public static string trialStartTime = "trialStartTime";
}
public enum FunctionResult
{
    SUCCESS = 1,
    FAIL = 0
}
public enum DeviceState
{
    NONE,
    UNCHECKED,
    CHECKING,
    CHECKED,
    CONNECTING,
    CONNECTED,
    INITING,
    SERVING,
}
