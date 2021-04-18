using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
    public int answerAmmount;

    public string[] correctAnswer;

    public string lead;

    public string situation;

    public List<string> answers;

    public Sprite image;
}
