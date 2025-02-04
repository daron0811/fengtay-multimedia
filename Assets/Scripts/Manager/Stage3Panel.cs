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
    public GameObject gameObj;
    public Button nextDescBtn;
    public CountdownTimer countdownTimer;
    public List<FoodItem> foodItems;

    public TextMeshProUGUI cookbookNameText;
    public TextMeshProUGUI cookbookStepText;

    public List<string> steps;
    public int currentStep = 0;

    public Image stepTitleImage;
    public List<Sprite> stepTitleSprites;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        nextDescBtn.onClick.AddListener(StartGame);
        descObj.SetActive(true);
        gameObj.SetActive(false);
        foodItems = gameObj.transform.Find("Food/PickupFoods").GetComponentsInChildren<FoodItem>().ToList();
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
}
