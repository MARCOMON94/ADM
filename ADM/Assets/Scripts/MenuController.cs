using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;
    public Slider volumeSlider;
    public Button backButton;

    void Start()
    {
        volumeSlider.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
    }

    public void LoadIntroScene()
    {
        SceneManager.LoadScene("Intro");
    }

    public void LoadBattleScene()
    {
        SceneManager.LoadScene("Battle");
    }

    public void ShowSettings()
    {
        // Hide main menu buttons
        button1.gameObject.SetActive(false);
        button2.gameObject.SetActive(false);
        button3.gameObject.SetActive(false);
        button4.gameObject.SetActive(false);

        // Show settings elements
        volumeSlider.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
    }

    public void HideSettings()
    {
        // Hide settings elements
        volumeSlider.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);

        // Show main menu buttons
        button1.gameObject.SetActive(true);
        button2.gameObject.SetActive(true);
        button3.gameObject.SetActive(true);
        button4.gameObject.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}
