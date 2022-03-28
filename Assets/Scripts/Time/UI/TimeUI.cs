using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TimeUI : MonoBehaviour
{
    public RectTransform dayNightImage;
    public RectTransform clockParent;
    public Image seasonImage;
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI timeText;

    public Sprite[] seasonSprites;

    private List<GameObject> clockBlocks = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < clockParent.childCount; i++)
        {
            clockBlocks.Add(clockParent.GetChild(i).gameObject);
            clockParent.GetChild(i).gameObject.SetActive(false);
        }    
    }

    private void OnEnable()
    {
        EventHandler.GameMinuteEvent += OnGameMinuteEvent;
        EventHandler.GameDateEvent += OnGameDateEvent;
    }

    private void OnDisable()
    {
        EventHandler.GameMinuteEvent -= OnGameMinuteEvent;
        EventHandler.GameDateEvent -= OnGameDateEvent;
    }

    private void OnGameMinuteEvent(int minute, int hour)
    {
        timeText.text = hour.ToString("00") + ":" + minute.ToString("00");
    }

    private void OnGameDateEvent(int hour, int day, int mouth, int year, Season season)
    {
        dateText.text = year + "Äê" + mouth.ToString("00") + "ÔÂ" + day.ToString("00") + "ÈÕ";
        seasonImage.sprite = seasonSprites[(int)season];

        SwitchHourImage(hour);
        DayNightImageRotate(hour);
    }

    private void SwitchHourImage(int hour)
    {
        int index = hour / 4;
        for (int i = 0; i < clockBlocks.Count; i++)
        {
            if (i < index + 1)
                clockBlocks[i].SetActive(true);
            else
                clockBlocks[i].SetActive(false);
        }
    }

    private void DayNightImageRotate(int hour)
    {
        var target = new Vector3(0, 0, hour * 15 - 90);
        dayNightImage.DORotate(target, 1f, RotateMode.Fast);
    }
}
