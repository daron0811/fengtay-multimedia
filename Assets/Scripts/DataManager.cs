using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    public TextAsset foodAsset;
    public List<FoodInfo> foodInfos;
    public Dictionary<int, List<FoodInfo>> foodBySeason;

    void Start()
    {
        if (foodAsset != null)
        {
            foodInfos = JsonConvert.DeserializeObject<List<FoodInfo>>(foodAsset.text);
            foodBySeason = new Dictionary<int, List<FoodInfo>>();

            foreach (var foodInfo in foodInfos)
            {
                foreach (char seasonChar in foodInfo.season)
                {
                    int season = int.Parse(seasonChar.ToString());
                    if (!foodBySeason.ContainsKey(season))
                    {
                        foodBySeason[season] = new List<FoodInfo>();
                    }
                    foodBySeason[season].Add(foodInfo);
                }
            }
        }
    }

    public List<FoodInfo> GetFoodBySeason(int season)
    {
        if(foodBySeason==null)
        {
            return null;
        }
        if (foodBySeason.ContainsKey(season))
        {
            return foodBySeason[season];
        }
        return null;
    }
}

[System.Serializable]
public class FoodInfo
{
    public int id;// { get; set; }
    public string name;// { get; set; }
    public string locate; //{ get; set; }
    public string food; //{ get; set; }
    public string season; //{ get; set; }
}

