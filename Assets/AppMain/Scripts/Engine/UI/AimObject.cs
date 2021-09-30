using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFTAI.X.Trial.Step;

public class AimObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<Renderer>().enabled = false;
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
                    case TrialState.FREE_TRIALING:
                    case TrialState.COUNTING_DOWN:
                        this.gameObject.GetComponent<Renderer>().enabled = true;
                        break;
                    case TrialState.END_POINT_DISPLAYING:
                        this.gameObject.GetComponent<Renderer>().enabled = true;
                        DisplayEndPoint();
                        break;
                    default:
                        this.gameObject.GetComponent<Renderer>().enabled = false;
                        break;
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Occur Error: {e.Message}\n{e.StackTrace}");
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log($"On: {collision.transform.name}");
        EventDisruptor.PublishTrialEvent(TrialEventType.End_Point_Arrived);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        //Debug.Log($"Out: {collision.transform.name}");
        EventDisruptor.PublishTrialEvent(TrialEventType.End_Point_Exited);
    }
    private void DisplayEndPoint()
    {
        float[] taskPointValue = ParameterUtil.endPointDic[TrialCore.TrialIndex];
        var m_EndSourcePoint = new Vector2(taskPointValue[0], taskPointValue[1]);
        var m_EndTargetPoint = MainUI.M2VecConvert2GameVec(m_EndSourcePoint);
        var worldPosition = Camera.main.ScreenToWorldPoint(m_EndTargetPoint);
        this.transform.position = new Vector3(worldPosition.x, worldPosition.y, this.transform.position.z);
        this.transform.SetAsFirstSibling();
    }
}
