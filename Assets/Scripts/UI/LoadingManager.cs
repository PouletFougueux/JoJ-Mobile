using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Pour charger la scène suivante

public class LoadingTextManager : MonoBehaviour
{
    public GameObject[] letterImages; // Tableau de GameObjects (Images) représentant les lettres "L O A D I N G"
    public float letterDisplayDelay = 0.3f; // Délai entre l'affichage des lettres
    public string sceneToLoad = "MainMenu"; // Nom de la scène à charger

    void Start()
    {
        StartCoroutine(ShowLoadingText());
    }

    IEnumerator ShowLoadingText()
    {
        // Afficher chaque lettre une par une
        for (int i = 0; i < letterImages.Length; i++)
        {
            letterImages[i].SetActive(true); // Afficher l'image de la lettre
            yield return new WaitForSeconds(letterDisplayDelay); // Attendre avant de montrer la lettre suivante
        }

        // Attendre un peu avant de charger la scène (facultatif)
        yield return new WaitForSeconds(1f);

        LoadNextScene();
    }

    // Méthode pour charger la scène suivante
    void LoadNextScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad); // Charge la scène de manière asynchrone
        asyncLoad.allowSceneActivation = true; // Permet à la scène de s'activer une fois chargée
    }
}
