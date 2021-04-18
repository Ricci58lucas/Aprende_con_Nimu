using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class AnswerButtonScript : MonoBehaviour
{
    public Text buttonText;

    void Start() => gameObject.GetComponent<Button>().onClick.AddListener(CheckAnswer);
    
    private void CheckAnswer()
    {
        string buttonNumber = gameObject.name.Substring(gameObject.name.Length - 1);
        FindObjectOfType<GameManager>().CheckAnswer(int.Parse(buttonNumber));
    }

    public void DestroyParent(string lastChild)
    {
        // encarga al ultimo boton de actualizar el display
        if (lastChild != gameObject.name) return;

        GameObject parent = gameObject.transform.parent.gameObject;
        Destroy(parent);
        parent.SetActive(false);

        FindObjectOfType<GameManager>().UpdateDisplay();
    }
}
