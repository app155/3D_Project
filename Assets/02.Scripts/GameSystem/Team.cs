using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Teams
{
    Blue,
    Red,
}

public class Team
{
    public int score
    {
        get => _score;
        set
        {
            _score = value;
        }
    }

    public int id => _id;

    private List<ulong> _playersInTeam = new List<ulong>();
    private int _score;
    private int _id;

    public Team(int id)
    {
        _id = id;
    }

    public Team Register(ulong clientID)
    {
        _playersInTeam.Add(clientID);

        Debug.Log($"{clientID} registered team {id}");

        return this;
    }

    public List<ulong> GetPlayersInTeam()
    {
        return _playersInTeam;
    }
}
