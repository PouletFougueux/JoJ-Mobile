using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    // R�f�rences pour l'UI
    public TMP_Text codeSalonText;           // Pour afficher le code g�n�r� du salon
    public TMP_InputField codeInputField;    // Pour saisir le code du salon � rejoindre
    public TMP_InputField playerNameInputField; // Champ pour saisir le pseudo du joueur
    public Button createRoomButton;          // Bouton pour cr�er un salon
    public Button joinRoomButton;            // Bouton pour rejoindre un salon
    public TMP_Text statusText;             // Pour afficher des messages de statut
    public Color32 color;

    private string generatedRoomCode;        // Le code de salon g�n�r�

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();

        // Initialisation des boutons avec les �v�nements
        createRoomButton.onClick.AddListener(CreateRoom);
        joinRoomButton.onClick.AddListener(JoinRoom);

        // Message de statut initial
        statusText.text = "Entrez un pseudo et un code de salon pour commencer.";
        statusText.color = color; // On d�finit la couleur du message initial en noir
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server.");
    }

    // Fonction pour g�n�rer un code de salon al�atoire (5 lettres)
    string GenerateRoomCode()
    {
        string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string code = "";

        for (int i = 0; i < 5; i++) // G�n�re un code de 5 lettres
        {
            int randomIndex = Random.Range(0, characters.Length);
            code += characters[randomIndex];
        }

        return code;
    }

    // Fonction pour cr�er un salon
    void CreateRoom()
    {
        if (SetPlayerName())
        {
            generatedRoomCode = GenerateRoomCode(); // G�n�re un code unique pour le salon
            codeSalonText.text = "Code du salon : " + generatedRoomCode; // Affiche le code dans l'UI

            // Tentative de cr�ation d'une salle
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;
            roomOptions.MaxPlayers = 4; // Limite du nombre de joueurs
            PhotonNetwork.CreateRoom(generatedRoomCode, roomOptions); // Cr�e le salon
            statusText.text = "Cr�ation du salon en cours..."; // Message de statut
            statusText.color = color; // R�initialise la couleur en noir
        }
    }

    // Fonction pour rejoindre un salon avec un code
    void JoinRoom()
    {
        if (SetPlayerName())
        {
            string roomCode = codeInputField.text; // R�cup�re le code saisi dans l'InputField

            if (!string.IsNullOrEmpty(roomCode)) // V�rifie que le code n'est pas vide
            {
                PhotonNetwork.JoinRoom(roomCode); // Essaye de rejoindre la salle
                statusText.text = "Tentative de rejoindre le salon..."; // Message de statut
                statusText.color = color; // R�initialise la couleur en noir
            }
            else
            {
                SetErrorMessage("Veuillez entrer un code de salon."); // Affiche le message d'erreur
            }
        }
    }

    // D�finit le pseudo du joueur avec validation
    bool SetPlayerName()
    {
        string playerName = playerNameInputField.text; // R�cup�re le pseudo saisi

        // V�rification de la longueur du pseudo (entre 2 et 10 caract�res)
        if (playerName.Length < 2 || playerName.Length > 10)
        {
            SetErrorMessage("Le pseudo doit �tre compris entre 2 et 10 caract�res.");
            return false;
        }

        // V�rification des caract�res valides : uniquement lettres et chiffres
        if (!Regex.IsMatch(playerName, @"^[a-zA-Z0-9]+$"))
        {
            SetErrorMessage("Le pseudo ne peut contenir que des lettres et des chiffres.");
            return false;
        }

        PhotonNetwork.NickName = playerName; // D�finit le pseudo pour Photon
        return true;
    }

    // Fonction pour afficher les messages d'erreur en rouge
    void SetErrorMessage(string message)
    {
        statusText.text = message;
        statusText.color = Color.red; // Met la couleur du texte en rouge
    }

    // Callback lorsque la salle est cr��e avec succ�s
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        statusText.text = "Salon cr�� avec succ�s. Code: " + generatedRoomCode;
        statusText.color = Color.green; // Message de succ�s en vert
    }

    // Callback lorsque l'on rejoint une salle avec succ�s
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        statusText.text = "Vous avez rejoint le salon avec succ�s!";
        statusText.color = Color.green; // Message de succ�s en vert
        SceneManager.LoadScene("WaitingRoom");
    }

    // Callback lorsque l'entr�e dans une salle �choue
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        SetErrorMessage("Code du salon invalide. V�rifiez le code et r�essayez."); // Affiche l'erreur en rouge
    }
}

