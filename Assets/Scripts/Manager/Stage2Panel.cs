using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.UI;

public class Stage2Panel : MonoSingleton<Stage2Panel>
{
    public GameObject descObj;
    public List<GameObject> descItem;
    public Button nextDescBtn;
    private List<FoodItem> foodItems; //從子物件下取得
    public CountdownTimer countdownTimer;
    public TextMeshProUGUI cookbookNameText; //食譜名稱

    public List<PickItemToBucket> dragFoodsOnMap;



    void Start()
    {
        Init();
    }

    public void Init()
    {
        descObj.SetActive(true);
        foreach (var item in descItem)
        {
            item.SetActive(false);
        }
        descItem.First().SetActive(true);
        nextDescBtn.onClick.AddListener(ShowNextDesc);
        foodItems = transform.Find("PickupFoods").GetComponentsInChildren<FoodItem>().ToList();

        dragFoodsOnMap = transform.Find("FoodGroup").GetComponentsInChildren<PickItemToBucket>().ToList();
        PickItemToBucket();

        SetCookBookInfo();
        countdownTimer.onEnd += () =>
        {
            UIManager.Instance.SetState(UIManager.UIState.Stage_3);
        };
    }

    void PickItemToBucket()
    {
        if (dragFoodsOnMap == null || dragFoodsOnMap.Count == 0)
        {
            return;
        }

        foreach (PickItemToBucket item in dragFoodsOnMap)
        {
            item.onDragItem += () =>
            {
                Debug.LogWarning(item.name + " dropped on bucket");
                string fruitName = item.name.Split('-')[1];
                OnTriggerFoodItem(fruitName);
            };
        }
    }

    public void SetCookBookInfo()
    {
        SetFoodItems(GameManager.Instance.CurrentCookBookIndex);
        cookbookNameText.text = GameManager.Instance.CurrentCookBookInfo.name;
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
                foodItems[i].SetFoodItem(sprite, foods[i]);
                foodItems[i].Show();
            }
            else
            {
                foodItems[i].Hide();
            }
        }
    }

    private void ShowNextDesc()
    {
        var currentActive = descItem.FirstOrDefault(x => x.activeSelf);
        var currentIndex = descItem.IndexOf(currentActive);
        currentActive.SetActive(false);
        if (currentIndex + 1 < descItem.Count)
        {
            descItem[currentIndex + 1].SetActive(true);
        }
        else
        {
            descObj.SetActive(false);
            countdownTimer.StartTimer(30.0f);
        }
    }

    public void OnTriggerFoodItem(string foodName)
    {
        int foodIndex = -1;
        if (DataManager.Instance.haveFoodbyCookbook(GameManager.Instance.CurrentCookBookIndex, foodName, out foodIndex))
        {
            foodItems[foodIndex - 1].Checked(foodName);
        }
    }
}
