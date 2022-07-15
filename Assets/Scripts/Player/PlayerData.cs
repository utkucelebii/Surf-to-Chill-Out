using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float record;
    public int currency;
    public float volume;
    public int graphicQuality;

    public PlayerData(GameManager gameManager)
    {
        record = gameManager.record;
        currency = gameManager.currency;
        volume = gameManager.volume;
        graphicQuality = gameManager.graphicQuality;
    }
}
