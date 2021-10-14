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
            if (showStatusFlag)
            {
                titleText.gameObject.SetActive(true);
                trialStatusText.gameObject.SetActive(true);
                DeviceNameText.gameObject.SetActive(true);
            }
            else
            {
                titleText.gameObject.SetActive(false);
                trialStatusText.gameObject.SetActive(false);
                DeviceNameText.gameObject.SetActive(false);
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Occur Error: {e.Message}\n{e.StackTrace}");
        }
    }
}
