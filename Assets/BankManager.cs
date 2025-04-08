using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[Serializable]
public class BankTransaction
{
    public string type;
    public float amount;
    public DateTime dateTime;

    public BankTransaction(string type, float amount)
    {
        this.type = type;
        this.amount = amount;
        this.dateTime = DateTime.Now;
    }
}

public class BankManager : MonoBehaviour
{
    private const string AccountBalanceKeyPrefix = "AccountBalance_User";
    private const string UserNameKeyPrefix = "UserName_User";
    private const string TransactionHistoryKeyPrefix = "TransactionHistory_User";
    private int currentUserId = 1; // Default to user 1

    private List<BankTransaction> transactionHistory = new List<BankTransaction>();
    public GameObject transactionEntryPrefab; // Assign your Transaction Entry Prefab in the Inspector
    public GameObject transactionHistoryPagePrefab; // Assign your Transaction History Page Prefab in the Inspector
    public Transform transactionHistoryContent; // Assign the Content area of your ScrollView in the Inspector

    private float GetCurrentBalance()
    {
        return PlayerPrefs.GetFloat(AccountBalanceKeyPrefix + currentUserId, 0f);
    }

    private void SetCurrentBalance(float newBalance)
    {
        PlayerPrefs.SetFloat(AccountBalanceKeyPrefix + currentUserId, newBalance);
        PlayerPrefs.Save();
        UpdateBalanceUI();
    }

    private string GetCurrentUserName()
    {
        return PlayerPrefs.GetString(UserNameKeyPrefix + currentUserId, GetDefaultUserName(currentUserId)); // Provide a default name if not set
    }

    private void SetCurrentUserName(string newName)
    {
        PlayerPrefs.SetString(UserNameKeyPrefix + currentUserId, newName);
        PlayerPrefs.Save();
        UpdateUserProfileUI(); // Update the profile UI when the name changes
    }

    private string GetDefaultUserName(int userId)
    {
        if (userId == 1) return "User One";
        if (userId == 2) return "User Two";
        return "Unknown User";
    }

    public TextMeshProUGUI balanceText;
    public TextMeshProUGUI userNameText; // Reference to the UI Text for displaying the user's name

    void Start()
    {
        LoadUserProfile(); // Load user details at start
        LoadBalance();
        LoadTransactionHistory(); // Load transaction history at start
        UpdateBalanceUI();
        UpdateUserProfileUI();
    }

    public void SetCurrentUser(int userId)
    {
        currentUserId = userId;
        LoadUserProfile(); // Load user details when switching user
        LoadBalance();
        LoadTransactionHistory(); // Load transaction history when switching user
        UpdateBalanceUI();
        UpdateUserProfileUI();
    }

    public void SaveBalance()
    {
        SetCurrentBalance(GetCurrentBalance());
    }

    public float GetBalance()
    {
        return GetCurrentBalance();
    }

    private void LoadBalance()
    {
        UpdateBalanceUI();
    }

    private void UpdateBalanceUI()
    {
        if (balanceText != null)
        {
            balanceText.text = $"₦{GetCurrentBalance():N2}";
        }
        Debug.Log($"User {currentUserId} Balance: {GetCurrentBalance()}");
    }

    private void LoadUserProfile()
    {
        UpdateUserProfileUI();
    }

    private void UpdateUserProfileUI()
    {
        if (userNameText != null)
        {
            userNameText.text = GetCurrentUserName();
        }
        Debug.Log($"User {currentUserId} Name: {GetCurrentUserName()}");
    }

    public TMP_InputField depositInputField;
    public TMP_InputField withdrawInputField;
    public TMP_InputField transferToInputField;
    public TMP_InputField transferAmountInputField;

    public void DepositButtonClicked()
    {
        if (depositInputField != null && float.TryParse(depositInputField.text, out float amount))
        {
            Deposit(amount);
            depositInputField.text = ""; // Clear the input field
        }
        else
        {
            Debug.LogWarning("Invalid deposit amount.");
        }
    }

    private void Deposit(float amount)
    {
        if (amount > 0)
        {
            float currentBalance = GetCurrentBalance();
            currentBalance += amount;
            SetCurrentBalance(currentBalance); // This will save and update the UI
            RecordTransaction("Deposit", amount);
        }
        else
        {
            Debug.LogWarning("Deposit amount must be positive.");
        }
    }

    public void WithdrawButtonClicked()
    {
        if (withdrawInputField != null && float.TryParse(withdrawInputField.text, out float amount))
        {
            Withdraw(amount);
            withdrawInputField.text = ""; // Clear the input field
        }
        else
        {
            Debug.LogWarning("Invalid withdrawal amount.");
        }
    }

    private void Withdraw(float amount)
    {
        if (amount > 0)
        {
            float currentBalance = GetCurrentBalance();
            if (currentBalance >= amount)
            {
                currentBalance -= amount;
                SetCurrentBalance(currentBalance); // Save and update UI
                RecordTransaction("Withdrawal", amount);
            }
            else
            {
                Debug.LogWarning("Insufficient funds.");
            }
        }
        else
        {
            Debug.LogWarning("Withdrawal amount must be positive.");
        }
    }

