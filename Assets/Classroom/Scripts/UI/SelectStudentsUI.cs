using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.UI;

public class SelectStudentsUI : MonoBehaviour
{
    public static SelectStudentsUI Instance;

    //public GameObject studentScrollParent;
    public ScrollParentStudentSpawner studentScrollParent;
    //private bool enabled = false;

    private Interactable toggle;

    private void Start()
    {
        Instance = this;
        toggle = GetComponent<Interactable>();

        if (studentScrollParent == null)
        {
            Debug.LogError("MISSING STUDENT SCROLL UI");
            return;
        }

        //enabled = studentScrollParent.gameObject.activeSelf;
        toggle.IsToggled = studentScrollParent.gameObject.activeSelf;
    }

    public void ToggleStudentsScrollUI()
    {
        if (studentScrollParent == null)
        {
            Debug.LogError("MISSING STUDENT SCROLL UI");
            return;
        }

        //enabled = !enabled;

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

        //studentScrollParent.UpdateStudentScrollObject();
    }

}
