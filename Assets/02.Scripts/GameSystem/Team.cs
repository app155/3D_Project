using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Teams
{
    None,
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

    public int id { get; set; }

    private List<ulong> _playersInTeam = new List<ulong>();
    private int _score;
    private int _id;

    public Team(int id)
    {
        this.id = id;
    }

    public void Register(ulong clientID)
    {
        _playersInTeam.Add(clientID);
    }
}
