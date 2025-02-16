using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [Header("食物演出")]
    public List<GameObject> foodSpriteList;

    public Animator potAnimator;

    [Header("結束片段")]
    public GameObject finishPanel;
    public Button finishBtn;

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

        finishBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.SetState(UIManager.UIState.Stage_4);
        });
        ResetStatus();
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
        foreach (var item in foodSpriteList)
        {
            item.SetActive(false);
        }
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

        cookbookNameText.text = GameManager.Instance.CurrentCookBookInfo.name;
        steps = GameManager.Instance.CurrentCookBookInfo.steps;

        currentStep = 0;
        cookbookStepText.text = steps[currentStep];
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
               countdownTimer.StartTimer(30.0f);
           });

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
            foodSpriteList[currentStep].SetActive(true);
            currentStep++;
            float waitForAnimSeconds = PlayAnimation("Food_0" + currentStep);

            StartCoroutine(SetStepAnimation(currentStep, waitForAnimSeconds));
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

    IEnumerator SetStepAnimation(int currentStep = 0, float WaitForSeconds = 0.0f)
    {
        yield return new WaitForSeconds(WaitForSeconds);
        if (currentStep >= steps.Count)
        {
            GoToFinalStep();
            yield break;
        }

        cookbookStepText.text = steps[currentStep];
        stepTitleImage.sprite = stepTitleSprites[currentStep];
        yield break;
    }


    float PlayAnimation(string animationName)
    {
        potAnimator.Play(animationName, -1, 0); // 立即播放動畫，從頭開始
        AnimatorStateInfo stateInfo = potAnimator.GetCurrentAnimatorStateInfo(0);
        Debug.LogError(stateInfo.length);
        return stateInfo.length;
    }

    public void GoToFinalStep()
    {
        countdownTimer.StopTimer();
        countdownTimer.onEnd -= ShowNextStep;
        PopupPanel.Instance.PlayTimeUp(() =>
           {
               finishPanel.SetActive(true);
               //    UIManager.Instance.SetState(UIManager.UIState.Stage_4);
           });
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
        if (steps[currentStep].Contains(foodName) == true)
        {
            Debug.LogWarning("Step Contains Food :" + foodName);
            ShowNextStep();
            AudioManager.Instance.PlayAudioOnce(4);
        }
    }
}
