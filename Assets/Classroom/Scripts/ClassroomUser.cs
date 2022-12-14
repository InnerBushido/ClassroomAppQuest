using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using TMPro;

public class ClassroomUser : MonoBehaviourPunCallbacks
{
    #region Public Fields

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    public static GameObject RoverInstance;
    public static ClassroomUserUI userUI;
    public static ParabolaArch userSoul;

    public TextMeshPro userNameText;
    public GameObject raisedHand;
    public GameObject userSoulPrefab;
    public ParabolaArch thisUserSoul;

    public bool isProfessor = false;
    public string userName = "";
    public string userRegion = "";
    public string userEmail = "";
    public Color userColor = Color.red;

    public List<Renderer> userMesh = new List<Renderer>();

    public Transform startOfSoul;
    public bool studentMuted = false;

    public float studentVoiceLevel = 0;

    #endregion

    #region MonoBehaviour CallBacks

    private void Awake()
    {
        if(userMesh.Count <= 0)
        {
            userMesh[0] = GetComponent<Renderer>();
        }
    }

    #endregion

    #region Coroutines

    IEnumerator SendVoiceLevel()
    {
        yield return new WaitForSeconds(0.2f);
        photonView.RPC("PunRPC_SendVoiceLevel", RpcTarget.MasterClient, GetComponent<PhotonVoiceView>().RecorderInUse.LevelMeter.CurrentPeakAmp);
        StartCoroutine(SendVoiceLevel());
    }

    #endregion

    #region Public Methods

    public void Initialize()
    {
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine)
        {
            if (LocalPlayerInstance == null)
            {
                LocalPlayerInstance = this.gameObject;
            }

            userName = ClassroomLauncher.userName;
            userRegion = ClassroomLauncher.userRegion;
            userEmail = ClassroomLauncher.userEmail;
            userColor = ClassroomLauncher.userColor;

            if (ClassroomUser.userSoul == null)
            {
                //Instantiate Professor Soul
                var soul = PhotonNetwork.Instantiate(userSoulPrefab.name, Camera.main.transform.position, Quaternion.identity);
                var parabola = soul.GetComponent<ParabolaArch>();
                if (parabola != null)
                {
                    ClassroomUser.userSoul = parabola;

                    parabola.startPointTransform = Camera.main.transform;
                    photonView.RPC("PunRPC_AddSoul", RpcTarget.AllBuffered, soul.GetPhotonView().ViewID);
                }
            }

            Color toSync = ClassroomLauncher.userColor;
            photonView.RPC("PunRPC_SetUserColor", RpcTarget.AllBuffered, new Vector3(toSync.r, toSync.g, toSync.b));
            photonView.RPC("PunRPC_SetUserName", RpcTarget.OthersBuffered, userName);
            photonView.RPC("PunRPC_SetUserRegion", RpcTarget.OthersBuffered, userRegion);
            photonView.RPC("PunRPC_SetUserEmail", RpcTarget.OthersBuffered, userEmail);

            if (isProfessor)
            {
                photonView.RPC("PunRPC_SetNickName", RpcTarget.AllBuffered, "Professor " + PhotonNetwork.NickName);
            }
            else if (ClassroomLauncher.connectedInDebug)
            {
                gameObject.name = userName;
                userNameText.text = userName;
            }
            else
            {
                photonView.RPC("PunRPC_SetNickName", RpcTarget.AllBuffered, PhotonNetwork.NickName);
            }

            StartCoroutine(SendVoiceLevel());
        }
    }

    public void UpdateStudentScrollList()
    {
        photonView.RPC("PunRPC_UpdateScrollList", RpcTarget.AllViaServer);
    }

    public void RaiseHandToggle()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("PunRPC_RaiseHand", RpcTarget.AllBuffered, !raisedHand.activeSelf);
        }
    }

    #endregion

    #region RPC Calls

    [PunRPC]
    private void PunRPC_SendVoiceLevel(float voiceLevel)
    {
        studentVoiceLevel = voiceLevel;
    }

    [PunRPC]
    private void PunRPC_SetNickName(string nName)
    {
        gameObject.name = nName;
        userNameText.text = nName;
    }

    [PunRPC]
    private void PunRPC_SetUserName(string _Name)
    {
        userName = _Name;
    }

    [PunRPC]
    private void PunRPC_SetUserRegion(string _Region)
    {
        userRegion = _Region;
    }

    [PunRPC]
    private void PunRPC_SetUserEmail(string _Email)
    {
        userEmail = _Email;
    }

    [PunRPC]
    private void PunRPC_SetUserColor(Vector3 _color)
    {
        Color color = new Color(_color.x, _color.y, _color.z);

        userColor = color;

        if (userMesh != null && userMesh.Count > 0)
        {
            foreach(Renderer r in userMesh)
            {
                r.material.color = userColor;
            }
        }

        userNameText.color = userColor;

        if(thisUserSoul != null)
        {
            thisUserSoul.AssignLineColor(userColor);
        }
    }

    [PunRPC]
    private void PunRPC_RaiseHand(bool raiseHand)
    {
        raisedHand.SetActive(raiseHand);
    }

    [PunRPC]
    private void PunRPC_UpdateScrollList()
    {
        if (userUI != null)
        {
            userUI.UpdateStudentScrollList();
        }
    }

    [PunRPC]
    void PunRPC_AddSoul(int addSoulView)
    {
        var soul = PhotonView.Find(addSoulView).gameObject;

        if(soul != null)
        {
            var parabola = soul.GetComponent<ParabolaArch>();

            if (parabola != null)
            {
                thisUserSoul = parabola;

                parabola.endPointTransform = startOfSoul;
            }
            else
            {
                Debug.LogError("PLAYER HAS NO PARABOLA SOUL");
            }
        }
        else
        {
            Debug.LogError("PLAYER HAS NO SOUL");
        }
    }

    #endregion

}
