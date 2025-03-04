using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookBookStepItem : MonoBehaviour
{
    public List<GameObject> stepObjs;
    public Animator animator;
    public int currentIndex = 0;

    public List<string> steps;

    private void Start()
    {
        currentIndex = 0;
    }

    private void OnEnable()
    {
        currentIndex = 0;
        foreach (var item in stepObjs)
        {
            item.SetActive(false);
        }
    }

    public void PlayNextStep(int currentStep)
    {
        float waitForAnimSeconds = PlayAnimation("Food_0" + currentStep);
        StartCoroutine(SetStepAnimation(currentStep, waitForAnimSeconds));

    }
    float PlayAnimation(string animationName)
    {
        animator.Play(animationName, -1, 0); // 立即播放動畫，從頭開始
        float aniLength = GetAnimationClipLength(animationName);
        return aniLength;
    }


    float GetAnimationClipLength(string clipName)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                Debug.LogError(clipName + " Clip 的影片長度 : " + clip.length);
                return clip.length;
            }
        }
        Debug.LogError("找不到動畫：" + clipName);
        return 0f;
    }
    IEnumerator SetStepAnimation(int currentStep = 0, float WaitForSeconds = 0.0f)
    {
        Debug.LogError(currentStep + " : " + GameManager.Instance.CurrentCookBookInfo.steps.Count);
        yield return new WaitForSeconds(WaitForSeconds);
        if ((currentStep - 1) < stepObjs.Count)
        {
            stepObjs[currentStep - 1].SetActive(true);
        }

        if (currentStep > GameManager.Instance.CurrentCookBookInfo.steps.Count)
        {
            Stage3Panel.Instance.GoToFinalStep();
            yield break;
        }

        yield return new WaitForSeconds(0.5f);

        Stage3Panel.Instance.SetStepInfo();


        //已經到最後一步
        if (currentStep == GameManager.Instance.CurrentCookBookInfo.steps.Count - 1)
        {
            yield return new WaitForSeconds(0.5f);
            Stage3Panel.Instance.ShowNextStep();
        }
    }

    public void ShowFire()
    {

    }
}
