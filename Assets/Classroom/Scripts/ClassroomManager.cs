using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

public class ClassroomManager : MonoBehaviourPunCallbacks
{
    #region Public Fields

    public static ClassroomManager Instance;

    public GameObject professorPrefab;
    public GameObject studentPrefab;
    public GameObject voicePrefab;

    public GameObject roverPrefab;
    public GameObject instructorUIPrefab;
    public GameObject studentUIPrefab;

    public Transform professorSpawnTransform;
    //public Transform studentSpawnTransform;
    public GridObjectCollection studentSpawnCollection;
    public Transform roverSpawnTransform;

    public int connectedStudents = 0;

    public List<GameObject> connectedStudentsList = new List<GameObject>();

    #endregion

    private void Start()
    {
        Instance = this;

        if(PhotonVoiceNetwork.Instance == null)
        {
            Instantiate(voicePrefab);
        }

        if(PhotonNetwork.IsMasterClient)
        {
            InstantiatePlayer();
        }
    }

    private void InstantiatePlayer()
    {
        if (ClassroomLauncher.connectedAsProfessor)
        {
            InstantiateUser(professorPrefab, professorSpawnTransform, true);
        }
        else
        {
            //InstantiateUser(studentPrefab, studentSpawnTransform);
            InstantiateUser(studentPrefab, studentSpawnCollection.NodeListReadOnly[connectedStudentsList.Count].Transform);
        }

        if (ClassroomLauncher.connectedInDebug)
        {
            for (int i = 0; i < 8; i++)
            {
                ClassroomLauncher.userName = ClassroomLauncher.userNamesDebug[i];
                ClassroomLauncher.userRegion = ClassroomLauncher.userRegionsDebug[i];
                ClassroomLauncher.userEmail = ClassroomLauncher.userEmailsDebug[i];
                ClassroomLauncher.userColor = ClassroomLauncher.userColorsDebug[i];
                //InstantiateUser(studentPrefab, studentSpawnTransform);
                InstantiateUser(studentPrefab, studentSpawnCollection.NodeListReadOnly[connectedStudents].Transform);
            }
        }
    }

    private void InstantiateUser(GameObject userPrefab, Transform spawnTransform, bool isProfessor = false)
    {
        if (userPrefab == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> userPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        }
        else
        {
            if (ClassroomUser.LocalPlayerInstance == null || ClassroomLauncher.connectedInDebug)
            {
                Debug.LogFormat("We are Instantiating " + userPrefab.name + " from {0}", SceneManagerHelper.ActiveSceneName);

                var user = PhotonNetwork.Instantiate(userPrefab.name, spawnTransform.position, spawnTransform.rotation);

                user.GetComponent<ClassroomUser>().isProfessor = isProfessor;
                user.GetComponent<ClassroomUser>().Initialize();

                if(isProfessor)
                {
                    InstantiateProfessorPrefabs();
                }
                else
                {
                    if(!ClassroomLauncher.connectedInDebug)
                    {
                        InstantiateStudentPrefabs();
                        photonView.RPC("AddToConnectedStudentsList", RpcTarget.AllBufferedViaServer, user.GetPhotonView().ViewID);
                        user.GetComponent<ClassroomUser>().UpdateStudentScrollList();
                    }
                    else
                    {
                        connectedStudentsList.Add(user);
                    }
                }
            }
            else
            {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
        }
    }

    private void InstantiateProfessorPrefabs()
    {
        // Instantiate Rover over the Network
        //if (PhotonNetwork.IsMasterClient && ClassroomUser.RoverInstance == null)
        if(ClassroomUser.RoverInstance == null)
        {
            var rover = PhotonNetwork.Instantiate(roverPrefab.name, roverSpawnTransform.position, roverSpawnTransform.rotation);
            ClassroomUser.RoverInstance = rover;

            var roverAssembly = rover.transform.Find("RoverAssembly").gameObject;
            if(roverAssembly != null)
            {
                Microsoft.MixedReality.Toolkit.UI.ObjectManipulator objManip = roverAssembly.GetComponent<Microsoft.MixedReality.Toolkit.UI.ObjectManipulator>();
                NearInteractionGrabbable nearInt = roverAssembly.GetComponent<NearInteractionGrabbable>();

                if (objManip != null)
                {
                    objManip.enabled = true;
                }

                if(nearInt != null)
                {
                    nearInt.enabled = true;
                }
            }
        }

        //Instantiate Professor User UI
        var userUI = Instantiate(instructorUIPrefab);
        if(userUI != null)
        {
            var classroomUserUI = userUI.GetComponent<ClassroomUserUI>();

            if(classroomUserUI != null)
            {
                ClassroomUser.userUI = classroomUserUI;
            }
        }
    }

    private void InstantiateStudentPrefabs()
    {
        Instantiate(studentUIPrefab);
    }

    #region Photon Callbacks

    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        if(ClassroomUser.LocalPlayerInstance != null)
        {
            ClassroomUser user = ClassroomUser.LocalPlayerInstance.GetComponent<ClassroomUser>();
            if (user != null && !user.isProfessor)
            {
                //ClassroomGameManager.Instance.connectedStudents--;
                //ClassroomGameManager.Instance.UpdateConnectedStudents();

                //if (ClassroomUser.userUI != null)
                //{
                //    ClassroomUser.userUI.UpdateStudentScrollList();
                //}                
            }
        }

        SceneManager.LoadScene(0);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", newPlayer.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("InstantiateOnSpecificPlayer", RpcTarget.Others, newPlayer.UserId);
        }
    }

    #endregion

    #region RPC Calls

    [PunRPC]
    void UpdateConnectedStudents(int updatedConnectStudents)
    {
        connectedStudents = updatedConnectStudents;
    }

    [PunRPC]
    void AddToConnectedStudentsList(int newStudentView)
    {
        connectedStudentsList.Add(PhotonView.Find(newStudentView).gameObject);
    }

    [PunRPC]
    void RemoveFromConnectedStudentsList(int newStudentView)
    {
        connectedStudentsList.Remove(PhotonView.Find(newStudentView).gameObject);
    }

    [PunRPC]
    void InstantiateOnSpecificPlayer(string userID)
    {
        if(photonView.Owner.UserId == userID)
        {
            InstantiatePlayer();
        }
    }

    #endregion

    #region Public Methods

    public void UpdateConnectedStudents()
    {
        photonView.RPC("UpdateConnectedStudents", RpcTarget.AllBufferedViaServer, connectedStudents);
    }

    public void RemoveFromStudentList()
    {
        photonView.RPC("RemoveFromConnectedStudentsList", RpcTarget.AllBufferedViaServer, ClassroomUser.LocalPlayerInstance.GetPhotonView().ViewID);
    }

    public void LeaveRoom()
    {
        if (!ClassroomUser.LocalPlayerInstance.GetComponent<ClassroomUser>().isProfessor)
        {
            ClassroomManager.Instance.connectedStudents--;
            ClassroomManager.Instance.UpdateConnectedStudents();

            ClassroomManager.Instance.RemoveFromStudentList();

            //UpdateStudentScrollList();
            ClassroomUser.LocalPlayerInstance.GetComponent<ClassroomUser>().UpdateStudentScrollList();
        }

        PhotonNetwork.LeaveRoom();
    }

    #endregion

}
