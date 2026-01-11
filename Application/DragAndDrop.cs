// DragAndDrop.cs
using System;
using UnityEngine;
using Catatonia;

namespace Catatonia.Application
{
    class DragAndDrop
    {
        State state;
        GameObject item;
        Vector2 offset;
        Vector2 nullVector;
        Vector2 offsetCell;
        Main mainOo;
        float cellSize = 2f; // размер клетки

        public DragAndDrop(Main mainO)
        {
            state = State.none;
            item = null;
            nullVector = Camera.main.ScreenToWorldPoint(new Vector2(0f, Screen.height));
            offsetCell = new Vector2(1.1f, -1.1f);
            mainOo = mainO;
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
            return Input.GetMouseButton(0);
        }

        Vector2 getClickPosition()
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
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

            // заменяет элемент по которому кликнули на траву
            //mainOo.chObj(mainOo.grassPrefab, item);
            //mainOo.setServerWin(item.transform);
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