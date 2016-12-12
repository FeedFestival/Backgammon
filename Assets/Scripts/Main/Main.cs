using System.Collections.Specialized;
using Assets.Scripts.Main;
using UnityEngine;

public class Main : MonoBehaviour
{
    [HideInInspector]
    public CanvasController CanvasController;

    [HideInInspector]
    public PhpController PhpController;

    [HideInInspector]
    public GameSparksController GameSparksController;

    [HideInInspector] public Game Game;

    [HideInInspector]
    public User LoggedUser;

    public void Init(Game game)
    {
        Game = game;

        PhpController = GetComponent<PhpController>();
        PhpController.Init(this);

        GameSparksController = GetComponent<GameSparksController>();

        // we do this after the success of obtaining a game sessionId handshake
        //GameSparksController.Init(this);

        Transform[] allChildren = GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            switch (child.name)
            {
                case "<body":
                    CanvasController = child.GetComponent<CanvasController>();
                    break;

                default:
                    break;
            }
        }

        CanvasController.Init(this);
    }
}