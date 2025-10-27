using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject questions;

    // Show the UI
    public void ShowQuestions()
    {
        questions.SetActive(true);
    }

    // Hide the UI
    public void HideQuestions()
    {
        questions.SetActive(false);
    }

    // Toggle the UI on/off
    public void ToggleQuestions()
    {
        questions.SetActive(!questions.activeSelf);
    }
}