// Application/Zoom.cs
using UnityEngine;
using UnityEngine.InputSystem;

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
        if (Mouse.current != null)
        {
            // Проверяем, зажата ли левая кнопка мыши
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // Запоминаем начальную позицию мыши
                startMousePos = Mouse.current.position.ReadValue();
            }
            else if (Mouse.current.leftButton.isPressed)
            {
                Vector2 currentPos = Mouse.current.position.ReadValue();
                // Рассчитываем разницу в положении мыши в двухмерных координатах
                Vector2 delta = currentPos - startMousePos;

                // Направление движения камеры (обратное направление мыши)
                Vector2 moveDirection = delta;

                // Смещаем камеру против направления мыши
                cam.transform.Translate(-delta.normalized * panSpeed, Space.World);

                // Обновляем стартовую позицию мыши
                startMousePos = currentPos;
            }


            // Масштабирование через колесо мыши
            Vector2 scrollDelta = Mouse.current.scroll.ReadValue();
            if (scrollDelta.y != 0f)
            {
                float newSize = cam.orthographicSize - scrollDelta.y * zoomSpeed;
                cam.orthographicSize = Mathf.Clamp(newSize, minSize, maxSize);
            }
        }
    }
}
