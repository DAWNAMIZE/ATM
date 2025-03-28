using System;
using System.Collections.Generic;
using System.Transactions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.TextMeshPro;

public class BankManager : MonoBehaviour
{
    private const string AccountBalanceKeyPrefix = "AccountBalance_User";
    private const string UserNameKeyPrefix = "UserName_User";
    private int currentUserId = 1; // Default to user 1

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
        UpdateBalanceUI();
        UpdateUserProfileUI();
    }

    public void SetCurrentUser(int userId)
    {
        currentUserId = userId;
        LoadUserProfile(); // Load user details when switching user
        LoadBalance();
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
    private List<Transaction> transactionHistory = new List<Transaction>();
    private const string TransactionHistoryKey = "TransactionHistory_"; // Append account key

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

            // Add to recipient's account
            float recipientBalance = PlayerPrefs.GetFloat(AccountBalanceKeyPrefix + recipientId, 0f);
            PlayerPrefs.SetFloat(AccountBalanceKeyPrefix + recipientId, recipientBalance + amount);
            PlayerPrefs.Save();

            Debug.Log($"Transfer of ₦{amount:N2} to User {recipientId} successful.");
        }
        else
        {
            Debug.LogWarning("Insufficient funds for transfer.");
        }
    }
}