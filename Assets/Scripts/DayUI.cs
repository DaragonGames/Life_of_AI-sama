using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayUI : MonoBehaviour
{
    public float timeInSec = 5;
    public Image bg;
    public TMP_Text text;
    private float alpha = 1;
    private Days lastDay = Days.monday;

    void Start()
    {
        GameManager.instance.Progression += NewDay;
    }

    void OnDestroy()
    {
        GameManager.instance.Progression -= NewDay;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeInSec < 0)
        {
            timeInSec = 0;
            return;
        }
        alpha -= Time.deltaTime / timeInSec;
        bg.color = new(1, 1, 1, alpha);
        text.color = new(1, 1, 1, alpha);
    }

    public void NewDay()
    {
        GameManager gm = GameManager.instance;
        if (lastDay == gm.currentDay)
        {
            return;
        }
        lastDay = gm.currentDay;
        string[] days = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        int dayID = (int)gm.currentDay;
        text.text = days[dayID];
        alpha = 1;
    }
}
