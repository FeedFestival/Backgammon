using UnityEngine;
using System.Collections;
using Assets.Scripts.Main;
using GameSparks;
using GameSparks.Api;
using GameSparks.Api.Messages;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Core;

public class GameSparksController : MonoBehaviour
{
    private Main _main;

    //private User _user = new User
    //{
    //    UserName = "admin",
    //    Password = "fire4test"
    //};

    public void RegisterPlayer()
    {
        new RegistrationRequest()
            .SetDisplayName(_main.LoggedUser.UserName)
            .SetUserName(_main.LoggedUser.Email)
            .SetPassword(_main.LoggedUser.Password)
            .Send(response =>
            {
                if (response.HasErrors)
                    Debug.Log("Error registering player.");
                else
                    Debug.Log("Player registered.");
            }
            );
    }

    public void Login()
    {
        new AuthenticationRequest()
            .SetUserName(_main.LoggedUser.Email)
            .SetPassword(_main.LoggedUser.Password)
            .Send(response =>
        {
            if (response.HasErrors)
            {
                // Probably hes not registered.
                RegisterPlayer();
                Debug.Log("Regisering player");
            }
            else
            {
                Debug.Log("Is logged in");
                // now we can init Main.Game.Init();

                InitPlayerSession();
            }
        });
    }

    public void InitPlayerSession()
    {
        string json = @"
          {
            ""ffsUserId"" : ""1""
          }
        ";

        new LogEventRequest()
            .SetEventKey("InitPlayerSession")
            .SetEventAttribute("playerSettings", json)
            .Send(response =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("Player Saved To GameSparks...");
                }
                else
                {
                    Debug.Log("Error Saving Player Data...");
                }
            });


    }

    public void Init(Main main)
    {
        _main = main;

        GS.GameSparksAvailable += (available) =>
        {
            if (available)
            {
                // Check if player exists by trying to authentificate.
                Login();
            }
        };
    }
}