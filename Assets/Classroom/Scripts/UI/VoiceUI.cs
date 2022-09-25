using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.UI;
using Photon.Voice.Unity;
using Photon.Pun;

public class VoiceUI : MonoBehaviourPun
{
    public Recorder recorder;

    private float volumeBeforeMute;

    private void OnEnable()
    {
        BetterToggleMRTK.ToggleValueChanged += this.BetterToggle_ToggleValueChanged;
    }

    private void OnDisable()
    {
        BetterToggleMRTK.ToggleValueChanged -= this.BetterToggle_ToggleValueChanged;
    }

    private void BetterToggle_ToggleValueChanged(Interactable toggle)
    {
        ItemCardStudent student;
        switch (toggle.name)
        {
            case "MuteButton":
                photonView.RPC("PunRPC_MuteStudents", RpcTarget.OthersBuffered, toggle.IsToggled);
                break;
            case "TransmitButton":
                if (this.recorder)
                {
                    this.recorder.TransmitEnabled = toggle.IsToggled;
                }
                break;
            case "MuteStudentButton":
                student = toggle.gameObject.GetComponentInParent<ScrollParentStudentSpawner>().selectedStudent;
                if(student != null)
                {
                    student.ToggleMuteIcon(toggle.IsToggled);
                    photonView.RPC("PunRPC_MuteSpecificStudent", RpcTarget.OthersBuffered, toggle.IsToggled, student.studentUser.photonView.ViewID);
                }
                break;
            case "KickStudentButton":
                student = toggle.gameObject.GetComponentInParent<ScrollParentStudentSpawner>().selectedStudent;
                if (student != null)
                {
                    photonView.RPC("PunRPC_KickSpecificStudent", RpcTarget.OthersBuffered, student.studentUser.photonView.ViewID);
                }
                break;
            case "BanStudentButton":
                student = toggle.gameObject.GetComponentInParent<ScrollParentStudentSpawner>().selectedStudent;
                if (student != null)
                {
                    photonView.RPC("PunRPC_KickSpecificStudent", RpcTarget.OthersBuffered, student.studentUser.photonView.ViewID);
                }
                break;
        }
    }

    #region RPC Calls

    [PunRPC]
    private void PunRPC_MuteStudents(bool isMuted)
    {
        ClassroomUser user = ClassroomUser.LocalPlayerInstance.GetComponent<ClassroomUser>();
        if (!user.isProfessor)
        {
            user.studentMuted = isMuted;
            this.recorder.TransmitEnabled = !isMuted;
        }
    }

    [PunRPC]
    private void PunRPC_MuteSpecificStudent(bool isMuted, int userViewID)
    {
        ClassroomUser user = ClassroomUser.LocalPlayerInstance.GetComponent<ClassroomUser>();
        if (user.photonView.ViewID == userViewID)
        {
            user.studentMuted = isMuted;
            this.recorder.TransmitEnabled = !isMuted;
        }
    }

    [PunRPC]
    private void PunRPC_KickSpecificStudent(int userViewID)
    {
        ClassroomUser user = ClassroomUser.LocalPlayerInstance.GetComponent<ClassroomUser>();
        if (user.photonView.ViewID == userViewID)
        {
            ClassroomManager.Instance.LeaveRoom();
        }
    }

    #endregion

}
