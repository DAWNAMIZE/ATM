using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI; 
using static TMPro.TextMeshPro;

public class BankManager : MonoBehaviour
{
    private const string AccountBalanceKey = "237298379";
    private float currentBalance;

    public TextMeshProUGUI balanceText;

    void Start()
    {
        LoadBalance();
        UpdateBalanceUI();
    }

    public void SaveBalance(float newBalance)
    {
        currentBalance = newBalance;
        PlayerPrefs.SetFloat(AccountBalanceKey, currentBalance);
        PlayerPrefs.Save();
        UpdateBalanceUI();
    }

    public float GetBalance()
    {
        return currentBalance;
    }

    private void LoadBalance()
    {
        currentBalance = PlayerPrefs.GetFloat(AccountBalanceKey, 0f);
        UpdateBalanceUI();
    }

    private void UpdateBalanceUI()
    {
        if (balanceText != null)
        {
            balanceText.text = $"₦{currentBalance:N2}"; // Format as currency
        }
        Debug.Log(currentBalance);
    }

    public TMP_InputField depositInputField; // Link this in the Inspector
    public TMP_InputField withdrawInputField; // Link this in the Inspector

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
            currentBalance += amount; // Update the class-level variable
            SaveBalance(currentBalance); // This will save and update the UI
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
            if (currentBalance >= amount)
            {
                currentBalance -= amount;
                SaveBalance(currentBalance); // Save and update UI
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
}