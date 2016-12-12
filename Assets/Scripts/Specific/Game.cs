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
        SetSession("B2C79103-A772-4E27-A3BE-E57E452E4C36");
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