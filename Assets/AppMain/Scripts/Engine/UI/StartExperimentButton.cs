using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class StartExperimentButton : MonoBehaviour
{
    [SerializeField]
    private UnityEvent m_OnClick = null;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"StartExperimentButton.Start!");
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            switch (XCore.CurAppState)
            {
                case ApplicationState.TRIALING:
                    this.gameObject.GetComponent<Renderer>().enabled = false;
                    break;
                default:
                    this.gameObject.GetComponent<Renderer>().enabled = true;
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Occur Error: {e.Message}\n{e.StackTrace}");
        }
    }
    void OnMouseDown()
    {
        m_OnClick.Invoke();
    }
}
