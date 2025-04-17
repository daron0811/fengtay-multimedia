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
    private bool isPaused = false; // 🔵 新增：暫停狀態
    public bool IsRunning { get { return isRunning; } }
    public bool IsPaused { get { return isPaused; } }

    public float shakeThreshold = 5.0f; // 🟢 剩餘幾秒時開始晃動
    private bool isShaking = false;   // 是否正在晃動

    private Vector2 originPos;

    private void Awake()
    {
        originPos = this.GetComponent<RectTransform>().anchoredPosition;
    }

    private void OnEnable()
    {
        if (originPos == Vector2.zero)
        {
            GetComponent<RectTransform>().anchoredPosition = originPos;
        }
    }

    public void StartTimer(float durationTime = 10.0f)
    {
        this.GetComponent<RectTransform>().anchoredPosition = originPos; //重製位置
        if (isRunning) return;

        duration = durationTime;
        timeLeft = isCountdown ? duration : 0f;
        isRunning = true;
        isPaused = false;
        isShaking = false;

        onStart?.Invoke();
        StartCoroutine(TimerRoutine());
    }

    public void StopTimer()
    {
        isRunning = false;
        isPaused = false;
        StopAllCoroutines();
        StopShaking();
    }

    public void PauseTimer() // 🔵 新增：暫停功能
    {
        isPaused = true;
    }

    public void ResumeTimer() // 🔵 新增：恢復功能
    {
        if (isRunning && isPaused)
        {
            isPaused = false;
        }
    }

    private IEnumerator TimerRoutine()
    {
        while (isRunning)
        {
            if (!isPaused)
            {
                UpdateUI();
                yield return new WaitForSeconds(1f);

                if (isCountdown)
                    timeLeft--;
                else
                    timeLeft++;

                if (isCountdown && timeLeft <= shakeThreshold && !isShaking)
                {
                    StartShaking();
                }

                if ((isCountdown && timeLeft <= 0) || (!isCountdown && timeLeft >= duration))
                {
                    isRunning = false;
                    onEnd?.Invoke();
                    UpdateUI();
                    StopShaking();
                    yield break;
                }
            }
            else
            {
                // 若暫停中，每幀等待，避免過多 yield WaitForSeconds 呼叫
                yield return null;
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
