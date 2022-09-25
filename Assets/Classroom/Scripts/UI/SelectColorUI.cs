using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.UI;

public class SelectColorUI : MonoBehaviour
{
    [SerializeField] private FollowMeToggle followMe = null;
    public GameObject colorWheel;
    private bool enabled = false;

    private void Start()
    {
        if (colorWheel == null)
        {
            Debug.LogError("MISSING COLOR WHEEL");
            return;
        }

        enabled = colorWheel.activeSelf;
    }

    public void ToggleColorWheel()
    {
        if(colorWheel == null)
        {
            Debug.LogError("MISSING COLOR WHEEL");
            return;
        }

        enabled = !enabled;

        colorWheel.SetActive(enabled);

        followMe.SetFollowMeBehavior(!enabled);
    }

}
