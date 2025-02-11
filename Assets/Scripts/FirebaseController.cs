using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth; // Import Firebase Authentication

public class FirebaseController : MonoBehaviour
{
    public GameObject loginPanel, signupPanel, profilePanel;

    public InputField loginEmail, loginPassword, signupEmail, signupPassword, signupConfirmPassword, signupUserName;

    private FirebaseAuth auth; // Firebase Auth instance

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance; // Initialize Firebase Auth
    }

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
        if (string.IsNullOrEmpty(loginEmail.text) || string.IsNullOrEmpty(loginPassword.text)) // Use || (OR)
        {
            Debug.LogError("Email and password are required."); // More informative error
            return;
        }

        string email = loginEmail.text;
        string password = loginPassword.text;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCompleted)
            {
                Debug.Log("User logged in successfully!");
                // Open Profile Panel or change scenes
                OpenProfilePanel(); // Example: Open profile panel
            }
            else
            {
                Debug.LogError("Error logging in: " + task.Exception.Message);
                // Display error message to the user (e.g., in a UI text element)
            }
        });
    }

    public void SignUpUser()
    {
        if (string.IsNullOrEmpty(signupEmail.text) || string.IsNullOrEmpty(signupPassword.text) || string.IsNullOrEmpty(signupConfirmPassword.text) || string.IsNullOrEmpty(signupUserName.text)) // Use || (OR)
        {
            Debug.LogError("All fields are required."); // More informative error
            return;
        }

        if (signupPassword.text != signupConfirmPassword.text)
        {
            Debug.LogError("Passwords do not match.");
            return;
        }

        string email = signupEmail.text;
        string password = signupPassword.text;
        string username = signupUserName.text; // You can store the username in your database later

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCompleted)
            {
                Debug.Log("User signed up successfully!");
                // You might want to sign in the user automatically after signup:
                // LoginUser(); // Call LoginUser() to log the user in

                // Open Profile Panel or change scenes
                OpenProfilePanel(); // Example: Open profile panel
            }
            else
            {
                Debug.LogError("Error signing up: " + task.Exception.Message);
                // Display error message to the user
            }
        });
    }
}