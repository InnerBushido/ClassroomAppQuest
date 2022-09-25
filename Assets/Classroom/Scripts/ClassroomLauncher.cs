using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

using Microsoft.MixedReality.Toolkit.Experimental.UI;

public class ClassroomLauncher : MonoBehaviourPunCallbacks
{
    #region Public Fields

    public static bool connectedAsProfessor = false;
    public static bool connectedInDebug = false;

    public static string userName = "";
    public static string userRegion = "";
    public static string userEmail = "";
    public static Color userColor = Color.red;

    public static List<string> userNamesDebug = new List<string>() { "Student 1", "Student 2", "Student 3", "Student 4", "Student 5", "Student 6", "Student 7", "Student 8" };
    public static List<string> userRegionsDebug = new List<string>() { "Region 1", "Region 2", "Region 3", "Region 4", "Region 5", "Region 6", "Region 7", "Region 8" };
    public static List<string> userEmailsDebug = new List<string>() {"Email 1", "Email 2", "Email 3", "Email 4", "Email 5", "Email 6", "Email 7", "Email 8"};
    public static List<Color> userColorsDebug = new List<Color>() { Color.white, Color.black, Color.gray, Color.red, Color.blue, Color.green, Color.cyan, Color.yellow };

    public KeyboardInput keyboard;

    #endregion

    #region Private Fields

    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    [SerializeField]
    private GameObject controlPanel;

    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    [SerializeField]
    private GameObject progressLabel;

    string gameVersion = "1";
    bool isConnecting;

    #endregion

    #region MonoBehaviour CallBacks

    private void Start()
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }

    void Awake()
    {
        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            ConnectAsProfessor();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ConnectAsStudent();
        }
    }

    #endregion

    #region Public Methods

    public void ConnectAsProfessor()
    {
        keyboard.SetUserMetaData();
        connectedAsProfessor = true;
        connectedInDebug = false;
        Connect();
    }

    public void ConnectAsStudent()
    {
        keyboard.SetUserMetaData();
        connectedAsProfessor = false;
        connectedInDebug = false;
        Connect();
    }

    public void ConnectAsDebugProfessor()
    {
        keyboard.SetUserMetaData();
        connectedAsProfessor = true;
        connectedInDebug = true;
        Connect();
    }

    #endregion

    #region Private Methods

    private void Connect()
    {
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);

        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
            isConnecting = PhotonNetwork.ConnectUsingSettings();

            // #Critical, we must first and foremost connect to Photon Online Server.
            //PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    #endregion

    #region MonoBehaviourPunCallbacks Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");

        // we don't want to do anything if we are not attempting to join a room.
        // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
        // we don't want to do anything.
        if (isConnecting)
        {
            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
            PhotonNetwork.JoinRandomRoom();
            isConnecting = false;
        }
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);

        isConnecting = false;

        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions());
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

        if (!connectedAsProfessor)
        {
            Debug.Log("CONNECTED AS A STUDENT");
        }

        // #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            // #Critical
            // Load the Room Level.
            PhotonNetwork.LoadLevel("Classroom");
        }
    }

    #endregion

}
