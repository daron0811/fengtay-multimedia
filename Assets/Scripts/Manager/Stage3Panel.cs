using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.UI;

public class Stage3Panel : MonoSingleton<Stage3Panel>
{
    public GameObject descObj;
    public List<GameObject> descItem;
    public Button nextDescBtn;
    public List<FoodItem> foodItems;
    public CountdownTimer countdownTimer;

    void Start()
    {
        Init();
    }

    public void Init()
    {
    
    }

    public void SetFoodItems(int cookbookIndex)
    {
      
    }

    private void ShowNextDesc()
    {
       
    }

    public void OnTriggerFoodItem(string foodName)
    {
        
    }
}
