using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using TMPro;
using System.Linq;

public class Registration : MonoBehaviour
{
    public TextMeshProUGUI titleText;

    public TextMeshProUGUI helpText;

    public TextMeshProUGUI usernameText;
    public TMP_InputField usernameInput;

    public TextMeshProUGUI passwordText;  
    public TMP_InputField passwordInput;

    public TextMeshProUGUI confirmPasswordText;
    public TMP_InputField confirmPasswordInput;

    public TextMeshProUGUI validationText;

    public TextMeshProUGUI registerText;
    public TextMeshProUGUI goToLoginText;


    public GameObject login;

    private int passwordLength = 4;

    private string key = "AccountData";

    // Start is called before the first frame update
    void Awake()
    {
        // Load accounts
        if (!PlayerPrefs.HasKey(key))
        {
            InitAccounts();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (usernameInput.isFocused)
                passwordInput.Select();
        }

        if (Input.GetKeyDown(KeyCode.Return))
            Register();

        titleText.text = LocalizationManager.GetTextByKey("REGISTRATION");

        helpText.text = LocalizationManager.GetTextByKey("HELP_REGISTRATION");

        usernameText.text = LocalizationManager.GetTextByKey("USERNAME");
        usernameInput.placeholder.GetComponent<TextMeshProUGUI>().text = LocalizationManager.GetTextByKey("USERNAME");
        passwordText.text = LocalizationManager.GetTextByKey("PASSWORD");
        passwordInput.placeholder.GetComponent<TextMeshProUGUI>().text = LocalizationManager.GetTextByKey("PASSWORD");
        confirmPasswordText.text = LocalizationManager.GetTextByKey("CONFIRM_PASSWORD");
        confirmPasswordInput.placeholder.GetComponent<TextMeshProUGUI>().text = LocalizationManager.GetTextByKey("CONFIRM_PASSWORD");

        registerText.text = LocalizationManager.GetTextByKey("REGISTER");
        goToLoginText.text = LocalizationManager.GetTextByKey("HAVE_ACCOUNT");
    }

    public void InitAccounts()
    {
        // Create
        AccountData accountData = new AccountData { accounts = new List<Account>() };
        string json = JsonUtility.ToJson(accountData);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }

    public void Register()
    {
        if (usernameInput.text != "" && passwordInput.text != "" && confirmPasswordInput.text != "")
        {
            if (passwordInput.text.Length > passwordLength)
            {
                if (passwordInput.text == confirmPasswordInput.text)
                    CreateAccount();
                else
                    validationText.text = LocalizationManager.GetTextByKey("PASSWORDS_NOT_MATCH");
            }
            else
                validationText.text = LocalizationManager.GetTextByKey("PASSWORD_LONGER_1") + $" {passwordLength} " + LocalizationManager.GetTextByKey("PASSWORD_LONGER_2") + "!";
        }
        else
            validationText.text = LocalizationManager.GetTextByKey("FIELDS_EMPTY");

    }

    public void CreateAccount()
    {
        // Load
        string jsonString = PlayerPrefs.GetString(key);
        AccountData accountData = JsonUtility.FromJson<AccountData>(jsonString);

        // Create new accountData if null
        if (accountData.accounts == null)
            accountData = new AccountData { accounts = new List<Account>() };

        // Only create if account doesn't exist
        if (!accountData.accounts.Any(a => a.username == usernameInput.text))
        {
            // Create
            Account account = new Account(usernameInput.text, passwordInput.text);

            // Add
            accountData.accounts.Add(account);

            // Update
            string json = JsonUtility.ToJson(accountData);
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();

            // Go to login
            login.SetActive(true);
            gameObject.SetActive(false);
        }
        else
            validationText.text = LocalizationManager.GetTextByKey("ACCOUNT_EXISTS");
    }

    public void GoToLogin()
    {
        login.SetActive(true);
        gameObject.SetActive(false);
    }
}

public class AccountData
{
    public List<Account> accounts;
}

[System.Serializable]
public class Account
{
    public string username;
    public string password;

    public Account(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}
