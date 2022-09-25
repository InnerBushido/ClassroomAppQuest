using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class SamuraiAnimationManager : MonoBehaviourPunCallbacks
{

    private Animator animator;

    //private int currentAnim = 0;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (!animator)
        {
            Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
        }

        StartCoroutine(UpdatePlaybackOfAnim());
    }

    private void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if(Input.GetKeyDown(KeyCode.R))
        {
            animator.SetTrigger("RestartAnim");
        }
    }

    IEnumerator UpdatePlaybackOfAnim()
    {
        yield return new WaitForSeconds(1);

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (photonView.IsMine)
        {
            photonView.RPC("PunRPC_SendAnimationTime", RpcTarget.Others, stateInfo.normalizedTime, stateInfo.fullPathHash);
        }

        StartCoroutine(UpdatePlaybackOfAnim());
    }

    #region Photon Callbacks

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (photonView.IsMine)
        {
            photonView.RPC("PunRPC_SendAnimationTime", newPlayer, stateInfo.normalizedTime, stateInfo.fullPathHash);
        }
    }

    #endregion

    #region RPC Calls

    [PunRPC]
    private void PunRPC_SendAnimationTime(float animTime, int hashAnim)
    {
        //animator.playbackTime = animTime;
        animator.Play(hashAnim, 0, animTime);
    }

    #endregion

}
