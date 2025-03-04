using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.UI;

public class Stage3Panel : MonoSingleton<Stage3Panel>
{
    public GameObject descObj;
    public List<GameObject> descItem;
    public GameObject gameObj;
    public Button nextDescBtn;
    public CountdownTimer countdownTimer;
    private List<FoodItem> foodItems; // 從子物件下取得

    public TextMeshProUGUI cookbookNameText;
    public TextMeshProUGUI cookbookStepText;

    public Image cookbookImage;

    public List<string> steps;
    public int currentStep = 0;

    public Image stepTitleImage;
    public List<Sprite> stepTitleSprites;


    public Image btnImage;
    public Sprite normalSprite;
    public Sprite goSprite;


    [Header("拖曳物件")]
    public List<PickItemToBucket> foodSprites;
    public RectTransform dragTarget;

    private UIAnimationController foodAnimCtrl;

    // [Header("食物演出")]
    // public List<GameObject> foodSpriteList;
    // public Animator potAnimator;
    public CookBookStepItem targetCookBookStep = null;

    [Header("結束片段")]
    public GameObject finishPanel;
    public Button finishBtn;

    public Transform starEffect;
    private List<Image> starImages;

    public Image finishImage;

    bool isFinalStep = false;

    bool isInit = false;
    void Start()
    {
        Init();
    }

