using UnityEngine;
using System.Collections;
using System.Linq;
using System.Text;
using Assets.Scripts.Main;
using GameSparks;
using GameSparks.Api;
using GameSparks.Api.Messages;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Core;
using GameSparks.RT;
using UnityEngine.UI;

public class GameSparksController : MonoBehaviour
{
    private Main _main;

    /// <summary>The GameSparks Manager singleton</summary>
    private static GameSparksController instance = null;
    /// <summary>This method will return the current instance of this class </summary>
    public static GameSparksController Instance()
    {
        if (instance != null)
        {
            return instance; // return the singleton if the instance has been setup
        }
        else
        { // otherwise return an error
            Debug.LogError("GSM| GameSparksManager Not Initialized...");
        }
        return null;
    }
    void Awake()
    {
        instance = this; // if not, give it a reference to this class...
        DontDestroyOnLoad(this.gameObject); // and make this object persistent as we load new scenes
    }

    public void InitPlayerSession()
    {
        /*
         
        "UserName": "sim.danny",
          "DisplayName": "Dany",
          "ffsUserId": "1",
          "FUniqueId" : "OEPZMaDU0lWZR_wapoeERx-7s-g"

        */
        string json = " { " +
                      "\"UserName\" : \"" + _main.LoggedUser.UserName + "\", " +
                      "\"DisplayName\": \"" + _main.LoggedUser.FirstName + "\", " +
                      "\"ffsUserId\": \"" + _main.LoggedUser.Id + "\", " +
                      "\"FUniqueId\": \"" + _main.LoggedUser.FUniqueId + "\"" +
                      " } ";

        Debug.Log(json);

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

        InitChat();
    }

    private void InitChat()
    {
    }

    public void FindFriends()
    {

    }

    public void FindPlayers()
    {
        GameSparks.Api.Messages.MatchNotFoundMessage.Listener = (message) =>
        {
            Debug.Log("No Match Found..." + message.ToString());
        };

        GameSparks.Api.Messages.MatchFoundMessage.Listener += OnMatchFound;

        Debug.Log("GSM| Attempting Matchmaking...");
        new GameSparks.Api.Requests.MatchmakingRequest()
            .SetMatchShortCode("Backgammon_Chat") // set the shortCode to be the same as the one we created in the first tutorial
            .SetSkill(0) // in this case we assume all players have skill level zero and we want anyone to be able to join so the skill level for the request is set to zero
            .Send((response) =>
            {
                if (response.HasErrors)
                { // check for errors
                    Debug.LogError("GSM| MatchMaking Error \n" + response.Errors.JSON);
                }
            });
    }

    private void OnMatchFound(GameSparks.Api.Messages.MatchFoundMessage _message)
    {
        Debug.Log("Match Found!...");
        StringBuilder sBuilder = new StringBuilder();
        sBuilder.AppendLine("Match Found...");
        sBuilder.AppendLine("Host URL:" + _message.Host);
        sBuilder.AppendLine("Port:" + _message.Port);
        sBuilder.AppendLine("Access Token:" + _message.AccessToken);
        sBuilder.AppendLine("MatchId:" + _message.MatchId);
        sBuilder.AppendLine("Opponents:" + _message.Participants.Count());
        sBuilder.AppendLine("_________________");
        sBuilder.AppendLine(); // we'll leave a space between the player-list and the match data
        foreach (GameSparks.Api.Messages.MatchFoundMessage._Participant player in _message.Participants)
        {
            sBuilder.AppendLine("Player:" + player.PeerId + " User Name:" + player.DisplayName); // add the player number and the display name to the list
        }
        Debug.Log(sBuilder.ToString()); // set the string to be the player-list field
    }

    public void Init(Main main)
    {
        _main = main;

        GS.GameSparksAvailable += (available) =>
        {
            if (available)
            {
                AuthenticateUser(_main.LoggedUser.FUniqueId, _main.LoggedUser.Password, OnRegistration, OnAuthentication);
            }
        };
    }

    private void OnAuthentication(AuthenticationResponse response)
    {
        Debug.Log("User Authenticated...");

        //FindPlayers();    -> init match making

        InitPlayerSession();
    }

    private void OnRegistration(RegistrationResponse response)
    {
        Debug.Log("New User Registered...");
    }

    public delegate void AuthCallback(AuthenticationResponse _authresp2);
    public delegate void RegCallback(RegistrationResponse _authResp);
    /// <summary>
    /// Sends an authentication request or registration request to GS.
    /// </summary>
    /// <param name="_callback1">Auth-Response</param>
    /// <param name="_callback2">Registration-Response</param>
    public void AuthenticateUser(string _userName, string _password, RegCallback _regcallback, AuthCallback _authcallback)
    {
        new GameSparks.Api.Requests.RegistrationRequest()
                  // this login method first attempts a registration //
                  // if the player is not new, we will be able to tell as the registrationResponse has a bool 'NewPlayer' which we can check
                  // for this example we use the user-name was the display name also //
                  .SetDisplayName(_main.LoggedUser.FirstName)
                  .SetUserName(_userName)
                  .SetPassword(_password)
                  .Send((regResp) =>
                  {
                      if (!regResp.HasErrors)
                      { // if we get the response back with no errors then the registration was successful
                          Debug.Log("GSM| Registration Successful...");
                          _regcallback(regResp);
                      }
                      else
                      {
                          // if we receive errors in the response, then the first thing we check is if the player is new or not
                          if (!(bool)regResp.NewPlayer) // player already registered, lets authenticate instead
                          {
                              Debug.LogWarning("GSM| Existing User, Switching to Authentication");
                              new GameSparks.Api.Requests.AuthenticationRequest()
                                  .SetUserName(_userName)
                                  .SetPassword(_password)
                                  .Send((authResp) =>
                                  {
                                      if (!authResp.HasErrors)
                                      {
                                          Debug.Log("Authentication Successful...");
                                          _authcallback(authResp);
                                      }
                                      else
                                      {
                                          Debug.LogWarning("GSM| Error Authenticating User \n" + authResp.Errors.JSON);
                                      }
                                  });
                          }
                          else
                          {
                              // if there is another error, then the registration must have failed
                              Debug.LogWarning("GSM| Error Authenticating User \n" + regResp.Errors.JSON);
                          }
                      }
                  });
    }
}