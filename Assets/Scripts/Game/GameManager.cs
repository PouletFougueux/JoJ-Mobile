using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;
using ExitGames.Client.Photon;
using static QuestionManager;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject boxconteneur1; // Phase de question
    public GameObject boxconteneur2; // Phase du tableau des scores
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI timerText;
    public Slider answerSlider;
    public Button submitAnswerButton;

    public Transform leaderboardContent; // Conteneur pour les rangées des joueurs
    public GameObject playerRowPrefab;  // Prefab pour une ligne du leaderboard

    public QuestionManager questionManager;  // Référence au QuestionManager

    private int pointsToWin = 10;
    private float timer = 30f;
    private bool isQuestionPhase = true;

    void Start()
    {
        // Assurez-vous que QuestionManager est assigné et que les questions sont chargées
        questionManager.LoadQuestionsFromResources("questions");  // Charge les questions depuis le fichier JSON
        ShowQuestionPhase();
        submitAnswerButton.onClick.AddListener(SubmitAnswer);
    }

    void Update()
    {
        if (isQuestionPhase)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timer).ToString();

            if (timer <= 0)
            {
                EndQuestionAndShowScores();
            }
        }
    }

    void ShowQuestionPhase()
    {
        boxconteneur1.SetActive(true);
        boxconteneur2.SetActive(false);

        Question currentQuestion = questionManager.GetCurrentQuestion();
        if (currentQuestion != null)
        {
            questionText.text = currentQuestion.questionText;
            answerSlider.value = 0;
            timer = 20f;
        }
        else
        {
            EndGame();
        }
    }

    void EndQuestionAndShowScores()
    {
        boxconteneur1.SetActive(false);
        boxconteneur2.SetActive(true);
        UpdateScoreBoard();
        StartCoroutine(NextQuestionDelay());
    }

    void UpdateScoreBoard()
    {
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
        var sortedPlayers = players.OrderByDescending(player => GetPlayerScore(player)).ToList();

        foreach (var player in sortedPlayers)
        {
            int playerScore = GetPlayerScore(player);

            GameObject playerRow = Instantiate(playerRowPrefab, leaderboardContent);
            PlayerRowUI rowUI = playerRow.GetComponent<PlayerRowUI>();
            if (rowUI != null)
            {
                rowUI.playerNameText.text = player.NickName;
                rowUI.progressBarSlider.value = (float)playerScore / pointsToWin;
                rowUI.scoreText.text = playerScore.ToString();
            }
        }
    }

    int GetPlayerScore(Photon.Realtime.Player player)
    {
        if (player.CustomProperties.ContainsKey("score"))
        {
            return (int)player.CustomProperties["score"];
        }
        return 0;
    }

    void AddPlayerScore(Photon.Realtime.Player player, int points)
    {
        int currentScore = GetPlayerScore(player);
        int newScore = currentScore + points;
        player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "score", newScore } });
        RaiseScoreUpdateEvent(player, newScore);
    }

    void RaiseScoreUpdateEvent(Photon.Realtime.Player player, int newScore)
    {
        object[] content = new object[] { player.NickName, newScore };
        byte eventCode = 1;
        PhotonNetwork.RaiseEvent(eventCode, content, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 1)
        {
            object[] data = (object[])photonEvent.CustomData;
            string playerName = (string)data[0];
            int playerScore = (int)data[1];
            UpdatePlayerScoreInUI(playerName, playerScore);
        }
    }

    void UpdatePlayerScoreInUI(string playerName, int playerScore)
    {
        foreach (Transform child in leaderboardContent)
        {
            PlayerRowUI rowUI = child.GetComponent<PlayerRowUI>();
            if (rowUI != null && rowUI.playerNameText.text == playerName)
            {
                rowUI.scoreText.text = playerScore.ToString();
                rowUI.progressBarSlider.value = (float)playerScore / pointsToWin;
                break;
            }
        }
    }

    IEnumerator NextQuestionDelay()
    {
        yield return new WaitForSeconds(5f);
        questionManager.LoadNextQuestion();
        ShowQuestionPhase();
    }

    public void SubmitAnswer()
    {
        if (isQuestionPhase)
        {
            ProcessAnswer();
        }
    }

    private void ProcessAnswer()
    {
        int playerAnswer = Mathf.RoundToInt(answerSlider.value);
        Question currentQuestion = questionManager.GetCurrentQuestion();

        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
        int closestPlayerIndex = -1;
        int minDifference = int.MaxValue;

        foreach (var player in players)
        {
            int playerDifference = Mathf.Abs(playerAnswer - currentQuestion.correctAnswer);
            if (playerDifference < minDifference)
            {
                minDifference = playerDifference;
                closestPlayerIndex = Array.IndexOf(players, player);
            }
        }

        if (PhotonNetwork.PlayerList.Length == 1)
        {
            AddPlayerScore(PhotonNetwork.LocalPlayer, 1);
        }
        else if (closestPlayerIndex != -1)
        {
            Photon.Realtime.Player closestPlayer = players[closestPlayerIndex];
            AddPlayerScore(closestPlayer, 1);
        }

        Photon.Realtime.Player winner = CheckForWinner();
        if (winner != null)
        {
            EndGame(winner);
        }
        else
        {
            EndQuestionAndShowScores();
        }
    }

    Photon.Realtime.Player CheckForWinner()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (GetPlayerScore(player) >= pointsToWin)
            {
                return player;
            }
        }
        return null;
    }

    void EndGame(Photon.Realtime.Player winner = null)
    {
        if (winner != null)
        {
            Debug.Log($"{winner.NickName} a gagné avec {GetPlayerScore(winner)} points !");
        }
        else
        {
            Debug.Log("La partie est terminée !");
        }

        boxconteneur1.SetActive(false);
        boxconteneur2.SetActive(false);
        submitAnswerButton.gameObject.SetActive(false);
    }
}

