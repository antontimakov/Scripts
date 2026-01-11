// Application/Shop.cs
using UnityEngine;
using UnityEngine.UI;

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
    /// Ссылка на первую кнопку магазина
    /// </summary>
    public Button bShop1;

    /// <summary>
    /// Ссылка на вторую кнопку магазина
    /// </summary>
    public Button bShop2;

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
        bShop1 = GameObject.Find("ShopButton1").GetComponent<Button>();
        bShop2 = GameObject.Find("ShopButton2").GetComponent<Button>();
        bCloseShop = GameObject.Find("ButtonCloseShop").GetComponent<Button>();
        hideShop();
        bCloseShop.onClick.AddListener(hideShop);
    }
    public void showShop() {
        wShop.SetActive(true);
        Sprite sGround = Resources.Load<Sprite>("Sprites/ground");
        bShop1.GetComponent<Image>().sprite = sGrass;
        bShop2.GetComponent<Image>().sprite = sGround;
        bShop1.onClick.AddListener(SetActiveItem);
    }

    private void hideShop() {
        wShop.SetActive(false);
    }
    private void SetActiveItem()
    {
        activeItemObj.SetActiveItem();
        hideShop();
    }
}
