using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject GameUIObject;
    private GameUI_Controller GameUI;

    public TextAsset[] levelFiles;
    public Sprite[] levelImages;
    public TextAsset femaleNamesFile;
    public TextAsset maleNamesFile;
    private List<Level> levelList;
    private Level currentSituation;

    private int dailyLvlIndex;
    private int dayNumber;
    private int totalLvlAmmount;
    private int lvlAmmountPerDay;

    private List<int> sortedDayLevels;
    private List<int> usedLevels;

    private string[] femaleNames;
    private string[] maleNames;
    private List<string> sortedNames;

    void Start()
    {
        GameUI = GameUIObject.GetComponent<GameUI_Controller>();

        totalLvlAmmount = levelFiles.Length;

        sortedDayLevels = new List<int>();
        usedLevels = new List<int>();

        femaleNames = femaleNamesFile.text.Split('\n');
        maleNames = maleNamesFile.text.Split('\n');

        SetUpNameOrder();
        LoadLevelConfigs();

        dayNumber = 0;
        CheckIfDaysLeft();
    }

    #region Game Mechanics

    public void CheckIfDaysLeft()
    {
        if (NoLevelsLeft(totalLvlAmmount - usedLevels.Count))
            FindObjectOfType<SceneLoader>().LoadScene("EndGame");
        else
            FindObjectOfType<SceneLoader>().LoadDay(++dayNumber);
    }

    public void NewDay()
    {
        SetUpLevelOrder();

        dailyLvlIndex = -1;
        UpdateDisplay();
    }

    /// <summary>Actualiza la interfaz y todos sus componentes (panel de situacion, respuestas, barra de progreso, etc)</summary>
    public void UpdateDisplay()
    {
        dailyLvlIndex++;
        GameUI.UpdateProgressBar(dailyLvlIndex, lvlAmmountPerDay);

        if (dailyLvlIndex >= lvlAmmountPerDay) return;

        currentSituation = levelList[sortedDayLevels[dailyLvlIndex]];
        GameUI.SetSituationPanel(currentSituation.situation);
        GameUI.SetImagePanel(currentSituation.image);
        GameUI.SetAnswerPanel(currentSituation.answerAmmount.ToString(), currentSituation.answers.ToArray());

        string endPointer = (dayNumber + 1).ToString();
        if (NoLevelsLeft(totalLvlAmmount - usedLevels.Count))
            endPointer = "Fin";

        GameUI.UpdatePointersText(dayNumber.ToString(), endPointer);
        GameUI.ActivateInfoButton(false);
    }

    public void CheckAnswer(int answer)
    {
        foreach (string correctAnswer in currentSituation.correctAnswer)
        {
            if (answer == int.Parse(correctAnswer))
            {
                CorrectAnswer();
                return;
            }
        }

        WrongAnswer();
    }

    private void CorrectAnswer()
    {
        FindObjectOfType<AudioManager>().Play("Correct");

        GameObject answerPanel = GameObject.FindGameObjectWithTag("AnswerPanel");
        Animator[] childAnim = answerPanel.GetComponentsInChildren<Animator>();

        // evita que se pueda interactuar con los botones durante su salida
        foreach (Animator child in childAnim)
            child.GetComponent<Button>().enabled = false;

        StartCoroutine(DelayAnimation(childAnim, 0.3f));
    }

    private IEnumerator DelayAnimation(Animator[] childAnimators, float delay)
    {
        foreach (Animator anim in childAnimators)
        {
            anim.SetTrigger("SlideOut");
            yield return new WaitForSeconds(delay);
        }
    }

    private void WrongAnswer()
    {
        FindObjectOfType<AudioManager>().Play("Answers");

        GameUI.ActivateInfoButton(true);
    }

    #endregion

    #region Game Setup

    /// <summary>Genera una lista de nomber random para poner en la situacion</summary>
    private void SetUpNameOrder()
    {
        sortedNames = new List<string>();

        // genera una lista de nombres ordenadas de forma random (pares m, impares f)
        for (int index = 0; index < (femaleNames.Length + maleNames.Length); index++)
        {
            // indica si se va a elegir una nombr f o m
            string[] namesList = (index % 2 == 0) ? maleNames : femaleNames;

            int rndNameIndex = UnityEngine.Random.Range(0, namesList.Length);

            // comprueba que no sea un nombre usado
            if (sortedNames.Contains(namesList[rndNameIndex]))
            {
                index--;
                continue;
            }

            sortedNames.Add(namesList[rndNameIndex]);
        }
    }

    /// <summary>Carga la información (llamada configuración) de cada situación</summary>
    private void LoadLevelConfigs()
    {
        levelList = new List<Level>();

        for (int fileIndex = 0; fileIndex < levelFiles.Length; fileIndex++)
        {
            if (levelFiles[fileIndex] == null) continue;

            string[] configLines = levelFiles[fileIndex].text.Split('\n');

            Level level = new Level
            {
                answerAmmount = int.Parse(ProcessValidConfig(configLines[0])),
                // correctAnswer = int.Parse(ProcessValidConfig(configLines[1])),
                lead = ProcessValidConfig(configLines[2]),
                situation = string.Format(ProcessValidConfig(configLines[3]), sortedNames[fileIndex]),
                answers = new List<string>()
            };

            level.correctAnswer = ProcessValidConfig(configLines[1]).Split(',');

            for (int i = 0; i < level.answerAmmount; i++)
                level.answers.Add(ProcessValidConfig(configLines[4 + i]));

            // en caso de que haya nombres en las respuestas
            //level.answers.Add(string.Format(ProcessValidConfig(configLines[4 + i]), sortedNames[fileIndex]));

            level.image = levelImages[fileIndex];

            levelList.Add(level);
        }
    }

    /// <summary>Lee la parte importante de la linea de configuración (Lo que está detras de ':')</summary>
    private string ProcessValidConfig(string configLine) => configLine.Split(':')[1];

    /// <summary>Elige  situaciones de manera random y las guarda en una lista ordenada.</summary>
    private void SetUpLevelOrder()
    {
        lvlAmmountPerDay = UnityEngine.Random.Range(5, 8);
        int dailyLvlCounter = lvlAmmountPerDay;

        // genera una lista de situaciones ordenadas de forma random
        sortedDayLevels.Clear();
        for (int index = 0; index < totalLvlAmmount; index++)
        {
            int rndLvl = UnityEngine.Random.Range(0, totalLvlAmmount);

            // usedLevels evita que se rehusen situaciones de dias previos
            if (sortedDayLevels.Contains(rndLvl) || usedLevels.Contains(rndLvl))
            {
                index--;
                continue;
            }

            sortedDayLevels.Add(rndLvl);
            usedLevels.Add(rndLvl);

            if (--dailyLvlCounter == 0) break;
        }
    }

    /// <summary>Revisa que haya suficientes niveles disponibles para jugar otro día</summary>
    private bool NoLevelsLeft(int levelsLeft)
    {
        if (levelsLeft < 7) return true;

        return false;
    }

    #endregion
}
