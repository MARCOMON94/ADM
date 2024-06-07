using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    // Botones del menú
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;
    public Slider volumeSlider;
    public Button backButton;

    void Start()
    {
        /* 
        Al inicio, oculta los elementos del control de volumen y el botón 
        de regresar.
        */
        volumeSlider.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
    }

    public void LoadIntroScene()
    {
        /* 
        Carga la escena de introducción.
        */
        SceneManager.LoadScene("Intro");
    }

    public void LoadBattleScene()
    {
        /* 
        Carga la escena de batalla.
        */
        SceneManager.LoadScene("Battle");
    }

    public void ShowSettings()
    {
        /* 
        Oculta los botones del menú principal y muestra los elementos 
        de configuración.
        */
        button1.gameObject.SetActive(false);
        button2.gameObject.SetActive(false);
        button3.gameObject.SetActive(false);
        button4.gameObject.SetActive(false);

        volumeSlider.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
    }

    public void HideSettings()
    {
        /* 
        Oculta los elementos de configuración y muestra los botones del 
        menú principal.
        */
        volumeSlider.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);

        button1.gameObject.SetActive(true);
        button2.gameObject.SetActive(true);
        button3.gameObject.SetActive(true);
        button4.gameObject.SetActive(true);
    }

    public void QuitGame()
    {
        /* 
        Sale del juego.
        */
        Application.Quit();
    }

    public void SetVolume(float volume)
    {
        /* 
        Ajusta el volumen del juego.
        */
        AudioListener.volume = volume;
    }
}
