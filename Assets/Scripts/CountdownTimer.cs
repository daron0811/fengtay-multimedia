using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class CountdownTimer : MonoBehaviour
{
    public float duration = 10f; // 設定倒數時間（秒）
    public bool isCountdown = true; // true: 倒數計時, false: 正向計時

    public Image hundredImage;  // 百位數
    public Image tenImage;      // 十位數
    public Image unitImage;     // 個位數
    public Sprite[] numberSprites; // 0~9 對應的數字圖片
    public Sprite defaultSprite; // 預設空白圖片

    public event Action onStart; // 計時開始事件
    public event Action onEnd; // 計時結束事件

    private float timeLeft;
    private bool isRunning = false;

    public void StartTimer(float durationTime = 10.0f)
    {
        if (isRunning) return;

        duration = durationTime;
        timeLeft = isCountdown ? duration : 0f;
        isRunning = true;
        onStart?.Invoke();
        StartCoroutine(TimerRoutine());
    }

    public void StopTimer()
    {
        isRunning = false;
        StopAllCoroutines();
    }

    private IEnumerator TimerRoutine()
    {
        while (isRunning)
        {
            UpdateUI();
            yield return new WaitForSeconds(1f);

            if (isCountdown)
                timeLeft--;
            else
                timeLeft++;

            if ((isCountdown && timeLeft <= 0) || (!isCountdown && timeLeft >= duration))
            {
                isRunning = false;
                onEnd?.Invoke();
                UpdateUI();
                yield break;
            }
        }
    }

    private void UpdateUI()
    {
        int num = Mathf.Clamp((int)timeLeft, 0, 999);
        int hundreds = num / 100;
        int tens = (num / 10) % 10;
        int units = num % 10;

        // 更新百位數圖片
        if (hundredImage != null)
            hundredImage.sprite = (hundreds > 0 || num == 0) ? numberSprites[hundreds] : defaultSprite;

        // 更新十位數圖片
        if (tenImage != null)
            tenImage.sprite = (hundreds > 0 || tens > 0 || num == 0) ? numberSprites[tens] : defaultSprite;

        // 更新個位數圖片
        if (unitImage != null)
            unitImage.sprite = numberSprites[units];
    }
}
