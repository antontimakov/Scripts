// Application/Shop.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Catatonia.Application;
public class Shop
{
    private Main mainObj;

    /// <summary>
    /// Ссылка на модальное окно магазина
    /// </summary>
    public GameObject wShop;

    /// <summary>
    /// Ссылка на панель магазина
    /// </summary>
    public GameObject pShop;

    /// <summary>
    /// Ссылка на первую кнопки магазина
    /// </summary>
    public List<Button> shopButtons;

    /// <summary>
    /// Ссылка на кнопку закрытия модального окна магазина
    /// </summary>
    public Button bCloseShop;

    /// <summary>
    /// Экземпляр класса ActiveItem
    /// </summary>
    public ActiveItem activeItemObj;

    /// <summary>
    /// 
    /// </summary>
    public Sprite sGrass;
    public Shop(Main mainObj)
    {
        this.mainObj = mainObj;
        wShop = GameObject.Find("ModalWindowShop");
        pShop = GameObject.Find("PanelShop");
        shopButtons = new List<Button>
        {
            GameObject.Find("ShopButton1").GetComponent<Button>(),
            GameObject.Find("ShopButton2").GetComponent<Button>()
        };
        bCloseShop = GameObject.Find("ButtonCloseShop").GetComponent<Button>();
        hideShop();
        bCloseShop.onClick.AddListener(hideShop);
    }
    public void showShop() {
        wShop.SetActive(true);
        
        string[] spriteNames = { "magic_plant", "tomato" };
        int i = 0;

        foreach (Button btn in shopButtons)
        {
            string spritePath = "Sprites/" + spriteNames[i++];
            Sprite CurrentSprite = Resources.Load<Sprite>(spritePath);
            if (CurrentSprite != null)
            {
                btn.GetComponent<Image>().sprite = CurrentSprite;
                btn.onClick.AddListener(() => {SetActiveItem(CurrentSprite);});
            }
        }
    }

    private void hideShop() {
        wShop.SetActive(false);
    }
    private void SetActiveItem(Sprite CurrentSprite)
    {
        activeItemObj.SetActiveItem(CurrentSprite);
        hideShop();
    }
}
