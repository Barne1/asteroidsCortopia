using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUIManager : MonoBehaviour
{
    [NonSerialized] public int Score = 0;
    private Text text;

    private void Start()
    {
        text = GetComponent<Text>();
    }

    public void AddScore(int addScore)
    {
        Score += addScore;
        text.text = "Score: " + Score;
    }
}
