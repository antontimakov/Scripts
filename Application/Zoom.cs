// Application/Zoom.cs
using UnityEngine;

namespace Catatonia.Application;
public class Zoom
{
    private Main mainObj;
    private float panSpeed; // Скорость передвижения камеры
    private float zoomSpeed; // Скорость изменения размеров
    private float minSize;   // Минимальный размер камеры
    private float maxSize;  // Максимальный размер камеры

    private Vector2 startMousePos; // Начальная позиция мыши при зажиме кнопки
    public Camera cam;
    public Zoom(Main mainObj)
    {
        panSpeed = 0.3f; // Скорость передвижения камеры
        zoomSpeed = 0.4f; // Скорость изменения размеров
        minSize = 1f;   // Минимальный размер камеры
        maxSize = 20f;  // Максимальный размер камеры
        this.mainObj = mainObj;
        cam = GameObject.Find("MainCamera").GetComponent<Camera>();
    }
    public void zoomGameField()
    {
        // Проверяем, зажата ли левая кнопка мыши
        if (Input.GetMouseButtonDown(0))
        {
            // Запоминаем начальную позицию мыши
            startMousePos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            // Рассчитываем разницу в положении мыши в двухмерных координатах
            Vector2 delta = (Vector2)Input.mousePosition - startMousePos;

            // Направление движения камеры (обратное направление мыши)
            Vector2 moveDirection = delta;

            // Смещаем камеру против направления мыши
            cam.transform.Translate(-moveDirection.normalized * panSpeed, Space.World);

            // Обновляем стартовую позицию мыши
            startMousePos = Input.mousePosition;
        }

        float scrollValue = Input.mouseScrollDelta.y;

        if (scrollValue != 0)
        {
            // Изменяем ортографический размер камеры
            cam.orthographicSize -= scrollValue * zoomSpeed; //  * Time.deltaTime
            // Ограничиваем диапазон размеров
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minSize, maxSize);
        }
    }
}
