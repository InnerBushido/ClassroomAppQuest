using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.UI;

public class SelectColorUI : MonoBehaviour
{
    #region Public Fields

    [SerializeField] private FollowMeToggle followMe = null;
    public GameObject colorWheel;

    #endregion

    #region Private Fields

    private bool enabled = false;

    #endregion

    #region Monobehaviour Callbacks

    private void Start()
    {
        if (colorWheel == null)
        {
            Debug.LogError("MISSING COLOR WHEEL");
            return;
        }

        enabled = colorWheel.activeSelf;
    }

    #endregion

    #region Public Methods

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

    #endregion

}
