using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FirebaseController : MonoBehaviour
{
    public GameObject loginPanel, signupPanel, profilePanel;
    public InputField loginEmail, loginPassword, signupEmail, signupPassword, signupConfirmPassword;
    private object signupUserName;

    public void OpenLoginPanel()
    {
        loginPanel.SetActive(true);
        signupPanel.SetActive(false);
        profilePanel.SetActive(false);
    }

    public void OpenSignUpPanel()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(true);
        profilePanel.SetActive(false);
    }

    public void OpenProfilePanel()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        profilePanel.SetActive(true);
    }

    public void LoginUser()
    {
        if(string.IsNullOrEmpty(loginEmail.text)&&string.IsNullOrEmpty(loginPassword.text))
        {
            return;
        }
        //Do Login
    }
    public void SignUpUser()
    {
        if (string.IsNullOrEmpty(signupEmail.text) && string.IsNullOrEmpty(signupPassword.text) && string.IsNullOrEmpty(signupPassword.text))
        {
            return;
        }
        //Do LoginUp
    }

    // Example of a basic signup function (you'll need to add Firebase logic here)
    public void SignUp()
    {
        if (signupPassword.text != signupConfirmPassword.text)
        {
            Debug.LogError("Passwords do not match!");
            return;
        }

        // Basic input validation (add more as needed)
        if (string.IsNullOrEmpty(signupEmail.text) || string.IsNullOrEmpty(signupPassword.text))
        {
            Debug.LogError("Email and password are required!");
            return;
        }

        // TODO: Integrate Firebase Signup here
        string email = signupEmail.text;
        string password = signupPassword.text;

        // Example placeholder:
        Debug.Log("Signing up with: " + email + " " + password);

        // After successful signup, you might want to:
        // OpenProfilePanel(); 
    }

    // Example of a basic login function (you'll need to add Firebase logic here)
    public void Login()
    {
        // Basic input validation (add more as needed)
        if (string.IsNullOrEmpty(loginEmail.text) || string.IsNullOrEmpty(loginPassword.text))
        {
            Debug.LogError("Email and password are required!");
            return;
        }

        // TODO: Integrate Firebase Login here
        string email = loginEmail.text;
        string password = loginPassword.text;

        // Example placeholder:
        Debug.Log("Logging in with: " + email + " " + password);

        // After successful login, you might want to:
        // OpenProfilePanel();
    }
}