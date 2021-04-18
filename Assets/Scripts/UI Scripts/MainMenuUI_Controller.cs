using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class MainMenuUI_Controller : MonoBehaviour
{
    public GameObject audioManager;
    public GameObject sceneLoader;
    public GameObject dialogManager;

    [Header("UI GameObjects")]
    public GameObject gameTitle;
    public GameObject mainMenuContainer;
    public GameObject optionsMenuContainer;
    public GameObject creditsContainer;
    public GameObject[] volumeSliders;

    [Header("Timeline")]
    public PlayableDirector timeline;

    void Start()
    {
        Instantiate(audioManager);
        Instantiate(sceneLoader);
        Instantiate(dialogManager);

        GameObject.Find("Curtain").GetComponent<Animator>().SetTrigger("SlideOut");

        FindObjectOfType<AudioManager>().Play("HappyLoop");

        foreach (GameObject volSlider in volumeSliders)
        {
            float sliderValue = FindObjectOfType<AudioManager>().GetVolumeLevel(volSlider.name);
            volSlider.GetComponent<Slider>().value = sliderValue;
        }
    }

    public void StartGame()
    {
        FindObjectOfType<AudioManager>().Play("Button");

        MoveCamera.instance.movingSpeed = 50f;
        for (int i = 1; i <= 4; i++)
            GameObject.Find("parallax" + i).GetComponent<Parallax>().BringIntroBG();

        timeline.Play();
        timeline.stopped += OnTimelineStopped;
    }

    private void OnTimelineStopped(PlayableDirector tl)
    {
        if (timeline != tl) return;

        StartCoroutine(dialogManager.GetComponent<DialogManager>().IntroScene());
    }

    public void OpenOptionsMenu(bool value)
    {
        FindObjectOfType<AudioManager>().Play("Button");

        gameTitle.SetActive(!value);
        mainMenuContainer.SetActive(!value);
        optionsMenuContainer.SetActive(value);
    }

    public void RollCredits(bool value)
    {
        FindObjectOfType<AudioManager>().Play("Button");

        gameTitle.SetActive(!value);
        mainMenuContainer.SetActive(!value);
        creditsContainer.SetActive(value);

        if (!value)
            creditsContainer.GetComponent<Animator>().SetTrigger("SlideOut");
    }

    public void QuitGame()
    {
        FindObjectOfType<AudioManager>().Play("Button");

        // @TODO: chequear si esta mid-game, y hacer que aparezca cartel de confirmacion

        Application.Quit();
    }

    public void SkipIntro() => FindObjectOfType<SceneLoader>().LoadScene("Game");
}
