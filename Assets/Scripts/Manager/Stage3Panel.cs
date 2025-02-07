using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    void Start()
    {
        Init();
    }

    public void Init()
    {
        // nextDescBtn.onClick.AddListener(StartGame);
        descObj.SetActive(true);
        foreach (var item in descItem)
        {
            item.SetActive(false);
        }
        descItem.First().SetActive(true);
        nextDescBtn.onClick.AddListener(ShowNextDesc);

        gameObj.SetActive(false);
        foodItems = gameObj.transform.Find("Food/PickupFoods").GetComponentsInChildren<FoodItem>().ToList();
        foodSprites = gameObj.transform.Find("Food/PickupFoods").GetComponentsInChildren<PickItemToBucket>().ToList();
        PickItemToBucket();

        btnImage.sprite = normalSprite;

        foreach (var item in foodSpriteList)
        {
            item.SetActive(false);
        }
    }

    private void ShowNextDesc()
    {
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

        countdownTimer.StartTimer();
        countdownTimer.onEnd += () =>
        {
            ShowNextStep();
        };
    }

    public void ShowNextStep()
    {
        if (countdownTimer.IsRunning)
        {
            countdownTimer.StopTimer();
        }

        foodSpriteList[currentStep].SetActive(true);

        currentStep++;

        if (currentStep >= steps.Count) // 結束步驟
        {
            countdownTimer.StopTimer();
            countdownTimer.onEnd -= ShowNextStep;
            UIManager.Instance.SetState(UIManager.UIState.Stage_4);
            return;
        }
        if (currentStep < steps.Count)
        {
            countdownTimer.StopTimer();
        }
        cookbookStepText.text = steps[currentStep];
        stepTitleImage.sprite = stepTitleSprites[currentStep];
        countdownTimer.StartTimer();
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
    }

    public void OnTriggerFoodItem(string foodName)
    {
        Debug.LogWarning(foodName);
        if (steps[currentStep].Contains(foodName) == true)
        {
            Debug.LogWarning("Step Contains Food :" + foodName);
            ShowNextStep();
        }
    }
}
