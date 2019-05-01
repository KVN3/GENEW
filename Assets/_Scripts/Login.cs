using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class Login : MonoBehaviour
{
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI passwordText;
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI validationText;


    public GameObject mainMenu;
    public GameObject registration;

    private string key = "AccountData";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (usernameInput.isFocused)
                passwordInput.Select();
        }

        if (Input.GetKeyDown(KeyCode.Return))
            UserLogin();

        usernameText.text = LocalizationManager.GetTextByKey("USERNAME");
        passwordText.text = LocalizationManager.GetTextByKey("PASSWORD");
    }

    public void UserLogin()
    {
        if (usernameInput.text != "" && passwordInput.text != "")
        {
            // if account exists
            Account account = GetAccountByUsername();
            if (account != null)
            {
                if (account.password == passwordInput.text)
                {
                    PlayerPrefs.SetString("currentAccount", account.username);

                    mainMenu.SetActive(true);
                    gameObject.SetActive(false);
                }
                else
                    validationText.text = "Wrong password!";
            }
            else
                validationText.text = "Account does not exist!";
        }
        else
            validationText.text = "Fields are empty!";
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

