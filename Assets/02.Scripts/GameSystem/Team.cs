using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    public int score
    {
        get => _score;
        set
        {
            // asdfasdfsf
        }
    }

    private List<ulong> _playersInTeam = new List<ulong>();
    private int _score;

    public void Register(ulong clientID)
    {
        _playersInTeam.Add(clientID);
    }
}
