using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WaitingRoomManager : MonoBehaviourPunCallbacks
{
    // R�f�rences UI
    public TMP_Text roomCodeText;              // Pour afficher le code du salon
    public Transform playerListContainer;      // Conteneur pour les cartes joueur
    public GameObject playerCardPrefab;        // Prefab pour les cartes joueur
    public Button startGameButton;             // Bouton pour d�marrer la partie
    public Button leaveRoomButton;             // Bouton pour quitter le salon
    public TMP_Text statusText;                // Pour afficher les messages de statut

    void Start()
    {
        // V�rifie si la salle est disponible
        if (PhotonNetwork.CurrentRoom != null)
        {
            roomCodeText.text = "Code du salon : " + PhotonNetwork.CurrentRoom.Name;
        }
        else
        {
            roomCodeText.text = "Code du salon non disponible.";
        }

        // Met � jour la liste des joueurs d�s le d�but
        UpdatePlayerList();

        // Initialisation du texte de statut
        statusText.text = PhotonNetwork.IsMasterClient
            ? "Vous �tes le chef du groupe. Attendez les autres joueurs ou d�marrez la partie."
            : "En attente que le chef de groupe d�marre la partie.";

        // Active ou d�sactive le bouton *D�marrer la partie* en fonction du r�le
        UpdateHostControls();

        // Ajouter une action au bouton *Quitter le salon*
        leaveRoomButton.onClick.AddListener(LeaveRoom);
    }

    // Met � jour la liste des participants dans la salle
    void UpdatePlayerList()
    {
        // Supprime tous les anciens enfants avant d'ajouter les nouveaux
        foreach (Transform child in playerListContainer)
        {
            Destroy(child.gameObject);
        }

        // Ajoute une carte pour chaque joueur
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            // Instancie une nouvelle carte joueur
            GameObject playerCard = Instantiate(playerCardPrefab, playerListContainer);

            // Recherche r�cursive pour trouver "PlayerName"
            TMP_Text playerNameText = FindInChildren<TMP_Text>(playerCard, "PlayerName");
            if (playerNameText != null)
            {
                playerNameText.text = player.NickName; // Met � jour le pseudo
            }
            else
            {
                Debug.LogError($"Erreur : 'PlayerName' introuvable dans le prefab {playerCardPrefab.name}.");
            }
        }
    }

    // M�thode pour trouver un composant par nom dans toute la hi�rarchie
    T FindInChildren<T>(GameObject parent, string childName) where T : Component
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == childName)
            {
                return child.GetComponent<T>();
            }
        }
        return null;
    }

    // Met � jour les contr�les sp�cifiques � l'h�te
    void UpdateHostControls()
    {
        // Affiche le bouton uniquement pour le chef
        if (startGameButton != null)
        {
            startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        }

        // Si le joueur est l'h�te, on ajoute un listener au bouton
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.onClick.RemoveAllListeners();
            startGameButton.onClick.AddListener(StartGame);
        }
        else
        {
            startGameButton.onClick.RemoveAllListeners();
        }
    }

    // Fonction pour quitter le salon
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        statusText.text = "Vous avez quitt� le salon.";
    }

    // Fonction pour d�marrer la partie (r�serv�e � l'h�te)
    public void StartGame()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            statusText.text = "Seul l'h�te peut d�marrer la partie.";
            return;
        }

        // Appel d'un RPC pour d�marrer la partie sur tous les clients
        photonView.RPC("LoadGameScene", RpcTarget.All);
    }

    [PunRPC]
    public void LoadGameScene()
    {
        PhotonNetwork.LoadLevel("Mode");
    }

    // Callback lorsque l'h�te change
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        statusText.text = $"{newMasterClient.NickName} est maintenant le chef de groupe.";
        UpdateHostControls();
    }

    // Callback lorsqu'un joueur rejoint la salle
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        UpdatePlayerList();
        statusText.text = $"{newPlayer.NickName} a rejoint le salon.";
    }

    // Callback lorsqu'un joueur quitte la salle
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        UpdatePlayerList();
        statusText.text = $"{otherPlayer.NickName} a quitt� le salon.";

        if (PhotonNetwork.CurrentRoom.PlayerCount == 0)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    // Callback lorsque le joueur quitte le salon avec succ�s
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Callback pour g�rer les d�connexions ou probl�mes techniques
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        statusText.text = "D�connect� de Photon. Cause : " + cause.ToString();
        SceneManager.LoadScene("MainMenu");
    }
}

