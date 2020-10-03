using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluctuationIndicatorDrawer : MonoBehaviour
{
    private Color _trasparentColor = new Color(1, 1, 1, 0);

    private void Update()
    {
        try
        {
            gameObject.GetComponent<SpriteRenderer>().color = FindObjectOfType<Spider>().GetCurrentThread().CheckLastPointFluctuations() ? Color.white : _trasparentColor;
        }
        catch (NullReferenceException e){}
    }
}
