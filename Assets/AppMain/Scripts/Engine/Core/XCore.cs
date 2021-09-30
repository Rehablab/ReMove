/*
 * @Author: Qiang
 * @Date: 2021-05-10 15:06:15
 * @LastEditTime: 2021-09-23 17:20:45
 * @LastEditors: Ryan
 * @Description: 
 * @FilePath: \X-Engine\Assets\AppMain\Scripts\Engine\Core\XCore.cs
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static XCoreParameter;
using System.Collections.Concurrent;

public class XCore : MonoBehaviour
{
    public Func<Vector2, int> MoveStick;
    public Func<float[], int> ImposeResistance;
    public Func<int, float[], int> ImposeResistanceWithMode;
    // 应用全局状态
    public static ApplicationState CurAppState = ApplicationState.NONE;

    private static Vector2 m_HandPosition = new Vector2(0f, 0f);
    private ConcurrentDictionary<string, DeviceState> m_Devices = new ConcurrentDictionary<string, DeviceState>();
    public ConcurrentDictionary<string, DeviceState> Devices
    {
        get
        {
            return m_Devices;
        }
    }
    public Vector2 HandPosition
    {
        get
        {
            return m_HandPosition;
        }
    }
    /// <summary>
    /// 添加设备
    /// </summary>
    /// <param name="deviceName"></param>
    public void AddDevice(string deviceName)
    {
        AddDevice(deviceName, DeviceState.NONE);
    }
    /// <summary>
    /// XCore添加设备
    /// </summary>
    /// <param name="deviceName"></param>
    /// <param name="m_DeviceState"></param>
    private void AddDevice(string deviceName, DeviceState m_DeviceState)
    {
        if (m_Devices.TryAdd(deviceName, m_DeviceState))
        {
            Debug.Log($"AddDevices Success! [{deviceName}]");
        }
    }
    /**
     * @description: 移除设备
     * @param {*}
     * @return {*}
     */
    public void RemoveDevice(string deviceName)
    {
        m_Devices.TryRemove(deviceName, out _);
    }
    /// <summary>
    /// 更新设备状态
    /// </summary>
    /// <param name="deviceName"></param>
    /// <param name="newDeviceState"></param>
    public void UpdateDeviceState(string deviceName, DeviceState newDeviceState)
    {
        DeviceState nowDeviceState = m_Devices[deviceName];
        Debug.Log($"Device-[{deviceName}] From [{nowDeviceState}] => [{newDeviceState}]");
        m_Devices.TryUpdate(deviceName, newDeviceState, nowDeviceState);
        // Debug.Log($"Current Device State is: [{m_Devices[deviceName]}]");
    }
    public bool IsDeviceReady()
    {
        bool flag = true;
        foreach (var item in m_Devices)
        {
            Debug.Log($"[{item.Key}] == [{item.Value}]");
            if (!Enum.Equals(item.Value, DeviceState.SERVING))
            {
                Debug.Log($"{item.Key} has not ready");
                flag = false;
            }
        }
        return flag;
    }
    public void UpdateAppState(ApplicationState newApplicationState)
    {
        Debug.Log($"Application from [{CurAppState}] => [{newApplicationState}]");
        CurAppState = newApplicationState;
    }
    private void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        try
        {
            //Debug.Log($"handlePosition: {handlePosition.x}, {handlePosition.y}");
            switch (CurAppState)
            {
                case ApplicationState.UNTRIALED:
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.Log($"{e.Message}\n{e.StackTrace}");
        }
    }
    public void UpdateStickPosition(float x, float y)
    {
        m_HandPosition.x = x;
        m_HandPosition.y = y;
    }
    public void StartTrial()
    {
        UpdateAppState(ApplicationState.UNTRIALED);
        LoadDevices();
    }
    /// <summary>
    /// 载入设备
    /// </summary>
    private void LoadDevices()
    {
        foreach (var item in m_Devices)
        {
            Debug.Log($"Load [{item.Key}] = [{item.Value}] -> [{DeviceState.UNCHECKED}]");
            m_Devices[item.Key] = DeviceState.UNCHECKED;
        }
    }
    public void QuitTrial()
    {
        UpdateAppState(ApplicationState.NONE);
    }
    private void OnDestroy()
    {
        CurAppState = ApplicationState.NONE;
    }
}
