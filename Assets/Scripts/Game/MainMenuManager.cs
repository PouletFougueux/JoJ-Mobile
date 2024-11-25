using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class MainMenuManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        Debug.Log("Connecting to Photon...");
        PhotonNetwork.ConnectUsingSettings(); // Connecte au serveur Photon
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server.");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected from Photon: {cause}");
    }

    public void QuitGame()
    {
        // Quitte l'application (fonctionne uniquement dans une build, pas dans l'éditeur)
        Application.Quit();
        Debug.Log("Quitter le jeu");
    }
}
