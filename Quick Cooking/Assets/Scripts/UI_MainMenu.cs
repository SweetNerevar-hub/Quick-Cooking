using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class stores the functionality associated with the main menu interactions.
/// </summary>
public class UI_MainMenu : MonoBehaviour
{
    /// <summary>
    /// Loads the first game scene.
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
