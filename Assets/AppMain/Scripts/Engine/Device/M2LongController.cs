using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using System.Text;
using Ping = System.Net.NetworkInformation.Ping;
using System.Threading.Tasks;
using FFTAICommunicationLib;
using System.Threading;
using System.IO;

public class M2LongController : MonoBehaviour
{
    private static string DeviceName = "M2";
    private static readonly string M2IP = "192.168.102.200";
    private static float[] xRange;
    private static float[] yRange;
    // private static readonly string M2IP = "127.0.0.1";
    public XCore xCore;
    private DeviceState M2CurState
    {
        get
        {
            return xCore.Devices[DeviceName];
        }
    }
    System.Timers.Timer recordTimer;
    public StringBuilder csvContent = new StringBuilder();

    void Start()
    {
        xCore.AddDevice(DeviceName);
    }
    private void ShareActionWithXCore()
    {
        try
        {
            Debug.Log($"Bind [{DeviceName}] Func");
            xCore.MoveStick += MoveStick;
            xCore.ImposeResistance += ImposeResistance;
            xCore.ImposeResistanceWithMode += ImposeResistanceWithMode;
        }
        catch (Exception e)
        {
            Debug.Log($"{e.Message}\n{e.StackTrace}");
        }
    }
    void Update()
    {
        try
        {
            switch (XCore.CurAppState)
            {
                case ApplicationState.TRIALING:
                    CheckM2();
                    break;
                default:
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.Log($"{e.Message}\n{e.StackTrace}");
        }
    }
    private void OnDestroy()
    {
        UpdateDeviceState(DeviceState.NONE);
        Debug.Log(DynaLinkHS.Disconnect());
        if (recordTimer != null)
        {
            recordTimer.Stop();
            recordTimer = null;
        }
    }
    private void UpdateDeviceState(DeviceState newDeviceState)
    {
        xCore.UpdateDeviceState(DeviceName, newDeviceState);
    }
    private void CheckM2()
    {
        switch (M2CurState)
        {
            case DeviceState.UNCHECKED:
                CheckM2Exist();
                break;
            case DeviceState.CHECKED:
                ConnectM2();
                break;
            case DeviceState.CONNECTED:
                InitializeM2();
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
                M2PauseMotion();
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
            string csvfullfilename = $"{dir}\\LongRun_{TrialCore.TrialIndex + 1}_{DeviceName}.csv";
            File.WriteAllText(csvfullfilename, $"Time(ms)-{XCoreParameter.GetLongValue(CDLongKeys.trialStartTime)},Trial,State,PositionX,PositionY,ForceX,ForceY\n");
            File.AppendAllText(csvfullfilename, csvContent.ToString());
            StopRecordAndClearList();
        }
    }
    private void StopRecordAndClearList()
    {
        csvContent.Clear();
    }
    public void RecordOneRow(object source, System.Timers.ElapsedEventArgs e)
    {
        try
        {
            TrialState CurTrialState = (TrialState)Enum.Parse(typeof(TrialState), XCoreParameter.cdString[CDStringKeys.CurTrialState], true);
            switch (CurTrialState)
            {
                case TrialState.START_POINT_MOVING:
                case TrialState.END_POINT_DISPLAYING:
                case TrialState.FREE_TRIALING:
                case TrialState.COUNTING_DOWN:
                case TrialState.TAKING_BREAK:
                    var currentTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
                    var startTime = XCoreParameter.GetLongValue(CDLongKeys.trialStartTime);
                    var spentTime = currentTime - startTime;
                    csvContent.AppendLine($"{currentTime},{TrialCore.TrialIndex + 1},{XCoreParameter.cdString[CDStringKeys.CurTrialState]},{DynaLinkHS.StatusRobot.PositionDataJoint1},{DynaLinkHS.StatusRobot.PositionDataJoint2},{DynaLinkHS.StatusSensor.ADCSensor1.CalculateValue},{DynaLinkHS.StatusSensor.ADCSensor2.CalculateValue}");
                    break;
                default:
                    break;
            }
        }
        catch (Exception err)
        {
            Debug.Log($"{err.Message}\n{err.StackTrace}");
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
    private void CheckM2Exist()
    {
        UpdateDeviceState(DeviceState.CHECKING);
        Task.Run(() =>
        {
            // 确认M2已开机
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();

            options.DontFragment = true;

            string data = "I'm ok";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 2;
            PingReply reply = pingSender.Send(M2IP, timeout, buffer, options);
            // 1.5秒后确认结果
            System.Threading.Thread.Sleep(1500);
            if (reply.Status == IPStatus.Success)
            {
                Debug.Log($"Address: {reply.Address}");
                UpdateDeviceState(DeviceState.CHECKED);
            }
            else
            {
                UpdateDeviceState(DeviceState.UNCHECKED);
            }
        });
    }
    static private bool IsConnected
    {
        get
        {
            if (DynaLinkHS.StatusFlag.FlagServerLinkActive == 0)
            {
                return false;
            }
            return true;
        }
    }
    private void ConnectM2()
    {
        UpdateDeviceState(DeviceState.CONNECTING);
        Task.Run(() =>
        {
            {
                Debug.Log($"Connecting M2 now!");
                int count = 1;
                while (XCore.CurAppState == ApplicationState.TRIALING && M2CurState == DeviceState.CONNECTING)
                {
                    if (IsConnected)
                    {
                        Debug.Log($"M2HasConnected");
                        M2PauseMotion();
                        xRange = new float[2];
                        xRange[0] = DynaLinkHS.StatusRobot.PositionLimitationDataEndEffectorX1.Min;
                        xRange[1] = DynaLinkHS.StatusRobot.PositionLimitationDataEndEffectorX1.Max;
                        yRange = new float[2];
                        yRange[0] = DynaLinkHS.StatusRobot.PositionLimitationDataEndEffectorY1.Min;
                        yRange[1] = DynaLinkHS.StatusRobot.PositionLimitationDataEndEffectorY1.Max;
                        UpdateDeviceState(DeviceState.CONNECTED);
                        break;
                    }
                    // TODO 可适当增加 重试的次数
                    else if (!IsConnected && count < 2)
                    {
                        count++;
                        Debug.Log("Version3 Flag: " + DynaLinkHS.CmdSetProtocolVersion(DynaLinkHS.ProtocolVersion.Version3));
                        Debug.Log("Connect Flag: " + DynaLinkHS.Connect());
                        System.Threading.Thread.Sleep(2500);
                    }
                    else
                    {
                        Debug.Log($"M2Connected Failed");
                        UpdateDeviceState(DeviceState.CHECKED);
                        break;
                    }
                }
                Debug.Log($"ConnectM2 is over~");
            }
        });
    }
    public static void M2PauseMotion()
    {
        // DynaLinkHS.CmdServoOff();
        DynaLinkHS.CmdPauseMotion();
    }
    private void InitializeM2()
    {
        if (!IsConnected)
        {
            // M2未连接，则退回到初始状态
            UpdateDeviceState(DeviceState.UNCHECKED);
            return;
        }
        UpdateDeviceState(DeviceState.INITING);
        Task.Run(() =>
        {
            {
                DynaLinkHS.CmdServoOff();
                Debug.Log($"CmdFindHomePosition Flag: {DynaLinkHS.CmdFindHomePosition()}");
                var initCountTime = 0f;
                var sleepTime = 500;
                while (XCore.CurAppState == ApplicationState.TRIALING && M2CurState == DeviceState.INITING)
                {
                    System.Threading.Thread.Sleep(sleepTime);
                    initCountTime += sleepTime;
                    if (DynaLinkHS.StatusFlag.FlagCalibration == 1)
                    {
                        AfterM2Initialized();
                        // 连接并初始化后，进入工作状态
                        UpdateDeviceState(DeviceState.SERVING);
                        break;
                    }
                    else
                    {
                        // 长时间未初始化，则退回到CONNECTED状态
                        if (initCountTime >= 11000f)
                        {
                            Debug.Log($"Initialize M2 Failed");
                            UpdateDeviceState(DeviceState.CONNECTED);
                        }
                    }
                }
            }
        });
    }
    private void AfterM2Initialized()
    {
        M2PauseMotion();
        ShareActionWithXCore();
    }
    private void UpdateStickPosition()
    {
        var position = GetM2ScreenPosition();
        xCore.UpdateStickPosition(position.x, position.y);
    }
    private Vector2 GetM2ScreenPosition()
    {
        // var x = ((DynaLinkHS.StatusRobot.PositionDataJoint1 - xRange[0]) / (xRange[1] - xRange[0]));
        var x = DynaLinkHS.StatusRobot.PositionDataJoint1;
        // var y = ((DynaLinkHS.StatusRobot.PositionDataJoint2 - yRange[0]) / (yRange[1] - yRange[0]));
        var y = DynaLinkHS.StatusRobot.PositionDataJoint2;
        var position = new Vector2(x, y);
        return MainUI.M2VecConvert2GameVec(position);
    }
    private int MoveStick(Vector2 targetPoint)
    {
        //var distance = Vector2.Distance(xCore.GetHandlePosition(), vector2);
        if (Math.Abs(DynaLinkHS.StatusRobot.VelocityDataJoint1) <= 0.01f && Math.Abs(DynaLinkHS.StatusRobot.VelocityDataJoint2) <= 0.01f)
        {
            DynaLinkHS.CmdPassiveMovementControl1(targetPoint.x, targetPoint.y, 0.08f, 0.01f);
        }
        return (int)FunctionResult.SUCCESS;
    }
    private int ImposeResistance(params float[] parameters)
    {
        // Debug.Log($"parameters:{parameters}");
        return ImposeResistanceWithMode(0, parameters);
    }
    private int ImposeResistanceWithMode(int mode, params float[] parameters)
    {
        Debug.Log($"mode: {mode},parameters:{parameters}");
        int result = 0;
        var xF = DynaLinkHS.StatusSensor.ADCSensor1.CalculateValue;
        var yF = DynaLinkHS.StatusSensor.ADCSensor2.CalculateValue;
        switch (mode)
        {
            case 0:
            default:
                // result = DynaLinkHS.CmdTransparentControl(parameters);
                result = DynaLinkHS.CmdTransparentControlWithShakeEliminate(parameters);
                break;
            case 1:
                Debug.Log($"当前的力: ({xF}, {yF})");
                // result = DynaLinkHS.CmdTransparentControlWithExternalForce(parameters);
                if (xF >= 3f || yF >= 3f)
                {
                    parameters[4] = xF > 0 ? -parameters[4] : parameters[4];
                    parameters[5] = yF > 0 ? parameters[5] : -parameters[5];
                    // 主动-透明控制-外力导入
                    result = DynaLinkHS.CmdTransparentControlWithExternalForce(parameters);
                }
                else
                {
                    parameters[4] = 0F;
                    parameters[5] = 0F;
                    result = DynaLinkHS.CmdTransparentControlWithExternalForce(parameters);
                }
                break;
            case 2:
                // 助力-透明控制-限速
                result = DynaLinkHS.CmdTransparentControlWithLimitSpeed(parameters);
                break;
            case 3:
                Debug.Log($"当前的力: ({xF}, {yF})");
                // result = DynaLinkHS.CmdTransparentControlWithExternalForce(parameters);
                if (xF >= 3f || yF >= 3f)
                {
                    // 主动-力量辅助控制
                    // result = DynaLinkHS.CmdAssistControlWithSensor(parameters);
                    result = DynaLinkHS.CmdKineticControlWithSensor(parameters);
                }
                else
                {
                    // DynaLinkHS.CmdServoOff();
                    parameters[0] = 0F;
                    parameters[1] = 0F;
                    // result = DynaLinkHS.CmdAssistControlWithSensor(parameters);
                    result = DynaLinkHS.CmdKineticControlWithSensor(parameters);
                }
                break;
        }
        return result;
    }
}
