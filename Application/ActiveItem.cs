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
    /// наименование активного спрайта
    /// </summary>
    public string ActiveSpriteName { get; set; }
    public ActiveItem(Main mainObj)
    {
        this.mainObj = mainObj;
        oActiveItem = GameObject.Find("ActiveItemButton");
        bActiveItem = oActiveItem.GetComponent<Button>();
        bActiveItem.onClick.AddListener(hideActiveItem);
        hideActiveItem();
    }

    private void showActiveItem() {
        oActiveItem.SetActive(true);
    }

    public void hideActiveItem() {
        oActiveItem.SetActive(false);
    }
    public void SetActiveItem(Sprite CurrentSprite, string spriteName)
    {
        ActiveSpriteName = spriteName;
        showActiveItem();
        bActiveItem.GetComponent<Image>().sprite = CurrentSprite;
    }
}
