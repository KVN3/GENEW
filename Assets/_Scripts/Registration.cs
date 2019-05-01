using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using TMPro;
using System.Linq;

public class Registration : MonoBehaviour
{
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI passwordText;
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI validationText;

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

        usernameText.text = LocalizationManager.GetTextByKey("USERNAME");
        passwordText.text = LocalizationManager.GetTextByKey("PASSWORD");
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
        if (usernameInput.text != "" && passwordInput.text != "")
        {
            if (passwordInput.text.Length > passwordLength)
            {
                CreateAccount();
            }
            else
                validationText.text = $"Password has to be longer than {passwordLength} characters!";
        }
        else
            validationText.text = "Fields are empty!";

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
            validationText.text = "Account already exists!";
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
