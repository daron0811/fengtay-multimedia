using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class FoodItem : MonoBehaviour
{
    public String myFoodName = "";
    public Image foodSprite;
    public TextMeshProUGUI foodNameText;
    public TextMeshProUGUI foodLocalText;
    public Toggle toggle;

    public Image background;
    public Image imgCheckMark;

    private FoodInfo foodInfo;

    private Material imageMat;

    public GameObject particle;

    public void Awake()
    {
        if (foodSprite.material != null)
        {
            imageMat = new Material(foodSprite.material); // 產生材質的副本
            foodSprite.material = imageMat;
            // imageMat = foodSprite.material;
        }
        ShowParticle();
    }

    public void ShowParticle(bool isShow = false)
    {
        if (particle != null)
        {
            particle.gameObject.SetActive(isShow);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ShowCheckMark(bool isShow)
    {
        if (imgCheckMark == null)
        {
            return;
        }
        imgCheckMark.gameObject.SetActive(isShow);
    }

    public void SetFoodItem(Sprite sprite, string name = "", string local = "", bool isGray = false, bool isBGGray = false)
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
        if (foodLocalText != null)
        {
            foodLocalText.text = local;
        }
        if (toggle != null)
        {
            toggle.isOn = false;
        }
        SetBGGray(isBGGray);

        myFoodName = name;
        if (imageMat == null)
        {
            if (foodSprite.material != null)
            {
                imageMat = new Material(foodSprite.material); // 產生材質的副本
                foodSprite.material = imageMat;
            }
        }

        if (imageMat != null)
        {
            if (isGray == false)
            {
                imageMat.SetFloat("_Grayscale", 1);
            }
            else
            {
                imageMat.SetFloat("_Grayscale", 0);
            }
        }
    }

    public void SetFoodGary(bool isGray = false)
    {
        if (imageMat != null)
        {
            if (isGray == false)
            {
                imageMat.SetFloat("_Grayscale", 1);
            }
            else
            {
                imageMat.SetFloat("_Grayscale", 0);
            }
        }
    }

    public void SetBGGray(bool isBGGray)
    {
        if (background != null)
        {
            background.color = isBGGray ? new Color32(192, 192, 192, 255) : new Color32(255, 255, 255, 255);
        }
    }

    public void Checked(string foodName)
    {
        if (toggle.isOn)
        {
            return;
        }
        if (foodInfo.name == foodName)
        {
            toggle.isOn = true;
        }
    }

    public void SetGary(bool value = true)
    {
        if (foodSprite.material == null)
        {
            return;
        }
        if (imageMat == null)
        {
            if (foodSprite.material != null)
            {
                imageMat = new Material(foodSprite.material); // 產生材質的副本
                foodSprite.material = imageMat;
            }
        }
        if (imageMat != null)
        {
            imageMat.SetFloat("_Grayscale", value ? 0 : 1);
        }

    }
}
