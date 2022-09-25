using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class ClassroomUserUI : MonoBehaviour
{
    #region Public Fields

    public ScrollParentStudentSpawner studentScrollList;

    #endregion

    #region Public Methods

    public void RaiseHandToggled()
    {
        ClassroomUser.LocalPlayerInstance.GetComponent<ClassroomUser>().RaiseHandToggle();
    }

    public void LeaveRoom()
    {
        ClassroomManager.Instance.LeaveRoom();
    }

    public void UpdateStudentScrollList()
    {
        if(studentScrollList != null && !ClassroomLauncher.connectedInDebug)
        {
            Debug.Log("UPDATE STUDENT SCROLL LIST CALLED");
            studentScrollList.UpdateStudentScrollObject();
        }
    }

    #endregion

}
