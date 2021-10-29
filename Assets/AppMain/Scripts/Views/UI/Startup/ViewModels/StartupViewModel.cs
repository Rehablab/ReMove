using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartupViewModel : MonoBehaviour
{
    public Button startTrialButton;
    public Button backButton;
    public Text titleText;
    public Text trialStatusText;
    public Text DeviceNameText;
    public bool showStatusFlag = false;
    // Start is called before the first frame update
    void Start()
    {
        showStatusFlag = true;
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (showStatusFlag)
                {
                    showStatusFlag = false;
                }
                else
                {
                    showStatusFlag = true;
                }
            }

            startTrialButton.gameObject.SetActive(showStatusFlag);
            backButton.gameObject.SetActive(showStatusFlag);
            titleText.gameObject.SetActive(showStatusFlag);
            trialStatusText.gameObject.SetActive(showStatusFlag);
            DeviceNameText.gameObject.SetActive(showStatusFlag);
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

                    TrialState CurTrialState = (TrialState)Enum.Parse(typeof(TrialState), XCoreParameter.cdString[CDStringKeys.CurTrialState], true);
                    trialStatusText.text = CurTrialState.ToString();
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Occur Error: {e.Message}\n{e.StackTrace}");
        }
    }
}
