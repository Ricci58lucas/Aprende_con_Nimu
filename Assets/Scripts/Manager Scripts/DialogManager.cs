using System.Collections;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance = null;

    public TextAsset introDialogue;
    public TextAsset randomDialogues;
    public TextAsset outroDialogue;

    private int dialogIndex;
    private int previousDialog;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void NextDialogue() => dialogIndex++;

    public IEnumerator IntroScene()
    {
        string[] introSpeech = introDialogue.text.Split('&');

        dialogIndex = 0;
        previousDialog = -1;

        while (dialogIndex < introSpeech.Length)
        {
            if (previousDialog != dialogIndex)
            {
                previousDialog = dialogIndex;
                FindObjectOfType<SpeechBubble>().TypeText(introSpeech[dialogIndex], true);
            }

            yield return new WaitForFixedUpdate();
        }

        FindObjectOfType<SceneLoader>().LoadScene("Game");
    }

    public IEnumerator RandomDialogue()
    {
        string[] rndDialogueList = randomDialogues.text.Split('&');

        int rndDialog = UnityEngine.Random.Range(0, rndDialogueList.Length);
        FindObjectOfType<SpeechBubble>().GetComponent<SpeechBubble>().TypeText(rndDialogueList[rndDialog], false);

        yield return new WaitForSeconds(5f);

        FindObjectOfType<SpeechBubble>().GetComponent<Animator>().SetTrigger("Close");
    }

    public IEnumerator OutroScene()
    {
        string[] outroSpeech = outroDialogue.text.Split('&');

        dialogIndex = 0;
        previousDialog = -1;

        while (dialogIndex < outroSpeech.Length)
        {
            if (previousDialog != dialogIndex)
            {
                previousDialog = dialogIndex;
                FindObjectOfType<SpeechBubble>().TypeText(outroSpeech[dialogIndex], true);
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
