using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI questionText;    // Texte pour afficher les questions
    public TextMeshProUGUI messageText;     // Texte pour afficher les messages
    public TextMeshProUGUI[] playerScores;  // Textes pour afficher les scores des joueurs

    public void DisplayQuestion(string question)
    {
        questionText.text = question;
    }

    public void DisplayMessage(string message)
    {
        messageText.text = message;
    }

    public void UpdatePlayerScore(int playerID, int score)
    {
        playerScores[playerID].text = $"Joueur {playerID + 1} : {score} points";
    }
}
