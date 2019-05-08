using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class Login : MonoBehaviour
{
    #region Fields
    public TextMeshProUGUI titleText;

    public TextMeshProUGUI helpText;

    public TextMeshProUGUI usernameText;
    public TMP_InputField usernameInput;

    public TextMeshProUGUI passwordText;
    public TMP_InputField passwordInput;

    public TextMeshProUGUI validationText;

    public TextMeshProUGUI loginText;
    public TextMeshProUGUI goToRegisterText;

    public GameObject mainMenu;
    public GameObject registration;

    private string key = "AccountData";
    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (usernameInput.isFocused)
                passwordInput.Select();
        }

        if (Input.GetKeyDown(KeyCode.Return))
            UserLogin();

        titleText.text = LocalizationManager.GetTextByKey("LOGIN");
        helpText.text = LocalizationManager.GetTextByKey("HELP_LOGIN");

        usernameText.text = LocalizationManager.GetTextByKey("USERNAME");
        usernameInput.placeholder.GetComponent<TextMeshProUGUI>().text = LocalizationManager.GetTextByKey("USERNAME");
        passwordText.text = LocalizationManager.GetTextByKey("PASSWORD");
        passwordInput.placeholder.GetComponent<TextMeshProUGUI>().text = LocalizationManager.GetTextByKey("PASSWORD");

        loginText.text = LocalizationManager.GetTextByKey("LOGIN");
        goToRegisterText.text = LocalizationManager.GetTextByKey("DONT_HAVE_ACCOUNT");
    }

    
    public void UserLogin()
    {
        if (usernameInput.text != "" && passwordInput.text != "")
        {
            Account account = GetAccountByUsername();
            // if account exists
            if (account != null)
            {
                if (account.password == passwordInput.text)
                {
                    // Save/set currentAccount
                    string json = JsonUtility.ToJson(account);
                    PlayerPrefs.SetString("currentAccount", json);

                    // Go to main menu
                    mainMenu.SetActive(true);
                    gameObject.SetActive(false);
                }
                else
                    validationText.text = LocalizationManager.GetTextByKey("WRONG_PASSWORD"); ;
            }
            else
                validationText.text = LocalizationManager.GetTextByKey("ACCOUNT_NOT_EXIST"); ;
        }
        else
            validationText.text = LocalizationManager.GetTextByKey("FIELDS_EMPTY");
    }

    [ContextMenu("Logout")]
    public void UserLogoutDebug()
    {
        PlayerPrefs.DeleteKey("currentAccount");
    }

    public void UserLogout()
    {
        PlayerPrefs.DeleteKey("currentAccount");
        gameObject.SetActive(true);
        mainMenu.SetActive(false);
    }

    public Account GetAccountByUsername()
    {
        // Load
        string jsonString = PlayerPrefs.GetString(key);
        AccountData accountData = JsonUtility.FromJson<AccountData>(jsonString);

        return accountData.accounts.FirstOrDefault(a => a.username == usernameInput.text);
    }

    public void GoToRegistration()
    {
        registration.SetActive(true);
        gameObject.SetActive(false);
    }
}

