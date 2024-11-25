using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class ModeManager : MonoBehaviourPunCallbacks
{
    // R�f�rences UI
    public GameObject[] themeButtons;  // 9 boutons pour les th�mes
    public Slider modeSlider;          // Slider pour choisir entre 0 et 1
    public GameObject modeUI;          // UI qui contient les �l�ments � d�sactiver pour les non-h�tes
    public Button backButton;          // Bouton pour revenir � la salle d'attente

    private int selectedThemeIndex = -1;  // Indice du th�me s�lectionn� (0 � 8)
    private float selectedMode = 0f;      // Valeur du Slider choisie (0 ou 1)

    void Start()
    {
        // Initialisation des boutons de th�mes et de leur �couteur
        for (int i = 0; i < themeButtons.Length; i++)
        {
            int index = i;  // Capture de l'indice pour l'�couteur
            Button btn = themeButtons[i].GetComponent<Button>();
            btn.onClick.AddListener(() => OnThemeButtonClicked(index));
        }

        // Le slider est utilis� uniquement par l'h�te
        if (PhotonNetwork.IsMasterClient)
        {
            modeSlider.onValueChanged.AddListener(OnSliderValueChanged);
            backButton.interactable = true; // Activer le bouton Retour pour l'h�te
        }
        else
        {
            // Griser le slider et d�sactiver le bouton Retour pour les non-h�tes
            SetSliderGrayedOut(true);
            backButton.interactable = false;
        }

        // D�sactiver l'interaction avec les boutons de th�mes pour les non-h�tes
        if (!PhotonNetwork.IsMasterClient)
        {
            foreach (GameObject btn in themeButtons)
            {
                btn.GetComponent<Button>().interactable = false;
            }
        }

        // Ajouter l'�couteur pour le bouton Retour
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    // Fonction pour griser/d�sactiver le slider (pour les non-h�tes)
    void SetSliderGrayedOut(bool isGrayedOut)
    {
        modeSlider.interactable = !isGrayedOut;

        // Vous pouvez �galement appliquer un effet visuel comme un changement de couleur pour griser le slider
        Color color = isGrayedOut ? Color.gray : Color.white;
        modeSlider.GetComponentInChildren<Image>().color = color;  // Modifier la couleur du fond du slider
    }

    // Lorsque le chef s�lectionne un th�me
    void OnThemeButtonClicked(int themeIndex)
    {
        if (PhotonNetwork.IsMasterClient)  // Seulement l'h�te peut choisir le th�me
        {
            selectedThemeIndex = themeIndex;

            // Sauvegarder la valeur s�lectionn�e dans une variable statique
            GameSettings.SelectedThemeIndex = selectedThemeIndex;

            Debug.Log("Th�me s�lectionn� par l'h�te: " + selectedThemeIndex);
        }
    }

    // Fonction appel�e lorsque la valeur du Slider change
    void OnSliderValueChanged(float value)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            selectedMode = Mathf.RoundToInt(value);  // Arrondir � 0 ou 1
            Debug.Log("Mode s�lectionn� par l'h�te: " + selectedMode);
        }
    }

    // Quand le chef appuie sur le bouton d�marrer la partie
    public void OnStartGameButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("StartGame", RpcTarget.All);
        }
    }

    // RPC pour d�marrer la partie
    [PunRPC]
    public void StartGame()
    {
        PhotonNetwork.LoadLevel("SoloGame");
    }

    // Lorsque le chef clique sur le bouton Retour
    public void OnBackButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ReturnToWaitingRoom", RpcTarget.All);
        }
    }

    // RPC pour revenir � la salle d'attente
    [PunRPC]
    public void ReturnToWaitingRoom()
    {
        PhotonNetwork.LoadLevel("WaitingRoom");
    }
}
