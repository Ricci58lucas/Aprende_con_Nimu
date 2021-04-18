using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance = null;

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

    public void LoadScene(string sceneName) => StartCoroutine(TransitionScenes("SlideIn", sceneName));

    public IEnumerator TransitionScenes(string animation, string sceneName)
    {
        if(sceneName.Equals("Game"))
            FindObjectOfType<AudioManager>().TransitionMusic("HappyLoop", "PeaceAtLast");
        else
            FindObjectOfType<AudioManager>().TransitionMusic("PeaceAtLast", "HappyLoop");

        Animator crtAnim = GameObject.Find("Curtain").GetComponent<Animator>();
        crtAnim.SetTrigger(animation);

        yield return new WaitForSeconds(crtAnim.GetCurrentAnimatorStateInfo(0).length);

        SceneManager.LoadScene(sceneName);
    }

    public void LoadDay(int day) => StartCoroutine(TransitionDay(day));

    public IEnumerator TransitionDay(int day)
    {
        Animator crtAnim = GameObject.Find("Curtain").GetComponent<Animator>();

        if (day > 1)
        {
            crtAnim.SetTrigger("SlideIn");

            yield return new WaitForSeconds(crtAnim.GetCurrentAnimatorStateInfo(0).length);
        }

        GameObject.Find("CurtainText").GetComponent<Text>().text = "Día " + day;
        crtAnim.SetTrigger("Fade");

        FindObjectOfType<GameManager>().NewDay();
    }
}
