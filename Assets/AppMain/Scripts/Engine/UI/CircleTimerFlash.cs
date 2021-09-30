using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleTimerFlash : MonoBehaviour
{
    public GameObject m_AimObject;
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<Image>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (XCore.CurAppState == ApplicationState.TRIALING)
            {
                TrialState CurTrialState = (TrialState)Enum.Parse(typeof(TrialState), XCoreParameter.cdString[CDStringKeys.CurTrialState], true);
                switch (CurTrialState)
                {
                    case TrialState.COUNTING_DOWN:
                        this.gameObject.GetComponent<Image>().enabled = true;
                        FlashShow();
                        CountDown();
                        break;
                    default:
                        this.gameObject.GetComponent<Image>().enabled = false;
                        break;
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Occur Error: {e.Message}\n{e.StackTrace}");
        }
    }
    public void FlashShow()
    {
        this.transform.position = m_AimObject.gameObject.transform.position;
    }
    private void CountDown()
    {
        var circleTime = XCoreParameter.GetDoubleValue(CDDoubleKeys.CircleTime);
        var circleTimer = XCoreParameter.GetDoubleValue(CDDoubleKeys.CircleTimer) - Time.deltaTime;
        XCoreParameter.UpdateDoubleValue(CDDoubleKeys.CircleTimer, circleTimer);
        if (circleTimer > 0)
        {
            this.gameObject.GetComponent<Image>().fillAmount = Mathf.Lerp(0, 1, (float)(circleTimer / circleTime));
        }
        else
        {
            EventDisruptor.PublishTrialEvent(TrialEventType.End_Point_Hovered);
        }
    }
}
