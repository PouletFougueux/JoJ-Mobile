using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class ModeManager : MonoBehaviourPunCallbacks
{
    // Références UI
    public GameObject[] themeButtons;  // 9 boutons pour les thèmes
    public Slider modeSlider;          // Slider pour choisir entre 0 et 1
    public GameObject modeUI;          // UI qui contient les éléments à désactiver pour les non-hôtes
    public Button backButton;          // Bouton pour revenir à la salle d'attente

    private int selectedThemeIndex = -1;  // Indice du thème sélectionné (0 à 8)
    private float selectedMode = 0f;      // Valeur du Slider choisie (0 ou 1)

    void Start()
    {
        // Initialisation des boutons de thèmes et de leur écouteur
        for (int i = 0; i < themeButtons.Length; i++)
        {
            int index = i;  // Capture de l'indice pour l'écouteur
            Button btn = themeButtons[i].GetComponent<Button>();
            btn.onClick.AddListener(() => OnThemeButtonClicked(index));
        }

        // Le slider est utilisé uniquement par l'hôte
        if (PhotonNetwork.IsMasterClient)
        {
            modeSlider.onValueChanged.AddListener(OnSliderValueChanged);
            backButton.interactable = true; // Activer le bouton Retour pour l'hôte
        }
        else
        {
            // Griser le slider et désactiver le bouton Retour pour les non-hôtes
            SetSliderGrayedOut(true);
            backButton.interactable = false;
        }

        // Désactiver l'interaction avec les boutons de thèmes pour les non-hôtes
        if (!PhotonNetwork.IsMasterClient)
        {
            foreach (GameObject btn in themeButtons)
            {
                btn.GetComponent<Button>().interactable = false;
            }
        }

        // Ajouter l'écouteur pour le bouton Retour
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    // Fonction pour griser/désactiver le slider (pour les non-hôtes)
    void SetSliderGrayedOut(bool isGrayedOut)
    {
        modeSlider.interactable = !isGrayedOut;

        // Vous pouvez également appliquer un effet visuel comme un changement de couleur pour griser le slider
        Color color = isGrayedOut ? Color.gray : Color.white;
        modeSlider.GetComponentInChildren<Image>().color = color;  // Modifier la couleur du fond du slider
    }

    // Lorsque le chef sélectionne un thème
    void OnThemeButtonClicked(int themeIndex)
    {
        if (PhotonNetwork.IsMasterClient)  // Seulement l'hôte peut choisir le thème
        {
            selectedThemeIndex = themeIndex;

            // Sauvegarder la valeur sélectionnée dans une variable statique
            GameSettings.SelectedThemeIndex = selectedThemeIndex;

            Debug.Log("Thème sélectionné par l'hôte: " + selectedThemeIndex);
        }
    }

    // Fonction appelée lorsque la valeur du Slider change
    void OnSliderValueChanged(float value)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            selectedMode = Mathf.RoundToInt(value);  // Arrondir à 0 ou 1
            Debug.Log("Mode sélectionné par l'hôte: " + selectedMode);
        }
    }

    // Quand le chef appuie sur le bouton démarrer la partie
    public void OnStartGameButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("StartGame", RpcTarget.All);
        }
    }

    // RPC pour démarrer la partie
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

    // RPC pour revenir à la salle d'attente
    [PunRPC]
    public void ReturnToWaitingRoom()
    {
        PhotonNetwork.LoadLevel("WaitingRoom");
    }
}
