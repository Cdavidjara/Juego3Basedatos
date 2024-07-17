using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class launch : MonoBehaviourPunCallbacks
{
    public PhotonView personPrefab;
    public Transform puntoReferencia;
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connection success");
        PhotonNetwork.JoinRandomOrCreateRoom();

    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate(personPrefab.name, puntoReferencia.position, puntoReferencia.rotation);
    }


}
