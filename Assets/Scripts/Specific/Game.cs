using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Assets.Scripts.Main;
using Org.BouncyCastle.Asn1.Mozilla;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    //private User _user = new User
    //{
    //    UserName = "admin",
    //    Password = "fire4test"
    //};

    // username: ""
    // password: "stBlW0PBT7"

    // sim.danny
    // fire4test

    #region Game

    public Main Main;

    [HideInInspector]
    public Dictionary<string, GameObject> scope;

    void Awake()
    {
        Main.Init(this);

        InitButtonLogic();

        // TODO: On the loader we should have a message : Seting up session, while we wait for javascript and php.

        // This is a script that exists in the game.html
        // this script when is called - tells angular that unity is ready to recieve the game session id.
        Application.ExternalCall("setGameReady");

        //  this is called from te browser, TODO: delete before publish.

        //SetSession("3B37A1D1-29FF-4484-926F-8C0BBB0B9B16");








        var user = new User
        {
            Id = 1,
            UserName = "sim.danny",
            FirstName = "Dany",
            FUniqueId = "OEPZMaDU0lWZR_wapoeERx-7s-g",
            Password = "fire4test"
        };
        Main.LoggedUser = user;

        //var user = new User
        //{
        //    UserName = "dany.flory",
        //    FirstName = "FloryDany",
        //    FUniqueId = "5ZuD5B7a4OD5MNMQAHOWPHq5O5g",
        //    Password = "stBlW0PBT7"    
        //};
        //Main.LoggedUser = user;

        Main.GameSparksController.Init(Main);
    }

    public void SetSession(string sessionId)
    {
        // avoid error: m_size < k_reference_bit
        var t =  scope["SessionIdLabel"].GetComponent<Text>();
        t.text = sessionId;

        Main.PhpController.HandshakeSession(sessionId);
    }

    public void HandshakeSessionCallback(bool success, string fUniqueId = null)
    {
        if (success)
            Main.PhpController.FetchUser(fUniqueId);
        else
        {
            Debug.LogError("Couldn't handshake Session.");
        }
    }

    public void FetchUserCallback(User user)
    {
        Main.LoggedUser = user;
        Debug.Log(Main.LoggedUser.Password);
        Main.GameSparksController.Init(Main);
    }

    #endregion
    
    public void InitButtonLogic()
    {
        scope["ToggleChatButton"].GetComponent<Button>().onClick.AddListener( // <-- you assign a method to the button OnClick event here
            () =>
            {
                scope["ChatView"].SetActive(!scope["ChatView"].activeSelf);
            });

        scope["SendMessageButton"].GetComponent<Button>().onClick.AddListener(
            () =>
            {
                Debug.Log("Send Message");
            });
    }
}