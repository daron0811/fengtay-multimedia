using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookBookStepItem : MonoBehaviour
{
    public List<GameObject> stepObjs;
    public Animator animator;
    public int currentIndex = 1; // 初始為第 1 步驟

    public List<string> steps;

    private bool isPlaying = false;

    private void Start()
    {
        currentIndex = 1;
    }

    private void OnEnable()
    {
        currentIndex = 1;
        isPlaying = false;
        SetAllStepsActive(false);
    }

    public void PlayNextStep()
    {
        if (isPlaying)
        {
            Debug.LogError("還有動畫再播放！！");
            return;
        }
        isPlaying = true;

        string clipName = GetClipName(currentIndex);
        float waitForAnimSeconds = GetAnimationLength(clipName);
        StartCoroutine(PlayAnimation(clipName, waitForAnimSeconds));
    }

    private string GetClipName(int index)
    {
        return $"Food_{index:D2}";
    }

    private float GetAnimationLength(string animationName)
    {
        AnimationClip clip = GetAnimationClip(animationName);
        return clip != null ? clip.length : 0f;
    }

    private AnimationClip GetAnimationClip(string clipName)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                Debug.Log($"找到動畫 {clipName}，長度為 {clip.length} 秒");
                return clip;
            }
        }
        Debug.LogWarning($"找不到動畫: {clipName}");
        return null;
    }

    private IEnumerator PlayAnimation(string clipName, float waitTime)
    {
        animator.Play(clipName, -1, 0);
        float timer = 0f;
        float checkInterval = 0.1f;

        while (timer < waitTime)
        {
            if (animator.speed > 0)
            {
                timer += checkInterval;
            }
            yield return new WaitForSeconds(checkInterval);
        }
        isPlaying = false;
        currentIndex++;

        var stepCount = GameManager.Instance.CurrentCookBookInfo.steps.Count;
        var currentStep = Stage3Panel.Instance.currentStep;


        if (currentStep + 1 >= stepCount)
        {
            yield return new WaitForSeconds(1.0f);
            Stage3Panel.Instance.GoToFinalStep();
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            Stage3Panel.Instance.CheckNextStep();
        }





        // animator.Play(clipName, -1, 0);
        // yield return new WaitForSeconds(waitTime);

        // currentIndex++;

        // var stepCount = GameManager.Instance.CurrentCookBookInfo.steps.Count;
        // var currentStep = Stage3Panel.Instance.currentStep;

        // if (currentStep + 1 >= stepCount)
        // {
        //     yield return new WaitForSeconds(1.0f);
        //     Stage3Panel.Instance.GoToFinalStep();
        // }
        // else
        // {
        //     yield return new WaitForSeconds(0.5f);
        //     Stage3Panel.Instance.CheckNextStep();
        // }
    }

    private void SetAllStepsActive(bool isActive)
    {
        foreach (var item in stepObjs)
        {
            item.SetActive(isActive);
        }
    }
}