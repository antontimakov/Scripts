// Application/ActiveItem.cs
using UnityEngine;
using UnityEngine.UI;

namespace Catatonia.Application;
public class ActiveItem
{
    private Main mainObj;

    /// <summary>
    /// Ссылка на объект активного в данный момент элемента
    /// </summary>
    public GameObject oActiveItem;

    /// <summary>
    /// Ссылка на кнопку активного в данный момент элемента
    /// </summary>
    private Button bActiveItem;

    /// <summary>
    /// 
    /// </summary>
    public Sprite sGrass;
    public ActiveItem(Main mainObj)
    {
        this.mainObj = mainObj;
        oActiveItem = GameObject.Find("ActiveItemButton");
        bActiveItem = oActiveItem.GetComponent<Button>();
    }

    private void showActiveItem() {
        oActiveItem.SetActive(true);
    }

    public void hideActiveItem() {
        oActiveItem.SetActive(false);
    }
    public void SetActiveItem()
    {
        showActiveItem();
        // TODO вынести в параметр
        bActiveItem.GetComponent<Image>().sprite = sGrass;
    }
}
