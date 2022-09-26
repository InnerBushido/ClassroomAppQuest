using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Linq;

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
    public GridObjectCollection studentSpawnCollection;
    public Transform roverSpawnTransform;

    public List<GameObject> connectedStudentsList = new List<GameObject>();

    #endregion

    #region Private Fields

    private bool optimizePlayers = false;
    private int amountOfPlayersBeforeOptimization = 7;

    private Dictionary<int, float> activeRecorders = new Dictionary<int, float>();
    private Dictionary<int, float> orderedRecorders = new Dictionary<int, float>();

    #endregion

    #region MonoBehaviour CallBacks

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

        StartCoroutine(CheckVoiceOptimizationUpdate());
    }

    #endregion

    #region Coroutines

    IEnumerator CheckVoiceOptimizationUpdate()
    {
        yield return new WaitForSeconds(0.2f);
        if (optimizePlayers && PhotonNetwork.IsMasterClient)
        {
            CheckVoiceOptimization();
        }

        StartCoroutine(CheckVoiceOptimizationUpdate());
    }

    #endregion

    #region Private Methods

    private void CheckVoiceOptimization()
    {
        activeRecorders = new Dictionary<int, float>();

        // Find all active Recorders
        foreach (GameObject playerObject in connectedStudentsList)
        {
            ClassroomUser player = playerObject.GetComponent<ClassroomUser>();

            if (player != null)
            {
                activeRecorders.Add(player.photonView.ViewID, player.studentVoiceLevel);
            }
        }


        // Check MasterClient Recorder
        ClassroomUser localPlayer = ClassroomUser.LocalPlayerInstance.GetComponent<ClassroomUser>();
        if(localPlayer.isProfessor)
        {
            activeRecorders.Add(localPlayer.photonView.ViewID, localPlayer.studentVoiceLevel);
        }

        // If there are more than the Max Amount of Players
        if(activeRecorders.Count > amountOfPlayersBeforeOptimization)
        {
            orderedRecorders = activeRecorders.OrderByDescending(num => num.Value).ToDictionary(num => num.Key, num => num.Value);

            //foreach(var pair in orderedRecorders)
            //{
            //    Debug.Log("Ordered Pair: " + pair.Key + " ___ " + pair.Value);
            //}

            // Only activate the loudest Players
            int i = 0;
            foreach(var pair in orderedRecorders)
            {
                if (i < amountOfPlayersBeforeOptimization)
                {
                    photonView.RPC("PunRPC_MuteSpecificStudent", RpcTarget.All, false, pair.Key);
                }
                else
                {
                    photonView.RPC("PunRPC_MuteSpecificStudent", RpcTarget.All, true, pair.Key);
                }

                i++;
            }

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

                InstantiateUser(studentPrefab, studentSpawnCollection.NodeListReadOnly[connectedStudentsList.Count].Transform);
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
                        CheckPlayerOptimization();
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
        if(instructorUIPrefab != null)
        {
            var userUI = Instantiate(instructorUIPrefab);
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

    private void CheckPlayerOptimization()
    {
        if (PhotonNetwork.IsMasterClient)
        {

            if (connectedStudentsList.Count > amountOfPlayersBeforeOptimization)
            {
                optimizePlayers = true;
            }
            else
            {
                optimizePlayers = false;
            }

        }
    }

    #endregion

    #region Photon Callbacks

    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
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
    void AddToConnectedStudentsList(int newStudentView)
    {
        connectedStudentsList.Add(PhotonView.Find(newStudentView).gameObject);
        CheckPlayerOptimization();
    }

    [PunRPC]
    void RemoveFromConnectedStudentsList(int newStudentView)
    {
        connectedStudentsList.Remove(PhotonView.Find(newStudentView).gameObject);
        CheckPlayerOptimization();
    }

    [PunRPC]
    void InstantiateOnSpecificPlayer(string userID)
    {
        if(photonView.Owner.UserId == userID)
        {
            InstantiatePlayer();
        }
    }

    [PunRPC]
    private void PunRPC_MuteSpecificStudent(bool isMuted, int recorderViewID)
    {
        ClassroomUser user = ClassroomUser.LocalPlayerInstance.GetComponent<ClassroomUser>();
        Recorder recorder = user.GetComponent<PhotonVoiceView>().RecorderInUse;

        if (user.photonView.ViewID == recorderViewID)
        {
            //user.studentMuted = isMuted;

            if(isMuted)
            {
                recorder.TransmitEnabled = false;
            }
            else
            {
                // Only UnMute if student isn't forcibly muted
                if(!user.studentMuted)
                {
                    recorder.TransmitEnabled = true;
                }
            }
        }
    }

    #endregion

    #region Public Methods

    public void RemoveFromStudentList()
    {
        photonView.RPC("RemoveFromConnectedStudentsList", RpcTarget.AllBufferedViaServer, ClassroomUser.LocalPlayerInstance.GetPhotonView().ViewID);
    }

    public void LeaveRoom()
    {
        if (!ClassroomUser.LocalPlayerInstance.GetComponent<ClassroomUser>().isProfessor)
        {
            ClassroomManager.Instance.RemoveFromStudentList();

            ClassroomUser.LocalPlayerInstance.GetComponent<ClassroomUser>().UpdateStudentScrollList();
        }

        PhotonNetwork.LeaveRoom();
    }

    #endregion

}
