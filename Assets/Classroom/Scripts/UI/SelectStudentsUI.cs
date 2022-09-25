using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.UI;

public class SelectStudentsUI : MonoBehaviour
{

    #region Public Fields

    public static SelectStudentsUI Instance;
    public ScrollParentStudentSpawner studentScrollParent;

    #endregion

    #region Private Fields

    private Interactable toggle;

    #endregion

    #region Monobehaviour Callbacks

    private void Start()
    {
        Instance = this;
        toggle = GetComponent<Interactable>();

        if (studentScrollParent == null)
        {
            Debug.LogError("MISSING STUDENT SCROLL UI");
            return;
        }

        toggle.IsToggled = studentScrollParent.gameObject.activeSelf;
    }

    #endregion

    #region Public Methods

    public void ToggleStudentsScrollUI()
    {
        if (studentScrollParent == null)
        {
            Debug.LogError("MISSING STUDENT SCROLL UI");
            return;
        }

        if(toggle.IsToggled)
        {
            studentScrollParent.gameObject.SetActive(true);

            studentScrollParent.UpdateStudentScrollObject();
        }
        else
        {
            studentScrollParent.CloseStudentSpawnerMenu();
        }
    }

    public void DisableStudentsScrollUI()
    {
        if (studentScrollParent == null)
        {
            Debug.LogError("MISSING STUDENT SCROLL UI");
            return;
        }

        toggle.IsToggled = false;

        studentScrollParent.gameObject.SetActive(false);
    }

    #endregion

}
