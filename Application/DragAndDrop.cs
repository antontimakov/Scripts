// DragAndDrop.cs
using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem;
using Catatonia;
using Catatonia.Application.Models;

namespace Catatonia.Application
{
    class DragAndDrop
    {
        State state;
        GameObject item;
        Vector2 offset;
        Vector2 nullVector;
        Vector2 offsetCell;
        Main mainObj;

        /// <summary>
        /// Экземпляр класса GameField
        /// </summary>
        GameField gameFieldObj;
        float cellSize = 2f; // размер клетки

        public DragAndDrop(Main mainObj, GameField gameFieldObj)
        {
            state = State.none;
            item = null;
            nullVector = Camera.main.ScreenToWorldPoint(new Vector2(0f, Screen.height));
            offsetCell = new Vector2(1.1f, -1.1f);
            this.mainObj = mainObj;
            this.gameFieldObj = gameFieldObj;
        }
        public void Action()
        {
            switch (state)
            {
                case State.none:
                    if (isMouseButtonPressed())
                    {
                        pickup();
                    }
                    // Наводим мышь
                    else
                    {
                        // TODO вынести в отдельный метод
                        Vector2 currentPosition = getClickPosition();
                        Transform currentItem = GetItemAt(currentPosition);
                        if (currentItem == null)
                        {
                            mainObj.mIf.text = "";
                            return;
                        }
                        DataDb dataDbObj = currentItem.GetComponent<DataDb>();
                        if (dataDbObj != null){
                            ElemModel serverData = dataDbObj.serverData;
                            if (serverData != null && serverData.updated_modefied.HasValue)
                            {
                                DateTime increasedUpdated = serverData.updated_modefied.Value.AddSeconds(serverData.elem_lifetime);
                                if (increasedUpdated > DateTime.UtcNow)
                                {
                                    //mainObj.mIf.text = (increasedUpdated - DateTime.UtcNow).ToString();
                                    var deltaTime = increasedUpdated - DateTime.UtcNow;
                                    mainObj.mIf.text = string.Format("{0:D2}h:{1:D2}m:{2:D2}s", 
                                        deltaTime.Hours, 
                                        deltaTime.Minutes, 
                                        deltaTime.Seconds);

                                }
                            }
                            else
                            {
                                mainObj.mIf.text = "";
                            }
                        }
                        else
                        {
                            mainObj.mIf.text = "";
                        }
                    }
                    break;

                case State.drag:
                    if (isMouseButtonPressed())
                    {
                        drag();
                    }
                    else
                    {
                        drop();
                    }
                    break;
            }
        }

        bool isMouseButtonPressed()
        {
            return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        }

        Vector2 getClickPosition()
        {
            return Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        }

        Transform GetItemAt(Vector2 position)
        {
            RaycastHit2D[] figuries = Physics2D.RaycastAll(position, position, 1f);
            if (figuries.Length > 0)
            {
                return figuries[0].transform;
            }
            return null;
        }
        void pickup()
        {
            Vector2 clickPosition = getClickPosition();
            Transform clickedItem = GetItemAt(clickPosition);
            if (clickedItem == null)
            {
                return;
            }
            item = clickedItem.gameObject;
            //UnityEngine.Debug.Log(item.name);
            //offset = (Vector2)clickedItem.position - clickPosition;
            state = State.drag;
        }

        void drag()
        {
            //item.transform.position = getClickPosition() + offset;
        }
        void drop()
        {
            /*BoxCollider2D boxCollider = item.GetComponent<BoxCollider2D>();
            float height = boxCollider.size.y * item.transform.localScale.y;
            float width = boxCollider.size.x * item.transform.localScale.x;
            nullVector.y -= height / 2;
            nullVector.x += width / 2;
            item.transform.position = getNearestCell(item.transform.position);

            item.transform.position = calcCellPosition(getClickPosition());
            item = null;*/

            //mainObj.chObj(mainObj.grassPrefab, item);
            gameFieldObj.defineClickAction(item);
            state = State.none;
        }

        Vector2 calcCellPosition(Vector2 ItemPosition)
        {
            // было
            //return ItemPosition + offset;
            Vector2 cp = getClickPosition();
            Vector2 res = nullVector;
            for (int i = 0; i < 7; ++i)
            {
                if (cp.x > (res.x/* + offset.x*/))
                {
                    res.x += offsetCell.x;
                }
                if (cp.y < (res.y/* + offset.y*/))
                {
                    res.y += offsetCell.y;
                }
            }
            return res;
        }

        enum State
        {
            none,
            //pick,
            drag//,
                //drop
        }

        public Vector2 getNearestCell(Vector2 position)
        {
            return new Vector2(
                Mathf.Round(position.x / cellSize) * cellSize,
                Mathf.Round(position.y / cellSize) * cellSize);
        }
    }
}