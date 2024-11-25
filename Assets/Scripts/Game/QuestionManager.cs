using UnityEngine;
using System.Collections.Generic;

public class QuestionManager : MonoBehaviour
{
    private List<Theme> themes; // Liste des th�mes et questions
    private List<Question> currentThemeQuestions; // Liste des questions du th�me s�lectionn�
    private int currentThemeIndex = -1; // Indice du th�me actuel (sera r�cup�r� de GameSettings)
    private int currentQuestionIndex = 0; // Indice de la question actuelle

    void Start()
    {
        LoadQuestionsFromResources("questions"); // Charge les questions au d�marrage

        currentThemeIndex = GameSettings.SelectedThemeIndex;  // R�cup�re l'indice du th�me s�lectionn�
        if (currentThemeIndex == -1)
        {
            Debug.LogError("L'indice du th�me n'a pas �t� correctement initialis� !");
        }
        else
        {
            // Charger uniquement les questions du th�me s�lectionn�
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
            Debug.LogError("Le fichier JSON des questions n'a pas �t� trouv� dans le dossier Resources !");
            themes = new List<Theme>();
        }
    }

    // Charger uniquement les questions du th�me s�lectionn�
    private void LoadQuestionsForSelectedTheme()
    {
        if (currentThemeIndex >= 0 && currentThemeIndex < themes.Count)
        {
            Theme selectedTheme = themes[currentThemeIndex];
            currentThemeQuestions = selectedTheme.questions;
        }
        else
        {
            Debug.LogError("L'indice du th�me actuel est hors limites ou les donn�es des th�mes sont invalides.");
            currentThemeQuestions = new List<Question>(); // Liste vide pour �viter les erreurs
        }
    }

    // R�cup�re la question actuelle
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
            Debug.Log("Fin des questions pour ce th�me.");
            currentQuestionIndex = 0; // Optionnel : revenir � la premi�re question
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

