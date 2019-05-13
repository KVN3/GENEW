using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

// Also accountManager

public class Registration : MonoBehaviour
{
    #region Fields
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
    public GameObject mainMenu;

    private int passwordLength = 4;

    private readonly string key = "AccountData";
    #endregion

    // Start is called before the first frame update
    void Awake()
    {   
        // Load accounts
        if (!PlayerPrefs.HasKey(key))
        {
            InitAccounts();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If logged in redirect to main
        if (GetCurrentAccount() != null)
            GoToMainMenu();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (usernameInput.isFocused)
                passwordInput.Select();
            if (passwordInput.isFocused)
                confirmPasswordInput.Select();
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

    [ContextMenu("Delete Accounts")]
    public void InitAccounts()
    {
        // Create
        AccountData accountData = new AccountData { accounts = new List<Account>() };
        string json = JsonUtility.ToJson(accountData);
        PlayerPrefs.SetString(key, json);
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
            Account account = new Account(accountData.accounts.Count, usernameInput.text, passwordInput.text);
            account.achievements = AchievementManager.Instance.CreateAchievementListForPlayer(account);
            account.friendList = new List<string>();

            // Add
            accountData.accounts.Add(account);

            // Update
            string json = JsonUtility.ToJson(accountData);
            PlayerPrefs.SetString(key, json);

            // Go to login
            login.SetActive(true);
            gameObject.SetActive(false);
        }
        else
            validationText.text = LocalizationManager.GetTextByKey("ACCOUNT_EXISTS");
    }

    public static Account GetCurrentAccount()
    {
        // Load current account
        string jsonString = PlayerPrefs.GetString("currentAccount");
        return JsonUtility.FromJson<Account>(jsonString);
    }
    
    // Updates logged in account
    public static void SaveCurrentAccount(Account account)
    {
        string json = JsonUtility.ToJson(account);
        PlayerPrefs.SetString("currentAccount", json);
    }

    // Saves to the local accountData
    public static void SaveAccountToAccountData(Account account)
    {
        // Load accountData
        string jsonString = PlayerPrefs.GetString("AccountData");
        AccountData accountData = JsonUtility.FromJson<AccountData>(jsonString);
        
        accountData.accounts[account.id] = account;

        // Save account to accountData
        string json = JsonUtility.ToJson(accountData);
        PlayerPrefs.SetString("AccountData", json);
    }

    public void GoToLogin()
    {
        login.SetActive(true);
        gameObject.SetActive(false);
    }

    public void GoToMainMenu()
    {
        mainMenu.SetActive(true);
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
    public int id;
    public string username;
    public string password;

    public List<Achievement> achievements;
    public List<string> friendList;

    public Account(int id, string username, string password)
    {
        this.id = id;
        this.username = username;
        this.password = password;
    }
}
