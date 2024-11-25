using UnityEngine;
using System.Collections.Generic;

public class QuestionManager : MonoBehaviour
{
    private List<Theme> themes; // Liste des thèmes et questions
    private List<Question> currentThemeQuestions; // Liste des questions du thème sélectionné
    private int currentThemeIndex = -1; // Indice du thème actuel (sera récupéré de GameSettings)
    private int currentQuestionIndex = 0; // Indice de la question actuelle

    void Start()
    {
        LoadQuestionsFromResources("questions"); // Charge les questions au démarrage

        currentThemeIndex = GameSettings.SelectedThemeIndex;  // Récupère l'indice du thème sélectionné
        if (currentThemeIndex == -1)
        {
            Debug.LogError("L'indice du thème n'a pas été correctement initialisé !");
        }
        else
        {
            // Charger uniquement les questions du thème sélectionné
            LoadQuestionsForSelectedTheme();
        }
    }

    public void LoadQuestionsFromResources(string path)
    {
        TextAsset jsonText = Resources.Load<TextAsset>(path);
        if (jsonText != null)
        {
            ThemesData themesData = JsonUtility.FromJson<ThemesData>(jsonText.ToString());
            themes = themesData.themes;
        }
        else
        {
            Debug.LogError("Le fichier JSON des questions n'a pas été trouvé dans le dossier Resources !");
            themes = new List<Theme>();
        }
    }

    // Charger uniquement les questions du thème sélectionné
    private void LoadQuestionsForSelectedTheme()
    {
        if (currentThemeIndex >= 0 && currentThemeIndex < themes.Count)
        {
            Theme selectedTheme = themes[currentThemeIndex];
            currentThemeQuestions = selectedTheme.questions;
        }
        else
        {
            Debug.LogError("L'indice du thème actuel est hors limites ou les données des thèmes sont invalides.");
            currentThemeQuestions = new List<Question>(); // Liste vide pour éviter les erreurs
        }
    }

    // Récupère la question actuelle
    public Question GetCurrentQuestion()
    {
        if (currentQuestionIndex >= 0 && currentQuestionIndex < currentThemeQuestions.Count)
        {
            return currentThemeQuestions[currentQuestionIndex];
        }
        Debug.LogWarning("Aucune question disponible ou indice de question hors limites.");
        return null;
    }

    // Charge la prochaine question
    public void LoadNextQuestion()
    {
        currentQuestionIndex++;
        if (currentQuestionIndex >= currentThemeQuestions.Count)
        {
            Debug.Log("Fin des questions pour ce thème.");
            currentQuestionIndex = 0; // Optionnel : revenir à la première question
        }
    }

    public bool HasNextQuestion()
    {
        return currentQuestionIndex < currentThemeQuestions.Count;
    }

    [System.Serializable]
    public class ThemesData
    {
        public List<Theme> themes;
    }

    [System.Serializable]
    public class Theme
    {
        public string themeName;
        public List<Question> questions;
    }

    [System.Serializable]
    public class Question
    {
        public string questionText;
        public int correctAnswer;
    }
}

