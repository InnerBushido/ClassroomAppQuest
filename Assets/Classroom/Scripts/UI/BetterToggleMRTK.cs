// ----------------------------------------------------------------------------
// <copyright file="BetterToggle.cs" company="Exit Games GmbH">
// Photon Voice Demo for PUN- Copyright (C) 2016 Exit Games GmbH
// </copyright>
// <summary>
// Unity UI extension class that should be used with Unity's built-in Toggle
// to broadcast value change in a better way.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.UI;

[RequireComponent(typeof(Interactable))]
[DisallowMultipleComponent]
public class BetterToggleMRTK : MonoBehaviour
{
    private Interactable toggle;

    public delegate void OnToggle(Interactable toggle);

    public static event OnToggle ToggleValueChanged;

    private void Start()
    {
        this.toggle = this.GetComponent<Interactable>();
        this.toggle.OnClick.AddListener(delegate { this.OnToggleValueChanged(); });
    }

    public void OnToggleValueChanged()
    {
        if (ToggleValueChanged != null)
        {
            ToggleValueChanged(this.toggle);
        }
    }
}