    public void Init()
    {
        if (isInit)
        {
            return;
        }
        // nextDescBtn.onClick.AddListener(StartGame);
        nextDescBtn.onClick.AddListener(ShowNextDesc);
        foodItems = gameObj.transform.Find("Food/PickupFoods").GetComponentsInChildren<FoodItem>().ToList();
        foodSprites = gameObj.transform.Find("Food/PickupFoods").GetComponentsInChildren<PickItemToBucket>().ToList();

        foodAnimCtrl = dragTarget.GetComponent<UIAnimationController>();
        foodAnimCtrl.enabled = false;

        finishBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.SetState(UIManager.UIState.Stage_4);
        });
        ResetStatus();
        starImages = new List<Image>();
        if (starEffect != null)
        {
            for (int i = 0; i < starEffect.transform.childCount; i++)
            {
                Transform childTran = starEffect.transform.GetChild(i);
                starImages.Add(childTran.GetComponent<Image>());
                childTran.gameObject.SetActive(false);
            }
        }
        isInit = true;
    }

    void ResetStatus()
    {
        finishPanel.SetActive(false);
        descObj.SetActive(true);
        foreach (var item in descItem)
        {
            item.SetActive(false);
        }
        descItem.First().SetActive(true);
        gameObj.SetActive(false);
        PickItemToBucket();
        btnImage.sprite = normalSprite;
        fire.alpha = 0.0f;
        HideSmoke();
        currentStep = 0;
        //TODO : 改由一開始載入當時食譜的CookbookStepItem

        // foreach (var item in foodSpriteList)
        // {
        //     item.SetActive(false);
        // }

        isFinalStep = false;
    }


    void OnEnable()
    {
        if (isInit == false)
        {
            Init();
        }
        else   // 重置狀態
        {
            ResetStatus();
        }
    }

    private void ShowNextDesc()
    {
        AudioManager.Instance.PlayAudioOnce(1);
        var currentActive = descItem.FirstOrDefault(x => x.activeSelf);
        var currentIndex = descItem.IndexOf(currentActive);
        currentActive.SetActive(false);

        if (currentIndex + 1 < descItem.Count)
        {
            Debug.LogError(currentIndex + 1);
            descItem[currentIndex + 1].SetActive(true);
            if (currentIndex + 1 == descItem.Count - 1)
            {
                btnImage.sprite = goSprite;
            }
            else
            {
                btnImage.sprite = normalSprite;
            }
        }
        else
        {
            StartGame();
        }
    }

    public void PickItemToBucket()
    {
        if (foodSprites == null || foodSprites.Count == 0)
        {
            return;
        }

        foreach (PickItemToBucket item in foodSprites)
        {
            item.bucket = dragTarget;
            item.onDragItem += () =>
            {
                Debug.LogWarning(item.foodImage.sprite.name + " dropped on bucket");
                // string fruitName = item.name.Split('-')[1];
                OnTriggerFoodItem(item.foodImage.sprite.name);
            };
        }
    }

    public void StartGame()
    {
        SetFoodItems(GameManager.Instance.CurrentCookBookIndex);
        cookbookImage.sprite = UIManager.Instance.GetCookBookSprite(GameManager.Instance.CurrentCookBookInfo.icon);
        cookbookNameText.text = GameManager.Instance.CurrentCookBookInfo.name;
        // steps = GameManager.Instance.CurrentCookBookInfo.steps;

        // currentStep = 0;
        cookbookStepText.text = " 步驟準備中... ";//steps[currentStep];
        stepTitleImage.sprite = stepTitleSprites[currentStep];

        descObj.SetActive(false);
        gameObj.SetActive(true);

        countdownTimer.onEnd += () =>
                {
                    //TODO:原本是執行下一步，現在改成30秒
                    isFinalStep = true;
                    ShowNextStep();
                };


        PopupPanel.Instance.PlayReadyPanel(() =>
           {
               countdownTimer.StartTimer(60.0f);
               currentStep = 0;
               SetStepInfo();
           });

    }

    public void SetStepInfo()
    {
        if (currentStep >= GameManager.Instance.CurrentCookBookInfo.steps.Count)
        {
            GoToFinalStep();
            return;
        }
        Debug.LogError("CurrentStep : " + currentStep + "\n" + GameManager.Instance.CurrentCookBookInfo.steps[currentStep] + "\n" + GameManager.Instance.CurrentCookBookInfo.triggerSteps[currentStep]);
        cookbookStepText.color = new Color(1, 1, 1, 0);
        cookbookStepText.rectTransform.anchoredPosition = new Vector2(26.0f, -60.0f);
        cookbookStepText.text = GameManager.Instance.CurrentCookBookInfo.steps[currentStep];
        if (currentStep < stepTitleSprites.Count)
        {
            stepTitleImage.sprite = stepTitleSprites[currentStep];
        }

        cookbookStepText.DOFade(1.0f, 0.3f);
        cookbookStepText.rectTransform.DOLocalMove(new Vector3(26.0f, -40.0f, 0.0f), 1.0f);


        //如果下一個步驟是要持續滑動
        if (GameManager.Instance.CurrentCookBookInfo.triggerSteps[currentStep].Contains("swipe"))
        {
            //
            Debug.LogWarning("!!!SetSwipeStep");
            SetSwipeStep();
        }
        else if (GameManager.Instance.CurrentCookBookInfo.triggerSteps[currentStep].Contains("null"))
        {
            ShowNextStep();
        }
    }

    public void SetSwipeStep()
    {
        if (foodAnimCtrl != null)
        {
            currentStep++;
            foodAnimCtrl.animator = targetCookBookStep.animator;
            foodAnimCtrl.animationName = "Food_0" + currentStep;
            foodAnimCtrl.enabled = true;
            foodAnimCtrl.onAnimationEnd = () =>
            {
                foodAnimCtrl.enabled = false;
                SetStepInfo();
            };
        }
    }

    //執行下一步
    public void ShowNextStep()
    {
        ////原本每10秒執行下一個步驟的作法
        // if (countdownTimer.IsRunning)
        // {
        //     countdownTimer.StopTimer();
        // }
        // foodSpriteList[currentStep].SetActive(true);

        // currentStep++;

        // if (currentStep >= steps.Count) // 結束步驟
        // {
        //     countdownTimer.StopTimer();
        //     countdownTimer.onEnd -= ShowNextStep;
        //     UIManager.Instance.SetState(UIManager.UIState.Stage_4);
        //     return;
        // }
        // if (currentStep < steps.Count)
        // {
        //     countdownTimer.StopTimer();
        // }
        // cookbookStepText.text = steps[currentStep];
        // stepTitleImage.sprite = stepTitleSprites[currentStep];
        // countdownTimer.StartTimer();


        ///
        if (isFinalStep)
        {
            GoToFinalStep();
            return;
        }
        else
        {
            currentStep++;
            currentStep = Mathf.Clamp(currentStep, 0, GameManager.Instance.CurrentCookBookInfo.steps.Count);
            //如果有觸發成功，播動畫
            targetCookBookStep.PlayNextStep(currentStep);

            // float waitForAnimSeconds = PlayAnimation("Food_0" + currentStep);

            // StartCoroutine(SetStepAnimation(currentStep, waitForAnimSeconds));


            // if (currentStep >= steps.Count) // 結束步驟
            // {
            //     GoToFinalStep();
            //     return;
            // }
            // // if (currentStep < steps.Count)
            // // {
            // //     countdownTimer.StopTimer();
            // // }
            // cookbookStepText.text = steps[currentStep];
            // stepTitleImage.sprite = stepTitleSprites[currentStep];
        }
    }

    // float GetAnimationClipLength(string clipName)
    // {
    //     foreach (AnimationClip clip in potAnimator.runtimeAnimatorController.animationClips)
    //     {
    //         if (clip.name == clipName)
    //         {
    //             Debug.LogError(clipName + " Clip 的影片長度 : " + clip.length);
    //             return clip.length;
    //         }
    //     }
    //     Debug.LogError("找不到動畫：" + clipName);
    //     return 0f;
    // }

    // IEnumerator SetStepAnimation(int currentStep = 0, float WaitForSeconds = 0.0f)
    // {
    //     yield return new WaitForSeconds(WaitForSeconds);
    //     if (currentStep >= GameManager.Instance.CurrentCookBookInfo.steps.Count)
    //     {
    //         GoToFinalStep();
    //         yield break;
    //     }

    //     // foodSpriteList[currentStep - 1].SetActive(true);
    //     yield return new WaitForSeconds(0.5f);

    //     SetStepInfo();

    //     if (currentStep == 3)
    //     {
    //         yield return new WaitForSeconds(0.5f);
    //         ShowNextStep();
    //     }
    // }


    // float PlayAnimation(string animationName)
    // {
    //     potAnimator.Play(animationName, -1, 0); // 立即播放動畫，從頭開始
    //     float aniLength = GetAnimationClipLength(animationName);
    //     return aniLength;


    //     // AnimatorStateInfo stateInfo = potAnimator.GetCurrentAnimatorStateInfo(0);
    //     // Debug.LogError(stateInfo.length);
    //     // return stateInfo.length;
    // }

    public void GoToFinalStep()
    {
        countdownTimer.StopTimer();
        countdownTimer.onEnd -= ShowNextStep;
        // PopupPanel.Instance.goodJobPanel()
        PopupPanel.Instance.PlayGoodJob(() =>
           {
               finishImage.sprite = UIManager.Instance.GetCookBookSprite(GameManager.Instance.CurrentCookBookInfo.icon);
               finishPanel.SetActive(true);
               foreach (Image star in starImages)
               {
                   star.gameObject.SetActive(true);
                   FlickerStar(star);
               }
               //    UIManager.Instance.SetState(UIManager.UIState.Stage_4);
           });
    }
    void OnDisable()
    {
        foreach (Image star in starImages)
        {
            star.DOKill(); // 停止該 Image 上的所有 DoTween 動畫
        }
    }


    public void FlickerStar(Image star)
    {
        float duration = UnityEngine.Random.Range(0.5f, 1.5f); // 隨機閃爍時間
        float delay = UnityEngine.Random.Range(0f, 1f); // 隨機延遲開始時間

        // 執行閃爍動畫
        star.DOFade(0, duration)
            .SetDelay(delay)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => // 動畫完成後
            {
                float waitTime = UnityEngine.Random.Range(1f, 3f); // 隨機停頓 1~3 秒
                star.DOFade(1, 0.5f).OnComplete(() => // 先恢復顯示
                {
                    ChangePosition(star); // 隨機改變位置
                    FlickerStar(star); // 重新播放動畫
                }).SetDelay(waitTime);
            });

        // 可選：加入縮放變化
        star.transform.DOScale(UnityEngine.Random.Range(0.8f, 1.2f), duration)
            .SetEase(Ease.InOutSine);
    }

    void ChangePosition(Image star)
    {
        float newX = UnityEngine.Random.Range(-580.0f, 580.0f);
        float newY = UnityEngine.Random.Range(270.0f, -178.0f);
        star.rectTransform.anchoredPosition = new Vector2(newX, newY);
    }

    public void SetFoodItems(int cookbookIndex)
    {
        List<string> foods = DataManager.Instance.GetFoodbyCookbook(cookbookIndex);
        if (foods == null)
        {
            return;
        }

        for (int i = 0; i < foodItems.Count; i++)
        {
            if (i < foods.Count)
            {
                Sprite sprite = UIManager.Instance.GetFoodSprite(foods[i]);
                Debug.LogError(sprite.name + foods[i]);
                foodItems[i].SetFoodItem(sprite, foods[i], DataManager.Instance.GetFoodInfo(foods[i]).locate);
                foodItems[i].Show();
            }
            else
            {
                foodItems[i].Hide();
            }
        }

        return;
        Sprite sp = UIManager.Instance.GetFoodSprite("起司");
        if (sp != null)
        {
            foodItems.Last().SetFoodItem(sp, "起司", "起司");
            foodItems.Last().Show();
        }
    }

    public void OnTriggerFoodItem(string foodName)
    {
        Debug.LogWarning(foodName);
        if (GameManager.Instance.CurrentCookBookInfo.triggerSteps[currentStep].Contains(foodName) == true)
        {
            Debug.LogWarning("Step Contains Food :" + foodName);
            ShowNextStep();
            AudioManager.Instance.PlayAudioOnce(4);
        }
        else
        {
            Debug.LogWarning("Step :" + currentStep + "沒有此步驟 :" + foodName + " [ " + GameManager.Instance.CurrentCookBookInfo.triggerSteps[currentStep] + "]");
        }
    }

    public bool CheckStep()
    {
        return false;
    }

    #region 特效部分
    public CanvasGroup fire;
    public ParticleSystem smoke;

    public void ShowFire()
    {
        fire.alpha = 0.0f;
        fire.DOFade(1, 0.5f);
    }
    public void HideFire()
    {
        fire.alpha = 1.0f;
        fire.DOFade(0, 0.5f);
    }

    public void ShowSmoke()
    {
        if (smoke != null)
        {
            smoke.gameObject.SetActive(true);
            var emission = smoke.emission;//.enabled = false;
            emission.enabled = true;
        }
    }

    public void HideSmoke()
    {
        if (smoke != null)
        {
            smoke.gameObject.SetActive(true);
            var emission = smoke.emission;//.enabled = false;
            emission.enabled = false;
        }
    }

    #endregion
}
