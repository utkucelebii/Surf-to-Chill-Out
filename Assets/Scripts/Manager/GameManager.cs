using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public float record = 0;
    public int currency = 0;
    public float volume = 0;
    public int graphicQuality = 100;

    [Space(5f)]

    [Header("Default Settings")]
    public int defaultReward = 200;

    private void Awake()
    {
        //LoadData();
    }



    public void LoadData()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        record = data.record;
        currency = data.currency;
        volume = data.volume;
        graphicQuality = data.graphicQuality;   
    }

    public void SaveData()
    {
        //SaveSystem.SavePlayer(this);
    }

}
