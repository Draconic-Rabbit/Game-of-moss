using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverController : MonoBehaviour
{
    ScoreKeeper scoreKeeper;
    [SerializeField] TextMeshProUGUI gameoverText;
    [SerializeField] TextMeshProUGUI titleText;
    // Start is called before the first frame update
    void Start()
    {
        scoreKeeper = FindObjectOfType<ScoreKeeper>();

        if (scoreKeeper == null) return;

        string durationString = System.Math.Round(scoreKeeper.duration, 2).ToString();

        if (scoreKeeper.isWin)
        {
            titleText.text = "You saved the day !";
            gameoverText.text = "The world is safe. Sadly, nobody should know what happened in your lab.\nYour boss should stay ignorant of this event. You won't be fired.\nIt took you " + durationString + " seconds to perform this feat.";
        }
        else
        {
            if (scoreKeeper.isLooseByContact)
            {
                titleText.text = "The world is covered\nby moss.";
                gameoverText.text = "You slipped over the moss. And it's started to cover your whole body.\nAn unfortunate fate, for a renowned scientist.\nAt least, you delayed the inevitable by " + durationString + " seconds.";
            }
            else
            {
                titleText.text = "The world is covered\nby moss.";
                gameoverText.text = "You tried your best to protect the world from your error.\nUnfortunately, the moss you created was too efficient.\nAt least, you delayed the inevitable by " + durationString + " seconds.";
            }
        }
    }
}
