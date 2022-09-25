using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// Persists through all scenes
/// </summary>
public class ClassroomGameManager : MonoBehaviourPunCallbacks//, IPunObservable
{
    #region Public Fields

    public static ClassroomGameManager Instance;

    public int connectedStudents = 0;

    #endregion

    #region MonoBehaviour CallBacks

    private void Awake()
    {
        Instance = this;
        //if(ClassroomGameManager.Instance != null)
        //{
        //    Destroy(gameObject);
        //}
        //else
        //{
        //    Instance = this;
        //}
    }

    private void Start()
    {
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
    }

    #endregion

    #region Photon Callbacks

    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting

        //connectedStudents++;


        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

        }
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

        //connectedStudents--;


        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

        }
    }

    #endregion

    #region IPunObservable implementation


    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        if (stream.IsWriting)
    //        {
    //            // We own this player: send the others our data
    //            stream.SendNext(connectedStudents);
    //        }
    //        else
    //        {
    //            // Network player, receive data
    //            this.connectedStudents = (int)stream.ReceiveNext();
    //        }
    //    }
    //}


    #endregion

    #region RPC Calls

    [PunRPC]
    void UpdateConnectedStudents(int updatedConnectStudents)
    {
        connectedStudents = updatedConnectStudents;
    }

    #endregion

    #region Public Methods

    public void UpdateConnectedStudents()
    {
        photonView.RPC("UpdateConnectedStudents", RpcTarget.All, connectedStudents);
    }

    #endregion

}