    public void TransferButtonClicked()
    {
        if (transferToInputField != null && transferAmountInputField != null &&
            int.TryParse(transferToInputField.text, out int recipientId) &&
            float.TryParse(transferAmountInputField.text, out float transferAmount))
        {
            if (recipientId == currentUserId)
            {
                Debug.LogWarning("Cannot transfer to the same account.");
                return;
            }

            if (recipientId != 1 && recipientId != 2) // Assuming only two users
            {
                Debug.LogWarning("Invalid recipient ID. Only users 1 and 2 are supported.");
                return;
            }

            if (transferAmount <= 0)
            {
                Debug.LogWarning("Transfer amount must be positive.");
                return;
            }

            Transfer(recipientId, transferAmount);
            transferToInputField.text = "";
            transferAmountInputField.text = "";
        }
        else
        {
            Debug.LogWarning("Invalid recipient ID or transfer amount.");
        }
    }

    private void Transfer(int recipientId, float amount)
    {
        float senderBalance = GetCurrentBalance();

        if (senderBalance >= amount)
        {
            // Deduct from sender's account
            SetCurrentBalance(senderBalance - amount);
            RecordTransaction($"Transfer to User {recipientId}", amount);

            // Add to recipient's account
            float recipientBalance = PlayerPrefs.GetFloat(AccountBalanceKeyPrefix + recipientId, 0f);
            PlayerPrefs.SetFloat(AccountBalanceKeyPrefix + recipientId, recipientBalance + amount);
            PlayerPrefs.Save();
            RecordTransaction($"Transfer from User {currentUserId}", amount * -1, recipientId); // Record for recipient

            Debug.Log($"Transfer of ₦{amount:N2} to User {recipientId} successful.");
        }
        else
        {
            Debug.LogWarning("Insufficient funds for transfer.");
        }
    }

    private void RecordTransaction(string type, float amount, int? otherUserId = null)
    {
        transactionHistory.Add(new BankTransaction(type, amount));
        SaveTransactionHistory();
    }

    private void SaveTransactionHistory()
    {
        string json = JsonUtility.ToJson(new TransactionListWrapper(transactionHistory));
        PlayerPrefs.SetString(TransactionHistoryKeyPrefix + currentUserId, json);
        PlayerPrefs.Save();
    }

    private void LoadTransactionHistory()
    {
        string json = PlayerPrefs.GetString(TransactionHistoryKeyPrefix + currentUserId, "");
        if (!string.IsNullOrEmpty(json))
        {
            transactionHistory = JsonUtility.FromJson<TransactionListWrapper>(json)?.transactions ?? new List<BankTransaction>();
        }
    }

    [Serializable]
    private class TransactionListWrapper
    {
        public List<BankTransaction> transactions;

        public TransactionListWrapper(List<BankTransaction> transactions)
        {
            this.transactions = transactions;
        }
    }

    public void ShowTransactionHistoryPage()
    {
        if (transactionHistoryPagePrefab != null && transactionEntryPrefab != null && transactionHistoryContent != null)
        {
            // Instantiate the Transaction History Page prefab
            GameObject transactionHistoryPageInstance = Instantiate(transactionHistoryPagePrefab, transform.parent); // Or under your main Canvas

            // Clear existing entries
            foreach (Transform child in transactionHistoryContent)
            {
                Destroy(child.gameObject);
            }

            // Populate the transaction history (reversed to show latest first)
            foreach (BankTransaction transaction in transactionHistory.OrderByDescending(t => t.dateTime))
            {
                GameObject entry = Instantiate(transactionEntryPrefab, transactionHistoryContent);

                // Find the TextMeshPro elements by their names
                TextMeshProUGUI typeText = entry.transform.Find("Panel/TransactionType_Text").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI amountText = entry.transform.Find("Panel/TransactionAmount_Text").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI dateText = entry.transform.Find("Panel/TransactionDate_Text").GetComponent<TextMeshProUGUI>();

                // Set the text for each element
                if (typeText != null)
                {
                    typeText.text = transaction.type;
                }
                if (amountText != null)
                {
                    amountText.text = $"₦{transaction.amount:N2}";
                }
                if (dateText != null)
                {
                    dateText.text = transaction.dateTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }

            // You might want to handle hiding other UI elements here.
        }
        else
        {
            Debug.LogError("Transaction History Page Prefab, Entry Prefab, or Content area not assigned in the Inspector!");
        }
    }

    // You'll need to create a function to go back from the Transaction History page
    // and link it to the "Back to Main Menu" button on that page.
    public void HideTransactionHistoryPage()
    {
        if (transactionHistoryPagePrefab != null)
        {
            // Find and destroy the instantiated page
            GameObject instanceToDestroy = GameObject.FindGameObjectWithTag("TransactionHistoryPage"); // You might want to tag your instantiated page
            if (instanceToDestroy != null)
            {
                Destroy(instanceToDestroy);
                // Also, show your main menu UI again here.
            }
            else
            {
                Debug.LogWarning("Transaction History Page instance not found to destroy.");
            }
        }
    }
}