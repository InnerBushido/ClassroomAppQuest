using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Utilities;
using Photon.Pun;

public class ParabolaArch : MonoBehaviourPun
{
    #region Public Fields

    public ParabolaConstrainedLineDataProvider parabola;
    public MixedRealityLineRenderer lineRenderer;

    public Transform startPointTransform;
    public Transform endPointTransform;

    public ParticleSystem[] particles;

    #endregion

    #region Monobehaviour Callbacks

    private void Start()
    {
        if(photonView.IsMine && startPointTransform == null)
        {
            startPointTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            transform.position = startPointTransform.position;
            transform.rotation = Quaternion.identity;
        }

        MixedRealityPose newPose = parabola.EndPoint;
        newPose.Position = endPointTransform.position - transform.position;
        parabola.EndPoint = newPose;
    }

    #endregion

    #region Public Methods

    public void AssignLineColor(Color _color)
    {
        var colors = lineRenderer.LineColor.colorKeys;

        for(int i = 0; i < colors.Length; i++)
        {
            colors[i].color = _color;
        }

        lineRenderer.LineColor.colorKeys = colors;

        for(int i = 0; i < particles.Length; i++)
        {
            var colorLifetime = particles[i].colorOverLifetime;
            var gradient = colorLifetime.color;
            var colorKeys = gradient.gradient.colorKeys;

            if (particles[i].gameObject.name == "Afterburner")
            {
                colorKeys[3].color = _color;
                colorKeys[4].color = _color;
            }
            else if (particles[i].gameObject.name == "Glow")
            {
                colorKeys[1].color = _color;
                colorKeys[2].color = _color;
            }

            gradient.gradient.colorKeys = colorKeys;
            colorLifetime.color = gradient;
        }

    }

    #endregion

}
