using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class UIAnimationController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Animator animator;
    public string animationName = "YourAnimation"; // 設定動畫名稱
    public System.Action onAnimationEnd; // 動畫播放完畢的回調
    public float frameInterval = 0.1f; // 每一幀的間隔時間 (控制播放速度)

    private bool isDragging = false;
    private bool isPlaying = false;
    private int currentFrame = 0;
    private float animationLength;
    private int totalFrames;
    private Coroutine animationCoroutine;

    void Start()
    {
        animationLength = GetAnimationLength(animationName);
        totalFrames = Mathf.RoundToInt(animationLength / frameInterval);

        // onAnimationEnd = () => Debug.Log("動畫播放完畢！");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isPlaying)
        {
            isDragging = true;
            PlayAnimationFrameByFrame();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        StopAnimation();
    }

    private void PlayAnimationFrameByFrame()
    {
        if (animationCoroutine != null) return; // 防止重複啟動

        isPlaying = true;
        animationCoroutine = StartCoroutine(PlayFrameByFrame());
    }

    private IEnumerator PlayFrameByFrame()
    {
        while (isDragging && currentFrame < totalFrames)
        {
            float normalizedTime = (float)currentFrame / totalFrames;
            animator.Play(animationName, 0, normalizedTime);
            animator.speed = 0; // 停止 Unity 自動播放

            currentFrame++;
            yield return new WaitForSeconds(frameInterval);
        }

        isPlaying = false;
        animationCoroutine = null;

        if (currentFrame >= totalFrames)
        {
            onAnimationEnd?.Invoke(); // 執行動畫結束的 callback
            animator.speed = 1;
        }
    }

    private void StopAnimation()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
        isPlaying = false;
    }

    private float GetAnimationLength(string animName)
    {
        if (animator == null) return 0;
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        foreach (var clip in ac.animationClips)
        {
            if (clip.name == animName)
            {
                return clip.length; // 回傳動畫長度
            }
        }
        return 0;
    }
}
