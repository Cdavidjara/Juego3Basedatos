using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;


public class Laucher : MonoBehaviourPunCallbacks
{


    public PhotonView personajePrefab;
    public Transform PuntoReferencia;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Coneccion satisfactoria..!!");
        PhotonNetwork.JoinRandomOrCreateRoom();

    }

    public override void OnJoinedRoom()
    {
       PhotonNetwork.Instantiate(personajePrefab.name, PuntoReferencia.position, PuntoReferencia.rotation);
        
    }
}
