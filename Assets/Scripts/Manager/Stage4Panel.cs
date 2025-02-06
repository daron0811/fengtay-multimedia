using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Stage4Panel : MonoBehaviour
{
    [Header("描述說明")]
    public GameObject descObj;
    public List<GameObject> descItem;
    public Button nextDescBtn;

    [Header("成績表")]
    public GameObject detailPanel;
    private List<FoodItem> foodItems; //從子物件下取得
    private List<FoodItem> otherFoodItems; //從子物件下取得

    public TextMeshProUGUI cookbookNameText;
    void Start()
    {
        Init();
    }

    void Init()
    {
        descObj.SetActive(true);
        foreach (var item in descItem)
        {
            item.SetActive(false);
        }
        descItem.First().SetActive(true);
        nextDescBtn.onClick.AddListener(ShowNextDesc);

        detailPanel.SetActive(false);

        foodItems = detailPanel.transform.Find("PickupFoods").GetComponentsInChildren<FoodItem>().ToList();
        otherFoodItems = detailPanel.transform.Find("OtherFoods").GetComponentsInChildren<FoodItem>().ToList();

        SetCookBookInfo();
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
            detailPanel.SetActive(true);
            // countdownTimer.StartTimer(30.0f);
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

}
