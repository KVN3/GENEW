using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using TMPro;
using UnityEngine.UI;

// FB needed functionality:
// Share time
// Share achievements
// Share ship
//

// Test access token: 
// EAAQJNNEFLL0BALCZAulkDQWsVZAVHZBWIHwhFdV19iqhASdbeAsZAb1XBLQQnyHcYMzEJR2tTmyCjebHH0OTA6t8ToMhvAZBCFiiiPswUc8RT9KWhqQGavi1KTNYtRjxQHJZBBw56akQswS5ADwllbIAFNACLzTBbPeQ6eZCAlXkv2m5nnr0dZCGDZCrVQIrShfdwdA7jaLt6NgZDZD

public class FBScript : MonoBehaviour
{

    public GameObject dialogLoggedIn;
    public GameObject dialogLoggedOut;

    public TextMeshProUGUI dialogUserNameText;
    public Image dialogProfilePic;

    public TextMeshProUGUI friendsText;

    private void Awake()
    {
        FB.Init(SetInit, OnHideUnity);
    }

    void SetInit()
    {
        if (FB.IsLoggedIn)
            Debug.Log("FB is logged in!");
        else
        {
            Debug.Log("FB is not logged in!");
            //FBLogin();
        }

        DealWithFBMenus(FB.IsLoggedIn);
    }

    void OnHideUnity(bool isGameShown)
    {

        if (!isGameShown)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    #region Login/logout
    public void FBLogin()
    {
        List<string> permissions = new List<string>();
        permissions.Add("public_profile");
        permissions.Add("user_friends");

        FB.LogInWithReadPermissions(permissions, AuthCallBack);
    }

    public void FBLogout()
    {
        FB.LogOut();
    }
    #endregion

    void AuthCallBack(IResult result)
    {
        if (result.Error != null)
            Debug.Log(result.Error);
        else
        {
            if (FB.IsLoggedIn)
                Debug.Log("FB is logged in!");
            else
                Debug.Log("FB login fail!");
        }

        DealWithFBMenus(FB.IsLoggedIn);
    }

    void DealWithFBMenus(bool isLoggedIn)
    {
        if (isLoggedIn)
        {
            dialogLoggedIn.SetActive(true);
            dialogLoggedOut.SetActive(false);

            FB.API("/me?fields=first_name", HttpMethod.GET, DisplayUsername);
            FB.API("me/picture?type=square&height=128&width=128", HttpMethod.GET, DisplayProfilePic);
        }
        else
        {
            dialogLoggedIn.SetActive(false);
            // TO DO: Null error
            //dialogLoggedOut.SetActive(true);
        }
    }

    void DisplayUsername(IResult result)
    {
        if (result.Error == null)
        {
            dialogUserNameText.text = result.ResultDictionary["first_name"].ToString();
        }
        else
        {
            Debug.Log(result.Error);
        }
    }

    void DisplayProfilePic(IGraphResult result)
    {
        if (result.Texture != null)
        {
            dialogProfilePic.sprite = Sprite.Create(result.Texture, new Rect(0,0,128,128), new Vector2());
        }
    }

    #region Sharing

    public void FBShare()
    {
        FB.ShareLink(new System.Uri("http://google.com"), "My new ship", "Check out my new ship!", new System.Uri("https://assets.dryicons.com/uploads/icon/svg/5253/f50a98cb-fc7f-4407-83e7-59a6768ed814.svg"));
    }

    public void ShareWithFriends()
    {
        FB.FeedShare(
            link: new System.Uri("http://google.com"),
            linkName: $"Hey guys, check out my time! I got time on stage!",
            picture: new System.Uri("https://cdnb.artstation.com/p/assets/images/images/008/748/049/original/liz-reddington-spaceship01-gif.gif?1515046169")
            );
    }

    #endregion

    #region Inviting

    public void FBGameRequest()
    {
        FB.AppRequest(
            "Hey! come and play this awesome game!", 
            title: "Space Race Game"
            );
    }

    #endregion

    #region Friends
    public void GetFriendsPlayingThisGame()
    {
        string query = "/me/friends";
        FB.API(query, HttpMethod.GET, result =>
        {
            var dictionary = (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(result.RawResult);
            var friendsList = (List<object>)dictionary["data"];
            friendsText.text = string.Empty;
            foreach (var dict in friendsList)
                friendsText.text += ((Dictionary<string, object>)dict)["name"];
        });
    }
    #endregion
}
