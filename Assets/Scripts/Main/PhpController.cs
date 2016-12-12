using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LitJson;
using System.Configuration;
using Assets.Scripts.Main;
using Assets.Scripts.Utils;
using UnityEditor.iOS.Xcode;

public class PhpController : MonoBehaviour
{
    private const string LocalUrl = "http://localhost:8080/FFS/backend";
    private const string UnityInitGame = LocalUrl + "/Session_UnityInitGame.php";
    private const string GetUser = LocalUrl + "/User_GetLoggedUser.php";

    private readonly Dictionary<string, string> _postHeader = new Dictionary<string, string> { { "Content-Type", "text/json" } };

    private Main _main;

    public void Init(Main main)
    {
        _main = main;
    }

    public void HandshakeSession(string gameSessionId)
    {
        StartCoroutine(HandshakeSessionCall(gameSessionId));
    }

    IEnumerator HandshakeSessionCall(string gameSessionId)
    {
        var jsonString = "{ \"gameSessionId\" : \"" + gameSessionId + "\" }";
        byte[] encodedMessage = Encoding.ASCII.GetBytes(jsonString);

        var gameData = new WWW(UnityInitGame, encodedMessage, _postHeader);
        yield return gameData;

        JsonData data = JsonMapper.ToObject(gameData.text);

        if (data.Keys.Contains("Error"))
        {
            Debug.LogError(data["Error"]);
            _main.Game.HandshakeSessionCallback(false);
        }
        else
        {
            _main.Game.HandshakeSessionCallback(true, data["fUniqueId"].ToString());
        }
    }

    public void FetchUser(string fUniqueId)
    {
        StartCoroutine(FetchUserCall(fUniqueId));
    }

    IEnumerator FetchUserCall(string fUniqueId)
    {
        var jsonString = "{ \"fUniqueId\" : \"" + fUniqueId + "\", \"fAppId\" : null }";
        byte[] encodedMessage = Encoding.ASCII.GetBytes(jsonString);

        var userData = new WWW(GetUser, encodedMessage, _postHeader);
        yield return userData;

        JsonData data = JsonMapper.ToObject(userData.text);

        var user = new User
        {
            Id = int.Parse(data["Id"].ToString()),
            Name = data["Name"].ToString(),
            FUniqueId = data["FUniqueId"].ToString(),
            Email = data["Email"] == null ? "" : data["Email"].ToString(),
            FirstName = data["FirstName"].ToString(),
            MiddleName = data["MiddleName"] == null ? "" : data["MiddleName"].ToString(),
            LastName = data["LastName"].ToString(),
            FacebookApps = new List<FacebookApp>()
        };

        user.UserName = user.FirstName + "." + user.LastName;

        user.Password = data["Password"] == null ? "" : data["Password"].ToString();

        for (int i = 0; i < data["Apps"].Count; i++)
        {
            user.FacebookApps.Add(
                new FacebookApp
                {
                    Id = int.Parse(data["Apps"][i]["Id"].ToString()),
                    Name = (string)data["Apps"][i]["Name"],
                    BToken = (string)data["Apps"][i]["BToken"],
                    FacebookId = utils.GetLongDataValue(data["Apps"][i]["FAppId"].ToString()),
                }
                );
        }
        _main.Game.FetchUserCallback(user);
    }
}