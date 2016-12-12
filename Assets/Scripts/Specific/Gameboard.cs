using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameSparks.Api.Messages;

public class Gameboard : MonoBehaviour
{
    public List<Checker> Chekers;
    public List<Space> Spaces;

    void Start()
    {

    }

    void AddSpaces()
    {
        Spaces = new List<Space>();

        Team team;

        for (var i = 0; i <= 25; i++)
        {
            var space = new Space
            {
                Index = i
            };

            switch (i)
            {
                case 1:
                case 24:
                    team = i == 1 ? Team.Black : Team.White;
                    
                    space.Checkers = AddCheckers(2, team);
                    break;
                case 5:
                case 19:
                    team = i == 19 ? Team.Black : Team.White;
                    
                    space.Checkers = AddCheckers(5, team);
                    break;
                case 8:
                case 17:
                    team = i == 17 ? Team.Black : Team.White;
                    
                    space.Checkers = AddCheckers(3, team);
                    break;
                case 12:
                case 13:
                    team = i == 12 ? Team.Black : Team.White;
                    
                    space.Checkers = AddCheckers(5, team);
                    break;
            }
            Spaces.Add(space);
        }
    }

    List<Checker> AddCheckers(int howMany, Team team)
    {
        var checkers = new List<Checker>();

        for (var i = 0; i < howMany; i++)
        {
            var checker = new Checker
            {
                SpaceIndex = i,
                Team = team
            };
        }
        return checkers;
    }
}

public class Space
{
    public int Index;
    public List<Checker> Checkers;
}

public class Checker
{
    public Team Team;
    public int SpaceIndex;
}

public enum Team
{
    White,
    Black
}