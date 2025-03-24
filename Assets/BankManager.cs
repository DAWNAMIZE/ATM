using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.TextMeshPro;

public class BankManager : MonoBehaviour
{
    private const string AccountBalanceKeyPrefix = "AccountBalance_User";
    private int currentUserId = 1; // Default to user 1, you'll need to set this based on your user selection logic

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

    public TextMeshProUGUI balanceText;

    void Start()
    {
        LoadBalance();
        UpdateBalanceUI();
    }

    public void SetCurrentUser(int userId)
    {
        currentUserId = userId;
        LoadBalance();
        UpdateBalanceUI();
    }

    public void SaveBalance() // Modified to use current user
    {
        SetCurrentBalance(GetCurrentBalance());
    }

    public float GetBalance() // Modified to use current user
    {
        return GetCurrentBalance();
    }

    private void LoadBalance() // Modified to use current user
    {
        UpdateBalanceUI();
    }

    private void UpdateBalanceUI()
    {
        if (balanceText != null)
        {
            balanceText.text = $"₦{GetCurrentBalance():N2}"; // Format as currency
        }
        Debug.Log($"User {currentUserId} Balance: {GetCurrentBalance()}");
    }

    public TMP_InputField depositInputField; // Link this in the Inspector
    public TMP_InputField withdrawInputField; // Link this in the Inspector
    public TMP_InputField transferToInputField; // Link this in the Inspector for the recipient user ID
    public TMP_InputField transferAmountInputField; // Link this in the Inspector for the transfer amount

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