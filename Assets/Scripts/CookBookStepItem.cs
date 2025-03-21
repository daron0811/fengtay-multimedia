using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookBookStepItem : MonoBehaviour
{
    public List<GameObject> stepObjs;
    public Animator animator;
    public int currentIndex = 1; // 設定初始1，因為第一步是1

    public List<string> steps;

    private void Start()
    {
        currentIndex = 1;
    }

    private void OnEnable()
    {
        currentIndex = 1;
        foreach (var item in stepObjs)
        {
            item.SetActive(false);
        }
    }

    // public void PlayNextStep(int currentStep)
    // {
    //     float waitForAnimSeconds = GetAnimationLength("Food_0" + currentStep);
    //     StartCoroutine(SetStepAnimation(currentStep, waitForAnimSeconds));
    // }


    //直接播動畫
    public void PlayNextStep()
    {
        float waitForAnimSeconds = GetAnimationLength("Food_0" + currentIndex);
        StartCoroutine(PlayAnimation(waitForAnimSeconds));
    }

    float GetAnimationLength(string animationName)
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

    IEnumerator PlayAnimation(float waitTime = 0.0f)
    {
        yield return new WaitForSeconds(waitTime); // 等待這個動畫播放的時間

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        string currentAnim = stateInfo.shortNameHash.ToString(); // 取得當前動畫名稱 (避免Idle)

        while (true)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            // 如果動畫暫停，則持續等待
            while (animator.speed == 0)
            {
                yield return null; // 等待下一幀
            }

            // 檢查動畫是否播放完畢 (normalizedTime >= 1 表示動畫已經跑完)
            if (stateInfo.normalizedTime >= 1f && stateInfo.shortNameHash.ToString() == currentAnim)
            {
                break; // 動畫結束，跳出迴圈
            }

            yield return null; // 每幀檢查
        }


        currentIndex++; //準備下一個動畫

        //跟Stage3確認下一步，如果已經是最後一步了
        if (Stage3Panel.Instance.currentStep + 1 >= GameManager.Instance.CurrentCookBookInfo.steps.Count)
        {
            yield return new WaitForSeconds(1.0f); // 等待結果1秒後
            Stage3Panel.Instance.GoToFinalStep(); //進入最終步驟
        }
        else
        {
            //做檢查阿！
            yield return new WaitForSeconds(0.5f); // 等待0.5秒
            Stage3Panel.Instance.CheckNextStep(); //在這之前要檢查 這一步是不是有做完 設定步驟資訊
            yield break;
        }

        // if (Stage3Panel.Instance.currentStep > GameManager.Instance.CurrentCookBookInfo.steps.Count)
        // {
        //     Stage3Panel.Instance.GoToFinalStep();
        //     yield break;
        // }
    }

    // IEnumerator SetStepAnimation(int currentStep = 0, float waitTime = 0.0f)
    // {
    //     Debug.LogError($"{currentStep} : {GameManager.Instance.CurrentCookBookInfo.steps.Count}");

    //     yield return new WaitForSeconds(waitTime);

    //     // 確保 currentStep 在有效範圍內
    //     if (currentStep > 0 && currentStep - 1 < stepObjs.Count)
    //     {
    //         stepObjs[currentStep - 1].SetActive(true);
    //     }

    //     // 若 currentStep 超過步驟數，則進入最終步驟
    //     if (currentStep >= GameManager.Instance.CurrentCookBookInfo.steps.Count)
    //     {
    //         Stage3Panel.Instance.GoToFinalStep();
    //         yield break;
    //     }

    //     yield return new WaitForSeconds(0.5f);

    //     Stage3Panel.Instance.SetStepInfo();

    //     // 若已經到最後一步
    //     // if (currentStep == GameManager.Instance.CurrentCookBookInfo.steps.Count - 1)
    //     // {
    //     //     yield return new WaitForSeconds(0.5f);
    //     //     Stage3Panel.Instance.ShowNextStep();
    //     // }
    // }



    // IEnumerator SetStepAnimation(int currentStep = 0, float WaitForSeconds = 0.0f)
    // {
    //     Debug.LogError(currentStep + " : " + GameManager.Instance.CurrentCookBookInfo.steps.Count);
    //     yield return new WaitForSeconds(WaitForSeconds);
    //     if ((currentStep - 1) < stepObjs.Count)
    //     {
    //         stepObjs[currentStep - 1].SetActive(true);
    //     }

    //     if (currentStep > GameManager.Instance.CurrentCookBookInfo.steps.Count)
    //     {
    //         Stage3Panel.Instance.GoToFinalStep();
    //         yield break;
    //     }

    //     yield return new WaitForSeconds(0.5f);

    //     Stage3Panel.Instance.SetStepInfo();

    //     //已經到最後一步
    //     if (currentStep == GameManager.Instance.CurrentCookBookInfo.steps.Count - 1)
    //     {
    //         yield return new WaitForSeconds(0.5f);
    //         Stage3Panel.Instance.ShowNextStep();
    //     }
    // }

    // IEnumerator SetStepAnimation(int currentStep = 0, float waitTime = 0.0f)
    // {
    //     Debug.LogError($"{currentStep} : {GameManager.Instance.CurrentCookBookInfo.steps.Count}");

    //     yield return new WaitForSeconds(waitTime);

    //     // 確保 currentStep 在有效範圍內
    //     if (currentStep > 0 && currentStep - 1 < stepObjs.Count)
    //     {
    //         stepObjs[currentStep - 1].SetActive(true);
    //     }

    //     // 若 currentStep 超過步驟數，則進入最終步驟
    //     if (currentStep >= GameManager.Instance.CurrentCookBookInfo.steps.Count)
    //     {
    //         Stage3Panel.Instance.GoToFinalStep();
    //         yield break;
    //     }

    //     yield return new WaitForSeconds(0.5f);

    //     Stage3Panel.Instance.SetStepInfo();

    //     // 若已經到最後一步
    //     // if (currentStep == GameManager.Instance.CurrentCookBookInfo.steps.Count - 1)
    //     // {
    //     //     yield return new WaitForSeconds(0.5f);
    //     //     Stage3Panel.Instance.ShowNextStep();
    //     // }
    // }

}
