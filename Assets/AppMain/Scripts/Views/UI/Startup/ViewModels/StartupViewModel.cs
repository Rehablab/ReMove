using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartupViewModel : MonoBehaviour
{
    public Button startTrialButton;
    public Button backButton;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            switch (XCore.CurAppState)
            {
                case ApplicationState.NONE:
                    startTrialButton.enabled = true;
                    startTrialButton.interactable = true;
                    backButton.enabled = true;
                    backButton.interactable = true;
                    break;
                default:
                    startTrialButton.enabled = false;
                    startTrialButton.interactable = false;
                    backButton.enabled = false;
                    backButton.interactable = false;
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Occur Error: {e.Message}\n{e.StackTrace}");
        }
    }
}
