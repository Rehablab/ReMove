using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    private Rect m_PanelRect = default(Rect);
    public static Rect PanelRect;
    public static Rect PanelBoundaryRect;
    private static Vector2 OriginPoint
    {
        get
        {
            return m_OriginPoint;
        }
    }
    private static Vector2 m_OriginPoint = Vector2.zero;
    public XCore xCore;
    [SerializeField]
    public BoxCollider m_M2Boundary;

    void Start()
    {
        var rectTransform = this.transform.GetComponent<RectTransform>().rect;
        var localScreenPosition = Camera.main.WorldToScreenPoint(this.transform.position);
        m_OriginPoint = new Vector2(localScreenPosition.x, localScreenPosition.y) + rectTransform.min;
        Debug.Log($"MainUI.OriginPoint: {OriginPoint.ToString("f6")}");


        PanelRect = new Rect(OriginPoint.x, OriginPoint.y, rectTransform.size.x, rectTransform.size.y);
        Debug.Log($"MainUI.PanelRect: {PanelRect.ToString("f6")}");
        Debug.Log($"MainUI.PanelRect.min: {PanelRect.min.ToString("f6")}");
        Debug.Log($"MainUI.PanelRect.max: {PanelRect.max.ToString("f6")}");
        Debug.Log($"MainUI.PanelRect.size: {PanelRect.size.ToString("f6")}");
        PanelBoundaryRect = new Rect(0, 0, 0.52f, 0.36f);
        Debug.Log($"MainUI.PanelRect.m_M2Boundary.bounds.size: {m_M2Boundary.bounds.size.ToString("f6")}");
        Debug.Log($"PanelBoundaryRect: {PanelBoundaryRect.ToString("f6")}");
    }
    /// <summary>
    /// 开始实验。
    /// </summary>
    public void StartExperiment()
    {
        xCore.StartTrial();
    }
    // Update is called once per frame
    void Update()
    {
        try
        {
            switch (XCore.CurAppState)
            {
                case ApplicationState.NONE:
                    break;
                case ApplicationState.TRIALING:
                    UpdateTrialState();
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"{e.Message}\n{e.StackTrace}");
        }
    }
    private void UpdateTrialState()
    {
        TrialState CurTrialState = (TrialState)Enum.Parse(typeof(TrialState), XCoreParameter.cdString[CDStringKeys.CurTrialState], true);
    }
    /// <summary>
    /// M2PRO坐标转换成游戏坐标。  （M2PRO坐标单位: m）
    /// </summary>
    /// <param name="m2Vector"></param>
    /// <returns></returns>
    public static Vector2 M2VecConvert2GameVec(Vector2 m2Vector)
    {
        // 计算相对比例后，乘以实际宽度，再加上偏移量
        return (m2Vector * MainUI.PanelRect.size / MainUI.PanelBoundaryRect.size) + MainUI.PanelRect.min;
    }
    /// <summary>
    /// 游戏坐标转换成控制台的相对坐标。
    /// </summary>
    /// <param name="gameVector"></param>
    /// <returns></returns>
    public static Vector2 GameVecConvert2RelativeVec(Vector2 gameVector)
    {
        return (gameVector - MainUI.PanelRect.min) / MainUI.PanelRect.size;
    }
}
