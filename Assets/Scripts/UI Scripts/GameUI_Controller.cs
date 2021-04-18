using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI_Controller : MonoBehaviour
{
    public GameObject audioManager;
    public GameObject sceneLoader;
    public GameObject dialogManager;

    [Header("Audio Objects")]
    public GameObject[] volumeSliders;

    [Header ("UI Containers")]
    public GameObject answerButtonsContainer;
    public GameObject menuContainer;
    public GameObject infoContainer;
    public GameObject creditsContainer;

    [Header("Situation Objects")]
    public GameObject[] displayPrefabs;
    public Text situationText;
    public Image situationImage;

    [Header("Nimu Objects")]
    public GameObject nimu;
    public Sprite[] nimuPoses;

    [Header("ProgressBar Objects")]
    public Image progressBar;
    public GameObject progressPointer;
    public float progressBarSpeed;
    public Text currentDay;
    public Text nextDay;

    void Start()
    {
        Instantiate(audioManager);
        Instantiate(sceneLoader);
        Instantiate(dialogManager);

        foreach (GameObject volSlider in volumeSliders)
        {
            float sliderValue = FindObjectOfType<AudioManager>().GetVolumeLevel(volSlider.name);
            volSlider.GetComponent<Slider>().value = sliderValue;
        }
    }

    public void SetSituationPanel(string situationToDisplay)
    {
        StartCoroutine(SetBestFontSize(situationToDisplay));
    }

    private IEnumerator SetBestFontSize(string text)
    {
        situationText.text = "";
        situationText.resizeTextForBestFit = true;
        situationText.text = text;

        while (situationText.cachedTextGenerator.fontSizeUsedForBestFit == 0)
            yield return new WaitForEndOfFrame();

        situationText.fontSize = situationText.cachedTextGenerator.fontSizeUsedForBestFit;
        situationText.resizeTextForBestFit = false;
        situationText.text = "";

        StartCoroutine(TypeSituation(situationText, text, 0.03f));
    }

    private IEnumerator TypeSituation(Text textObject, string situation, float delay)
    {
        foreach (char c in situation)
        {
            textObject.text += c;
            //FindObjectOfType<AudioManager>().Play("Type");
            yield return new WaitForSeconds(delay);
        }

        // StartCoroutine(dialogManager.GetComponent<DialogManager>().RandomDialogue());
        SetNimuPose();
    }

    public void SetNimuPose(int spriteNumber = -1)
    {
        int nimuPose = UnityEngine.Random.Range(0, nimuPoses.Length);

        if (spriteNumber != -1 && (spriteNumber >= 0 && spriteNumber < nimuPoses.Length))
            nimuPose = spriteNumber;

        nimu.GetComponent<Image>().sprite = nimuPoses[nimuPose];
    }

    public void SetAnswerPanel(string answerAmmount, string[] answersToDisplay)
    {
        // instancia el display correcto
        foreach (GameObject display in displayPrefabs)
        {
            if(display.name == answerAmmount)
            {
                Instantiate(display, answerButtonsContainer.transform);
                foreach (Animator childComponent in display.GetComponentsInChildren<Animator>())
                    childComponent.SetTrigger("SlideIn");

                break;
            }
        } 

        // asigna los textos a cada boton
        for (int buttonNumber = 1; buttonNumber <= answersToDisplay.Length; buttonNumber++)
        {
            GameObject button = GameObject.Find(answerAmmount + "_" + buttonNumber);
            button.GetComponent<AnswerButtonScript>().buttonText.text = answersToDisplay[buttonNumber - 1];
        }
    }

    public void SetImagePanel(Sprite imageToDisplay)
    {
        if (imageToDisplay == null)
        {
            situationImage.transform.parent.gameObject.SetActive(false);
            return;
        }

        situationImage.transform.parent.gameObject.SetActive(true);
        situationImage.sprite = imageToDisplay;
    }

    public void UpdatePointersText(string startingPoint, string endingPoint)
    {
        currentDay.text = startingPoint;
        nextDay.text = endingPoint;
    }

    public void UpdateProgressBar(int currentLevel, int levelAmmount)
    {
        float progressPercentage = (float)currentLevel / levelAmmount;
        StartCoroutine(AdvanceProgressBar(progressPercentage));
    }

    private IEnumerator AdvanceProgressBar(float newValue)
    {
        float oldValue = progressBar.fillAmount;
        float elapsed = 0f;

        while (elapsed < progressBarSpeed)
        {
            elapsed += Time.deltaTime;

            progressBar.fillAmount = Mathf.Lerp(oldValue, newValue, elapsed / progressBarSpeed);

            RectTransform pointerRect = progressPointer.GetComponent<RectTransform>();
            RectTransform barRect = progressBar.GetComponent<RectTransform>();
            pointerRect.localPosition = new Vector3(0, (progressBar.fillAmount * 100 * barRect.rect.height / 100) - (barRect.rect.height / 2) + (pointerRect.rect.height / 2), 0);

            yield return null;
        }

        progressBar.fillAmount = newValue;

        if (GameEnded()) FindObjectOfType<GameManager>().CheckIfDaysLeft();
    }

    public bool GameEnded()
    {
        if (progressBar.fillAmount == 1f)
            return true;

        return false;
    }

    public void OpenPauseMenu(bool value)
    {
        FindObjectOfType<AudioManager>().Play("Button");

        menuContainer.SetActive(value);
    }

    public void ActivateInfoButton(bool value)
    {
        GameObject infoButton = GameObject.Find("InfoButton");
        infoButton.GetComponent<Animator>().SetBool("Activated", value);
        infoButton.GetComponent<Button>().interactable = value;
    }

    public void OpenInfoMenu(bool value)
    {
        FindObjectOfType<AudioManager>().Play("Info");

        infoContainer.SetActive(value);
        ActivateInfoButton(false);
    }

    public void OpenOptionsMenu(bool value)
    {
        FindObjectOfType<AudioManager>().Play("MenuChange");

        if (value)
            menuContainer.GetComponent<Animator>().SetTrigger("BringOptionsMenu");
        else
            menuContainer.GetComponent<Animator>().SetTrigger("BringPauseMenu");
    }

    public void RollCredits(bool value)
    {
        FindObjectOfType<AudioManager>().Play("Button");

        creditsContainer.SetActive(value);

        if (!value)
            creditsContainer.GetComponent<Animator>().SetTrigger("SlideOut");
    }

    public void ReturnMainMenu()
    {
        FindObjectOfType<AudioManager>().Play("Button");

        // @TODO: pedir confirmacion antes de hacer el cambio de escena
        FindObjectOfType<SceneLoader>().LoadScene("MainMenu");
    }
}
