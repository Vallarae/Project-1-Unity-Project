using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayButton()
    {
        SceneManager.LoadScene(2);
    }

    public void OptionsButton()
    {
        Debug.Log("Implement later, characters needed for implementation");
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}