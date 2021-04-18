using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour
{
    public Text bubbleText;
    public GameObject nextButton;
    private bool isOpen = false;

    public void TypeText(string text, bool showNextButton)
    {
        nextButton.SetActive(showNextButton);
        
        if (!isOpen)
            GetComponent<Animator>().SetTrigger("Open");

        //StartCoroutine(SetBestFontSize(text));
        bubbleText.text = "";
        StartCoroutine(Type(bubbleText, text, 0.04f));
    }

    private IEnumerator SetBestFontSize(string text)
    {
        bubbleText.text = "";
        bubbleText.resizeTextForBestFit = true;
        bubbleText.text = text;

        while (bubbleText.cachedTextGenerator.fontSizeUsedForBestFit == 0)
            yield return new WaitForEndOfFrame();

        bubbleText.fontSize = bubbleText.cachedTextGenerator.fontSizeUsedForBestFit;
        bubbleText.resizeTextForBestFit = false;
        bubbleText.text = "";

        StartCoroutine(Type(bubbleText, text, 0.04f));
    }

    private IEnumerator Type(Text textObject, string textToType, float delay)
    {
        foreach (char c in textToType)
        {
            textObject.text += c;
            yield return new WaitForSeconds(delay);
        }
    }

    public void IsOpen(int value) => isOpen = value == 1;
}
