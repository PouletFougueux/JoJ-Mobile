using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Liste des joueurs, avec leurs scores
    public List<Player> players = new List<Player>();

    // Méthode pour ajouter un joueur
    public void AddPlayer(Player player)
    {
        players.Add(player);
    }

    // Méthode pour récupérer la liste des joueurs
    public List<Player> GetPlayers()
    {
        return players;
    }

    // Méthode pour incrémenter le score d'un joueur
    public void AddScoreToPlayer(int playerId, int score)
    {
        Player player = players.Find(p => p.id == playerId);
        if (player != null)
        {
            player.score += score;
        }
    }
}

[System.Serializable]
public class Player
{
    public int id;
    public string name;
    public int score;
}
