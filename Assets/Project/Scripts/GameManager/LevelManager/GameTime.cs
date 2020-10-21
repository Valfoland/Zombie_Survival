using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTime : MonoBehaviour
{
    private static int startTimeMinutes;
    private static int startTimeSeconds;

    public static string Seconds;
    public static string Minutes;

    private void Start()
    {
        PlayerHealth.onDead += StopAllCoroutines;
        StartCoroutine(StartClock(startTimeMinutes, startTimeSeconds));
    }

    private void OnDestroy()
    {
        PlayerHealth.onDead -= StopAllCoroutines;
    }

    private IEnumerator StartClock(int startMinutes, int startSeconds)
    {
        int Seconds = startSeconds;
        int Minutes = startMinutes;
        const int LAST_SECOND = 59;
        const float WAIT_REAL_TIME = 1f;

        SetSeconds(Seconds);
        SetMinutes(Minutes);

        while (true)
        {
            if (Seconds > LAST_SECOND)
            {
                Seconds = 0;
                Minutes++;
                SetMinutes(Minutes);
            }
            SetSeconds(Seconds);
            Seconds++;
            yield return new WaitForSecondsRealtime(WAIT_REAL_TIME);
        }
    }

    private void SetSeconds(int seconds)
    {
        if (seconds >= 0 && seconds <= 9)
        {
            Seconds = "0" + seconds.ToString();
        }
        else
        {
            Seconds = seconds.ToString();
        }
    }

    private void SetMinutes(int minutes)
    {
        Minutes = minutes + ":";
    }
}
