using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Pour charger la sc�ne suivante

public class LoadingTextManager : MonoBehaviour
{
    public GameObject[] letterImages; // Tableau de GameObjects (Images) repr�sentant les lettres "L O A D I N G"
    public float letterDisplayDelay = 0.3f; // D�lai entre l'affichage des lettres
    public string sceneToLoad = "MainMenu"; // Nom de la sc�ne � charger

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

        // Attendre un peu avant de charger la sc�ne (facultatif)
        yield return new WaitForSeconds(1f);

        LoadNextScene();
    }

    // M�thode pour charger la sc�ne suivante
    void LoadNextScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad); // Charge la sc�ne de mani�re asynchrone
        asyncLoad.allowSceneActivation = true; // Permet � la sc�ne de s'activer une fois charg�e
    }
}
