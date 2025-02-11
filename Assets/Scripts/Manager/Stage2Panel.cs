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

    public Image btnImage;
    public Sprite normalSprite;
    public Sprite goSprite;

    public bool isInit = false;
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
        foodItems = transform.Find("PickupFoods").GetComponentsInChildren<FoodItem>().ToList();
        dragFoodsOnMap = transform.Find("FoodGroup").GetComponentsInChildren<PickItemToBucket>().ToList();

        countdownTimer.onEnd += () =>
        {
            UIManager.Instance.SetState(UIManager.UIState.Stage_3);
        };

        ResetStatus();
        isInit = true;
    }

    void OnEnable()
    {
        if (isInit == false)
        {
            Init();
        }
        else
        {
            ResetStatus();
        }
    }

    public void ResetStatus()
    {
        descObj.SetActive(true);
        foreach (var item in descItem)
        {
            item.SetActive(false);
        }
        descItem.First().SetActive(true);
        btnImage.sprite = normalSprite;
        PickItemToBucket();

        SetCookBookInfo();
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
        if (GameManager.Instance.CurrentCookBookIndex < 0)
        {
            Debug.LogWarning("CurrentCookBookIndex < 0");
            cookbookNameText.text = "未知";
            return;
        }
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
