using FFTAICommunicationLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class MouseLongController : MonoBehaviour
{
    public XCore xCore;
    private static string DeviceName = "Mouse";
    public StringBuilder csvContent = new StringBuilder();

    private DeviceState MouseCurState
    {
        get
        {
            return xCore.Devices[DeviceName];
        }
    }
    void Start()
    {
        xCore.AddDevice(DeviceName);
    }

    void Update()
    {
        try
        {
            switch (XCore.CurAppState)
            {
                case ApplicationState.TRIALING:
                    CheckMouse();
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"{e.Message}");
        }
    }
    private void UpdateDeviceState(DeviceState newDeviceState)
    {
        xCore.UpdateDeviceState(DeviceName, newDeviceState);
    }
    private void CheckMouse()
    {
        switch (MouseCurState)
        {
            case DeviceState.UNCHECKED:
                ShareActionWithXCore();
                UpdateDeviceState(DeviceState.SERVING);
                break;
            case DeviceState.SERVING:
                UpdateStickPosition();
                RecordData();
                break;
        }
    }
    private void RecordData()
    {
        TrialState CurTrialState = (TrialState)Enum.Parse(typeof(TrialState), XCoreParameter.cdString[CDStringKeys.CurTrialState], true);
        switch (CurTrialState)
        {
            case TrialState.TRIAL_PARAM_LOADING:
            case TrialState.START_POINT_MOVING:
            case TrialState.END_POINT_DISPLAYING:
                OpenRecordLoop();
                break;
            case TrialState.FREE_TRIALING:
            case TrialState.COUNTING_DOWN:
                // Save Data To Memory
                break;
            case TrialState.TAKING_BREAK:
                break;
            case TrialState.COMPLETING_TRIAL:
                SaveDataToFile();
                OnDestroy();
                break;
            default:
                // Do Nothing
                break;
        }
    }
    private void SaveDataToFile()
    {
        if (csvContent != null && csvContent.Length != 0)
        {
            var dir = System.Environment.CurrentDirectory + $"\\ReachingExp\\P2P";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            Directory.CreateDirectory(dir);
            string csvfullfilename = $"{dir}\\LongRun_{TrialCore.TrialIndex + 1}_{DeviceName}.csv";
            File.WriteAllText(csvfullfilename, $"Time(ms)-{XCoreParameter.GetLongValue(CDLongKeys.trialStartTime)},Trial,State,PositionX,PositionY\n");
            File.AppendAllText(csvfullfilename, csvContent.ToString());
            StopRecordAndClearList();
        }
    }
    private void StopRecordAndClearList()
    {
        csvContent.Clear();
    }
    System.Timers.Timer recordTimer;
    public void RecordOneRow(object source, System.Timers.ElapsedEventArgs e)
    {
        try
        {
            TrialState CurTrialState = (TrialState)Enum.Parse(typeof(TrialState), XCoreParameter.cdString[CDStringKeys.CurTrialState], true);
            switch (CurTrialState)
            {
                case TrialState.FREE_TRIALING:
                case TrialState.COUNTING_DOWN:
                    var currentTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
                    var startTime = XCoreParameter.GetLongValue(CDLongKeys.trialStartTime);
                    var spentTime = currentTime - startTime;
                    var position = MainUI.GameVecConvert2RelativeVec(xCore.HandPosition);
                    csvContent.AppendLine($"{currentTime},{TrialCore.TrialIndex + 1},{XCoreParameter.cdString[CDStringKeys.CurTrialState]},{position.x},{position.y}");
                    break;
                default:
                    break;
            }
        }
        catch (Exception err)
        {
            Debug.LogError($"{err.Message}");
        }
    }
    private void OpenRecordLoop()
    {
        if (recordTimer == null)
        {
            recordTimer = new System.Timers.Timer(10);//实例化Timer类，设置间隔时间为10毫秒；
            recordTimer.Elapsed += new System.Timers.ElapsedEventHandler(RecordOneRow);//到达时间的时候执行事件；
            recordTimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
            recordTimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
            recordTimer.Start(); //启动定时器
        }
    }
    private void ShareActionWithXCore()
    {
        try
        {
            Debug.Log($"Bind [Mouse] Func");
            xCore.MoveStick += MoveStick;
            xCore.ImposeResistance += ImposeResistance;
            xCore.ImposeResistanceWithMode += ImposeResistanceWithMode;
        }
        catch (Exception e)
        {
            Debug.LogError($"{e.Message}");
        }
    }
    private void UpdateStickPosition()
    {
        var position = GetMouseScreenPosition();
        xCore.UpdateStickPosition(position.x, position.y);
    }
    private Vector3 GetMouseScreenPosition()
    {
        var mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, MainUI.PanelRect.xMin, MainUI.PanelRect.xMax);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, MainUI.PanelRect.yMin, MainUI.PanelRect.yMax);
        return mouseScreenPosition;
    }
    [DllImport("user32.dll")] //强制设置鼠标坐标
    public static extern int SetCursorPos(int x, int y);
    private int MoveStick(Vector2 vector2)
    {
        // TODO 鼠标暂时不移动
        return (int)FunctionResult.SUCCESS;
    }
    private int ImposeResistance(float[] param)
    {
        // Do Nothing
        return (int)FunctionResult.SUCCESS;
    }
    private void OnDestroy()
    {
        UpdateDeviceState(DeviceState.NONE);
        if (recordTimer != null)
        {
            recordTimer.Stop();
            recordTimer = null;
        }
    }
    private int ImposeResistanceWithMode(int mode, params float[] parameters)
    {
        Debug.Log($"mode: {mode},parameters:{parameters}");
        int result = 0;
        return result;
    }
}
