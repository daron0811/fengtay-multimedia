using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using DG.Tweening; // 引入 DoTween

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
    public bool IsRunning { get { return isRunning; } }

    public float shakeThreshold = 5.0f; // 🟢 剩餘幾秒時開始晃動
    private bool isShaking = false;   // 是否正在晃動

    public void StartTimer(float durationTime = 10.0f)
    {
        if (isRunning) return;

        duration = durationTime;
        timeLeft = isCountdown ? duration : 0f;
        isRunning = true;
        isShaking = false; // 確保重啟時停止晃動

        onStart?.Invoke();
        StartCoroutine(TimerRoutine());
    }

    public void StopTimer()
    {
        isRunning = false;
        StopAllCoroutines();
        StopShaking(); // 🛑 停止晃動
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

            // 🔥 剩餘時間 <= shakeThreshold 開始晃動
            if (isCountdown && timeLeft <= shakeThreshold && !isShaking)
            {
                StartShaking();
            }

            if ((isCountdown && timeLeft <= 0) || (!isCountdown && timeLeft >= duration))
            {
                isRunning = false;
                onEnd?.Invoke();
                UpdateUI();
                StopShaking(); // 🛑 停止晃動
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

        if (hundredImage != null)
            hundredImage.sprite = (hundreds > 0 || num == 0) ? numberSprites[hundreds] : defaultSprite;

        if (tenImage != null)
            tenImage.sprite = (hundreds > 0 || tens > 0 || num == 0) ? numberSprites[tens] : defaultSprite;

        if (unitImage != null)
            unitImage.sprite = numberSprites[units];
    }

    // ✅ 使用 DoTween 讓數字晃動
    private void StartShaking()
    {
        isShaking = true;
        Transform target = unitImage.transform.parent; // 讓整個數字區域晃動

        target.DOShakePosition(0.5f, 5f, 10, 90, false, true)
              .SetLoops(-1) // 無限循環
              .SetId("ShakeEffect");
    }

    // 🛑 停止晃動
    private void StopShaking()
    {
        isShaking = false;
        DOTween.Kill("ShakeEffect"); // 停止 ID 為 "ShakeEffect" 的動畫
        Transform target = unitImage.transform.parent;
        target.localPosition = Vector3.zero; // 重設回原位
    }
}
