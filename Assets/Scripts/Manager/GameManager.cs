using System.Collections.Generic;
using UnityCommunity.UnitySingleton;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public int CurrentCookBookIndex = -1;
    public CookBookInfo CurrentCookBookInfo = null;
    private List<bool> pickedFoods = new List<bool>();
    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        CurrentCookBookIndex = -1;
    }

    public void ResetStatus()
    {
        CurrentCookBookIndex = -1;
    }

    // private void SetPickedFoods()
    // {
    //     pickedFoods.Clear();
    //     for (int i = 0; i < CurrentCookBookInfo.foods.Count; i++)
    //     {
    //         pickedFoods.Add(false);
    //     }
    // }
}
