using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class FoodItem : MonoBehaviour
{
    public Image foodSprite;
    public TextMeshProUGUI foodNameText;
    public Toggle toggle;

    private FoodInfo foodInfo;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetFoodItem(Sprite sprite, string name = "")
    {
        if (foodSprite != null)
        {
            foodSprite.sprite = sprite;
        }
        if (foodNameText != null)
        {
            foodNameText.text = name;
            foodInfo = DataManager.Instance.GetFoodInfo(name);
        }
        if (toggle != null)
        {
            toggle.isOn = false;
        }
    }

    public void Checked(string foodName)
    {
        if(toggle.isOn)
        {
            return;
        }
        if (foodInfo.food == foodName)
        {
            toggle.isOn = true;
        }
    }

}
