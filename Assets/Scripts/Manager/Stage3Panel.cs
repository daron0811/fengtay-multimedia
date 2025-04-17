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
    [Header("食譜物件Prefab")]
    public List<GameObject> foodPrefabList; // 目前8款

    public GameObject descObj;
    public List<GameObject> descItem;
    public GameObject gameObj;
    public Button nextDescBtn;
    public CountdownTimer countdownTimer;
    private List<FoodItem> foodItems; // 從子物件下取得

    public TextMeshProUGUI cookbookNameText;
    public TextMeshProUGUI cookbookStepText;

    public Image cookbookImage;

    [Header("食譜步驟")]
    public int currentStep = 0;
    public List<string> stepConditions;
    public string[] currentIngredients;
    public int ingredientIndex = 0;

    //標題的步驟數字
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

    bool isMulitTrigger = false; //現在是不是複選項目

    public int pickCountForScore = 0; //計算成績用，依照目前要選的是滿分，有成功選擇+1，如果沒選到就依照這個做扣分依據

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
        
        PickItemToBucket();
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
        btnImage.sprite = normalSprite;
        fire.alpha = 0.0f;
        currentStep = 0;
        pickCountForScore = 0;

        HideSmoke();
        HideFire();
        HideSwipeSFX();
        HideTapSFX();

        //TODO : 改由一開始載入當時食譜的CookbookStepItem
        //清除原本子物件
        for (int i = foodAnimCtrl.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(foodAnimCtrl.transform.GetChild(i).gameObject);
        }
        // foreach (var item in foodSpriteList)
        // {
        //     item.SetActive(false);
        // }

        isFinalStep = false;
        isMulitTrigger = false;

        if (foodAnimCtrl != null)
        {
            foodAnimCtrl.onAnimationEnd = null;
            foodAnimCtrl.onTapAnimation = null;

            // Stage3Panel.cs 的 ResetStatus() 增加以下程式
            foodAnimCtrl.enabled = false;
            foodAnimCtrl.ResetStatus(); // 這非常重要
        }

    }

    //TODO:可能會有重複進入問題
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
                OnTriggerFoodItem(item.foodImage.sprite.name);
            };
        }
    }

    //當投入物件的時候要做檢查
    public void OnTriggerFoodItem(string foodName)
    {
        CheckCondition(foodName);
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

    //開始遊戲準備前的設定
    public void StartGame()
    {
        SetFoodItems(GameManager.Instance.CurrentCookBookIndex);
        cookbookImage.sprite = UIManager.Instance.GetCookBookSprite(GameManager.Instance.CurrentCookBookInfo.icon);
        cookbookNameText.text = GameManager.Instance.CurrentCookBookInfo.name;

        //TODO:這邊要先檢查foodAnimCtrl底下是有其他物件
        GameObject cookbookObj = GameObject.Instantiate(foodPrefabList[GameManager.Instance.CurrentCookBookInfo.id], foodAnimCtrl.transform);
        targetCookBookStep = cookbookObj.GetComponent<CookBookStepItem>();
        cookbookObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        cookbookObj.SetActive(true);

        currentIngredients = null;
        ingredientIndex = 0;
        currentStep = 0;
        stepConditions = GameManager.Instance.CurrentCookBookInfo.triggerSteps;
        cookbookStepText.rectTransform.anchoredPosition = new Vector2(-10.0f, -40.0f);
        cookbookStepText.text = " 步驟準備中 ...";
        stepTitleImage.sprite = stepTitleSprites[currentStep];

        descObj.SetActive(false);
        gameObj.SetActive(true);

        countdownTimer.onEnd -= OnTimerEnd;
        countdownTimer.onEnd += OnTimerEnd;
        // countdownTimer.onEnd += () =>
        //         {
        //             //TODO:原本是執行下一步，現在改成30秒
        //             isFinalStep = true;
        //             GoToFinalStep();
        //         };

        PopupPanel.Instance.PlayReadyPanel(() =>
           {
               countdownTimer.StartTimer(60.0f);
               currentStep = 0;
               SetStepInfo(); // 開始遊戲後設置資訊
           });
    }

    // 將原本 lambda 抽出來變成獨立方法
    private void OnTimerEnd()
    {
        isFinalStep = true;
        GoToFinalStep();
    }

    #region UI相關 

    //設定櫃子上食材類型
    public void SetFoodItems(int cookbookIndex)
    {
        List<string> foods = DataManager.Instance.GetFoodbyCookbook(cookbookIndex);
        if (foods == null)
        {
            return;
        }
        if (foods.Count == 4)
        {
            decorateGroup.SetActive(false);
        }
        else
        {
            decorateGroup.SetActive(true);
        }
        for (int i = 0; i < foodItems.Count; i++)
        {
            if (i < foods.Count)
            {
                Sprite sprite = UIManager.Instance.GetFoodSprite(foods[i]);
                // Debug.LogError(sprite.name + foods[i]);
                foodItems[i].SetFoodItem(sprite, foods[i], DataManager.Instance.GetFoodInfo(foods[i]).locate);
                foodItems[i].ShowParticle(false);
                foodItems[i].Show();
            }
            else
            {
                foodItems[i].Hide();
            }
        }
    }

    #endregion

    #region 前置說明
    private void ShowNextDesc()
    {
        AudioManager.Instance.PlayAudioOnce(1);
        var currentActive = descItem.FirstOrDefault(x => x.activeSelf);
        var currentIndex = descItem.IndexOf(currentActive);
        currentActive.SetActive(false);

        if (currentIndex + 1 < descItem.Count)
        {
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
    #endregion


    //設定步驟資訊
    public void SetStepInfo()
    {
        if (currentStep >= GameManager.Instance.CurrentCookBookInfo.steps.Count)
        {
            Debug.LogError("已經是最後一步了! 確定要更新資訊?");
            GoToFinalStep();
            return;
        }

        Debug.LogError("SetStepInfo!!!");
        cookbookStepText.color = new Color(1, 1, 1, 0);
        cookbookStepText.rectTransform.anchoredPosition = new Vector2(-10.0f, -60.0f);
        cookbookStepText.text = GameManager.Instance.CurrentCookBookInfo.steps[currentStep];

        if (currentStep < stepTitleSprites.Count) // 顯示標題現在第幾步
        {
            stepTitleImage.sprite = stepTitleSprites[currentStep];
        }
        cookbookStepText.DOFade(1.0f, 0.3f);
        cookbookStepText.rectTransform.DOLocalMove(new Vector3(0.0f, -40.0f, 0.0f), 1.0f);


        //設定完文字後,檢查目前的狀態
        CheckCondition(); // 為了檢查狀態，是要播放點擊還是拖拉
        return;

        if (currentStep >= GameManager.Instance.CurrentCookBookInfo.steps.Count)
        {
            GoToFinalStep();
            return;
        }
        Debug.LogError("CurrentStep : " + currentStep + "\n" + GameManager.Instance.CurrentCookBookInfo.steps[currentStep] + "\n" + GameManager.Instance.CurrentCookBookInfo.triggerSteps[currentStep]);

        //因為這個階段還沒達成條件
        if (currentIngredients != null && ingredientIndex < currentIngredients.Length)
        {
            return;
        }



        CheckCondition(); // 為了檢查狀態
        //如果下一個步驟是要持續滑動
        // if (GameManager.Instance.CurrentCookBookInfo.triggerSteps[currentStep].Contains("swipe"))
        // {
        //     //
        //     Debug.LogWarning("!!!SetSwipeStep");
        //     SetSwipeStep();
        // }
        // else if (GameManager.Instance.CurrentCookBookInfo.triggerSteps[currentStep].Contains("null"))
        // {
        //     ShowNextStep();
        // }
    }

    //檢查目前的條件是否吻合
    void CheckCondition(string input = "")
    {
        Debug.LogError("CheckCondition " + currentStep + " , Input : " + input + " : " + stepConditions[currentStep]);
        if (stepConditions == null || stepConditions.Count == 0)
        {
            stepConditions = GameManager.Instance.CurrentCookBookInfo.triggerSteps;
        }

        if (stepConditions[currentStep].Contains(",")) // 若條件為多個食材，則拆解
        {
            if (currentIngredients == null || currentIngredients.Length == 0)
            {
                currentIngredients = stepConditions[currentStep].Split(',');
                ingredientIndex = 0;
                isMulitTrigger = true;
            }

            foreach (var item in foodItems)
            {
                if (item.myFoodName == currentIngredients[ingredientIndex])
                {
                    item.ShowParticle(true);
                }
                else
                {
                    item.ShowParticle(false);
                }
            }

            if (input == currentIngredients[ingredientIndex])
            {
                ingredientIndex++;
                SetPlayNextStep();//播動畫哦！
                //因為食材關係分數要記
                pickCountForScore++;
            }
            else if (currentIngredients[ingredientIndex] == "null")
            {
                ingredientIndex++;
                SetPlayNextStep(); //播動畫哦！
            }
            else if (currentIngredients[ingredientIndex] == "tap") // 要點擊
            {
                ingredientIndex++;
                SetTapStep();
            }
            else if (currentIngredients[ingredientIndex] == "swipe") // 要滑動
            {
                ingredientIndex++;
                SetSwipeStep();
            }
            else
            {
                return; // 按錯順序則直接跳出，保持當前進度
            }
            return;
        }
        else if (stepConditions[currentStep] == input) // 如果是食材對應
        {
            SetPlayNextStep();
            currentIngredients = null;
            isMulitTrigger = false;
            ingredientIndex = 0;
            pickCountForScore++;
        }
        else if (stepConditions[currentStep] == "null") // 如果是食材對應
        {
            SetPlayNextStep(); //播動畫哦！
            currentIngredients = null;
            isMulitTrigger = false;
            ingredientIndex = 0;
        }
        else if (stepConditions[currentStep] == "tap") // 要點擊
        {
            SetTapStep();
            currentIngredients = null;
            isMulitTrigger = false;
            ingredientIndex = 0;
        }
        else if (stepConditions[currentStep] == "swipe") // 要滑動
        {
            SetSwipeStep();
            currentIngredients = null;
            isMulitTrigger = false;
            ingredientIndex = 0;
        }

        foreach (var item in foodItems)
        {
            if (item.myFoodName == stepConditions[currentStep])
            {
                item.ShowParticle(true);
            }
            else
            {
                item.ShowParticle(false);
            }
        }
    }
    public void SetSwipeStep()
    {
        //這邊有錯 不要用自己的currentstep
        Debug.LogError("============Start [Swipe]");
        if (foodAnimCtrl != null)
        {
            ShowSwipeSFX();
            targetCookBookStep.animator.speed = 0;
            foodAnimCtrl.animator = targetCookBookStep.animator;
            foodAnimCtrl.animationName = "Food_0" + targetCookBookStep.currentIndex;
            foodAnimCtrl.enabled = true;
            foodAnimCtrl.nowIsDrag = true;
            foodAnimCtrl.onAnimationEnd = () =>
            {
                targetCookBookStep.currentIndex++;
                CheckNextStep();
                foodAnimCtrl.enabled = false;
                foodAnimCtrl.ResetStatus();
                targetCookBookStep.animator.speed = 1;
            };
        }
    }

    public void SetTapStep()
    {
        //這邊有錯 不要用自己的currentstep
        Debug.LogError("============Start [Tap]");
        if (foodAnimCtrl != null)
        {
            // countdownTimer.PauseTimer();
            ShowTapSFX();
            // currentStep++;
            foodAnimCtrl.animator = targetCookBookStep.animator;
            foodAnimCtrl.enabled = true;
            foodAnimCtrl.nowIsTap = true;
            foodAnimCtrl.onTapAnimation = () =>
            {
                SetPlayNextStep(); //播動畫哦！
                foodAnimCtrl.enabled = false;
                foodAnimCtrl.ResetStatus();
            };
        }
    }

    //播放下一段動畫
    public void SetPlayNextStep()
    {
        if (targetCookBookStep != null)
        {
            targetCookBookStep.PlayNextStep();
            foreach (var foodItem in foodItems)
            {
                foodItem.ShowParticle(false);
            }
        }
        if (countdownTimer != null)
        {
            countdownTimer.PauseTimer();
        }
    }

    //執行下一步
    public void CheckNextStep()
    {
        ///都從這裡做檢查，是不是要下一步
        if (currentIngredients != null && currentIngredients.Length != 0 && ingredientIndex < currentIngredients.Length) // 如果還有食材沒放完，則不執行下一步
        {
            Debug.LogError("目前還沒完成，請繼續" + currentStep);
            CheckCondition(); // 因為還沒要更新文字，但還是要繼續檢查狀態！！這邊可能要換成另一個方法
            return;
        }

        if (currentIngredients != null && ingredientIndex >= currentIngredients.Length - 1)
        {
            Debug.LogError("本次步驟已完成！請繼續" + currentStep);
            currentIngredients = null;
            ingredientIndex = 0;
        }

        //已經完成
        if (currentStep >= GameManager.Instance.CurrentCookBookInfo.steps.Count)
        {
            Debug.LogError("完成 !");
            GoToFinalStep();
            return;
        }
        currentStep++; // 進入下一步
        Debug.LogError("進入下一步 " + currentStep + " " + GameManager.Instance.CurrentCookBookInfo.triggerStep[currentStep]);
        if (countdownTimer.IsPaused)
        {
            countdownTimer.ResumeTimer();
        }
        SetStepInfo();
        return;



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
            targetCookBookStep.PlayNextStep();

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


    public void GoToFinalStep()
    {
        countdownTimer.StopTimer();
        countdownTimer.onEnd -= CheckNextStep;
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
               //計算成績
               //    pickCountForScore++;
               int foodCount = DataManager.Instance.GetFoodbyCookbook(GameManager.Instance.CurrentCookBookIndex).Count;
               pickCountForScore = Mathf.Clamp(pickCountForScore, 0, foodCount); // 限制在食材數量範圍內
               int score = foodCount - pickCountForScore; // 結算
               GameManager.Instance.Score -= score;
               Debug.LogError("你的分數:" + GameManager.Instance.Score);
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



    #region 特效部分
    public CanvasGroup fire;
    public ParticleSystem smoke;
    public ContractInfo contractInfo;

    public GameObject swipeEffect;
    public GameObject tapEffect;

    public GameObject decorateGroup;

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

    public void ShowControactInfo(int type = 0)
    {
        targetCookBookStep.animator.speed = 0;
        contractInfo.Show(type);
    }
    public void ShowSwipeSFX()
    {
        swipeEffect.SetActive(true);
    }

    public void HideSwipeSFX()
    {
        swipeEffect.SetActive(false);
    }

    public void ShowTapSFX()
    {
        tapEffect.SetActive(true);
    }

    public void HideTapSFX()
    {
        tapEffect.SetActive(false);
    }
    #endregion
}