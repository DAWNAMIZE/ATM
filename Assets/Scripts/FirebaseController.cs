using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FirebaseController : MonoBehaviour
{
    public GameObject loginPanel, signupPanel, profilePanel, BankTransfer;

    public InputField loginEmail, loginPassword, signupEmail, signupPassword, signupConfirmPassword, signupUserName;
    

    public void OpenLoginPanel()
    {
        Debug.Log("LoginPanel");
        loginPanel.SetActive(true);
        signupPanel.SetActive(false);
        profilePanel.SetActive(false);
        BankTransfer.SetActive(false);
    }

    public void OpenBankTransfer()
    {
        Debug.Log("BankTransfer");
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        profilePanel.SetActive(false);
        BankTransfer.SetActive(true);
    }

    public void OpenSignUpPanel()
    {
        Debug.Log("SignUp");
        loginPanel.SetActive(false);
        signupPanel.SetActive(true);
        profilePanel.SetActive(false);
        BankTransfer.SetActive(false);
    }

    public void OpenProfilePanel()
    {
        Debug.Log("profileButton");
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        profilePanel.SetActive(true);
        BankTransfer.SetActive(false);
    }

    public void LoginUser()
    {
        if(string.IsNullOrEmpty(loginEmail.text)&&string.IsNullOrEmpty(loginPassword.text))
        {
            return;
        }
        if (string.IsNullOrEmpty(loginEmail.text) || string.IsNullOrEmpty(loginPassword.text))
        {
            Debug.LogError("Email and password are required.");
            return;
        }
        //Do Login
    }
    public void SignUpUser()
    {
        if (string.IsNullOrEmpty(signupEmail.text) && string.IsNullOrEmpty(signupPassword.text) && string.IsNullOrEmpty(signupConfirmPassword.text) && string.IsNullOrEmpty(signupUserName.text))
        {
            return;
        }
        //Do SignUp
    }
}