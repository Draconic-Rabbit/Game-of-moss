using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public bool isWin;
    public bool isLooseByContact;
    public float duration;

    float startTime;

    static ScoreKeeper instance;

    public ScoreKeeper GetInstance()
    {
        return instance;
    }

    private void ManageSingleton()
    {
        if (instance != null)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Awake()
    {
        ManageSingleton();
    }

    public void restartTimer()
    {
        startTime = Time.time;
    }

    internal void SetGameOver(bool v, bool looseByContact)
    {
        isWin = v;
        this.isLooseByContact = looseByContact;
        duration = Time.time - startTime;
    }
}
