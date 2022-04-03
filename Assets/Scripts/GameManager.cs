using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    Point gridSize = new Point(64, 40);
    MossController mossController;
    PlayerMovement player;

    ScoreKeeper scoreKeeper;

    private void Awake()
    {
        mossController = GetComponent<MossController>();
        player = FindObjectOfType<PlayerMovement>();
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
    }

    // Start is called before the first frame update
    void Start()
    {
        scoreKeeper.restartTimer();
        mossController.initGrid(gridSize);
        mossController.SpawnMossAt(new Point(15, 10));
        mossController.StartMossExpansion();
    }

    // Update is called once per frame
    void Update()
    {
        int mossCover = mossController.getMossCoverPercent();

        if (mossCover == 0) { Win(); }
        if (mossCover >= 90) { Loose(false); }
        if (mossController.mossStatusAtWorldPos(player.transform.position)) { Loose(true); }
    }

    private void Loose(bool looseByContact)
    {
        scoreKeeper.SetGameOver(false, looseByContact);
        FindObjectOfType<SceneController>().LoadGameOver();
    }

    private void Win()
    {
        scoreKeeper.SetGameOver(true, true);
        FindObjectOfType<SceneController>().LoadGameOver();
    }
}
