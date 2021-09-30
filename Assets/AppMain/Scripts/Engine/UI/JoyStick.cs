using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyStick : MonoBehaviour
{
    public XCore xCore;
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
                case ApplicationState.TRIALING:
                    this.gameObject.GetComponent<Renderer>().enabled = true;
                    this.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                    UpdateHandPosition();
                    break;
                default:
                    this.gameObject.GetComponent<Renderer>().enabled = false;
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Occur Error: {e.Message}\n{e.StackTrace}");
        }
    }
    private void UpdateHandPosition()
    {
        var handPosition = xCore.HandPosition;
        var newPosition = Camera.main.ScreenToWorldPoint(handPosition);
        this.transform.position = new Vector3(newPosition.x, newPosition.y, this.transform.position.z);
    }
}
