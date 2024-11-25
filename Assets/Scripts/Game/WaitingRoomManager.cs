using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WaitingRoomManager : MonoBehaviourPunCallbacks
{
    // Références UI
    public TMP_Text roomCodeText;              // Pour afficher le code du salon
    public Transform playerListContainer;      // Conteneur pour les cartes joueur
    public GameObject playerCardPrefab;        // Prefab pour les cartes joueur
    public Button startGameButton;             // Bouton pour démarrer la partie
    public Button leaveRoomButton;             // Bouton pour quitter le salon
    public TMP_Text statusText;                // Pour afficher les messages de statut

    void Start()
    {
        // Vérifie si la salle est disponible
        if (PhotonNetwork.CurrentRoom != null)
        {
            roomCodeText.text = "Code du salon : " + PhotonNetwork.CurrentRoom.Name;
        }
        else
        {
            roomCodeText.text = "Code du salon non disponible.";
        }

        // Met à jour la liste des joueurs dès le début
        UpdatePlayerList();

        // Initialisation du texte de statut
        statusText.text = PhotonNetwork.IsMasterClient
            ? "Vous êtes le chef du groupe. Attendez les autres joueurs ou démarrez la partie."
            : "En attente que le chef de groupe démarre la partie.";

        // Active ou désactive le bouton *Démarrer la partie* en fonction du rôle
        UpdateHostControls();

        // Ajouter une action au bouton *Quitter le salon*
        leaveRoomButton.onClick.AddListener(LeaveRoom);
    }

    // Met à jour la liste des participants dans la salle
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

            // Recherche récursive pour trouver "PlayerName"
            TMP_Text playerNameText = FindInChildren<TMP_Text>(playerCard, "PlayerName");
            if (playerNameText != null)
            {
                playerNameText.text = player.NickName; // Met à jour le pseudo
            }
            else
            {
                Debug.LogError($"Erreur : 'PlayerName' introuvable dans le prefab {playerCardPrefab.name}.");
            }
        }
    }

    // Méthode pour trouver un composant par nom dans toute la hiérarchie
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

    // Met à jour les contrôles spécifiques à l'hôte
    void UpdateHostControls()
    {
        // Affiche le bouton uniquement pour le chef
        if (startGameButton != null)
        {
            startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        }

        // Si le joueur est l'hôte, on ajoute un listener au bouton
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
        statusText.text = "Vous avez quitté le salon.";
    }

    // Fonction pour démarrer la partie (réservée à l'hôte)
    public void StartGame()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            statusText.text = "Seul l'hôte peut démarrer la partie.";
            return;
        }

        // Appel d'un RPC pour démarrer la partie sur tous les clients
        photonView.RPC("LoadGameScene", RpcTarget.All);
    }

    [PunRPC]
    public void LoadGameScene()
    {
        PhotonNetwork.LoadLevel("Mode");
    }

    // Callback lorsque l'hôte change
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
        statusText.text = $"{otherPlayer.NickName} a quitté le salon.";

        if (PhotonNetwork.CurrentRoom.PlayerCount == 0)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    // Callback lorsque le joueur quitte le salon avec succès
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Callback pour gérer les déconnexions ou problèmes techniques
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        statusText.text = "Déconnecté de Photon. Cause : " + cause.ToString();
        SceneManager.LoadScene("MainMenu");
    }
}

