using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeManager : MonoBehaviour
{
    public Text recipeText; // UI顯示食譜文字
    public Button porkButton, potatoButton, carrotButton, cornButton, potButton; // 各按鈕

    private int currentStep = 0;
    private List<string> steps = new List<string>()
    {
        "鍋中放入橄欖油，加入<color=#FFFF00>豬肉</color> 拌炒。",
        "豬肉半熟時，依序放入<color=#FFFF00>馬鈴薯</color>  、<color=#FFFF00>胡蘿蔔</color>  、<color=#FFFF00>玉米</color>  等食材一起拌炒。",
        "加入水燉煮20分鐘。",
        "放入鹽、黑胡椒及蔥花。"
    };
    
    private List<string> conditions = new List<string>()
    {
        "豬",
        "馬鈴薯,五彩胡蘿蔔,玉米",
        "tap",
        "null"
    };

    private string[] currentIngredients;
    private int ingredientIndex = 0;

    void Start()
    {
        UpdateRecipeText();
        porkButton.onClick.AddListener(() => CheckCondition("豬"));
        potatoButton.onClick.AddListener(() => CheckCondition("馬鈴薯"));
        carrotButton.onClick.AddListener(() => CheckCondition("五彩胡蘿蔔"));
        cornButton.onClick.AddListener(() => CheckCondition("玉米"));
        potButton.onClick.AddListener(() => CheckCondition("tap"));
    }

    void CheckCondition(string input)
    {
        if (conditions[currentStep] == input)
        {
            NextStep();
            return;
        }
        
        if (conditions[currentStep].Contains(",")) // 若條件為多個食材，則拆解
        {
            if (currentIngredients == null || currentIngredients.Length == 0)
            {
                currentIngredients = conditions[currentStep].Split(',');
                ingredientIndex = 0;
            }
            
            if (input == currentIngredients[ingredientIndex])
            {
                ingredientIndex++;
                if (ingredientIndex >= currentIngredients.Length)
                {
                    NextStep();
                }
            }
            else
            {
                return; // 按錯順序則直接跳出，保持當前進度
            }
        }
    }

    void NextStep()
    {
        if (currentStep < steps.Count - 1)
        {
            currentStep++;
            UpdateRecipeText();
            currentIngredients = null; // 重置當前步驟條件
            ingredientIndex = 0;
            
            if (conditions[currentStep] == "null")
            {
                Invoke("NextStep", 2f); // 2秒後自動執行下一步
            }
        }
    }

    void UpdateRecipeText()
    {
        recipeText.text = steps[currentStep];
    }
}
