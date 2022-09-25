using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Microsoft.MixedReality.Toolkit.Rendering;
using Microsoft.MixedReality.Toolkit.UI;

public class ItemCardStudent : MonoBehaviour
{
    #region Public Fields

    public ClassroomUser studentUser;

    public TextMeshPro nameText;
    public Renderer studentColorMesh;
    public GameObject studentButtons;
    public GameObject muteIcon;
    public MeshRenderer muteIcon2;

    public Interactable clickOnStudentButton;

    public string userName = "";
    public string userRegion = "";
    public string userEmail = "";
    public Color userColor = Color.red;

    #endregion

    #region Monobehaviour Callbacks

    private void Start()
    {
        studentButtons.SetActive(false);
    }

    #endregion

    #region Public Methods

    public void ToggleStudentButtons()
    {
        ScrollParentStudentSpawner.Instance.PlacePressableButtonsAtLocation(studentButtons.transform, this);
    }

    public void DeselctToggleButton()
    {
        if (clickOnStudentButton != null)
        {
            clickOnStudentButton.IsToggled = false;
        }
    }

    public void ToggleMuteIcon(bool mute)
    {
        studentUser.studentMuted = mute;
        muteIcon.SetActive(studentUser.studentMuted);
        muteIcon2.enabled = studentUser.studentMuted;
    }

    public void UpdateNameText(string _text)
    {
        if(nameText == null)
        {
            Debug.LogError("Missing STUDENT CARD TEXTMESH NAME");
            return;
        }

        nameText.text = _text;
        userName = _text;
    }

    public void UpdateRegionText(string _text)
    {
        userRegion = _text;
    }

    public void UpdateEmailText(string _text)
    {
        userEmail = _text;
    }

    public void UpdateStudentColor(Color _color)
    {
        if (studentColorMesh == null)
        {
            Debug.LogError("Missing STUDENT CARD MESH");
            return;
        }

        var materialInstance = studentColorMesh.GetComponent<MaterialInstance>();
        if(materialInstance != null)
        {
            var material = materialInstance.AcquireMaterial();
            material.color = _color;
        }
        else
        {
            studentColorMesh.material.color = _color;
        }

        userColor = _color;
        nameText.color = userColor;
    }

    #endregion

}
