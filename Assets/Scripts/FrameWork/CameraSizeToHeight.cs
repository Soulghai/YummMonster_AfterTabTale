using UnityEngine;

/// <summary>
/// Скрипт подгоняющий размер камеры под указанную высоту в пикселях
/// (Пример: есть спрайт 600пкс в высоту. Размер экрана 300пкс. Камера будет видеть половину размера, поэтому увеличиваем Size x2)
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraSizeToHeight : MonoBehaviour
{
    public float sizeMultiplier = 1f;

    // Значение pixelPerUnit для данного спрайта
    public float pixelPerUnit = 100f;

    public float spriteHeight = 100f;

    public bool calculateSize = true;

	void Awake ()
	{
        CalculateSize();
	}

    void CalculateSize()
    {
        if (!calculateSize) return;

        GetComponent<Camera>().orthographicSize = sizeMultiplier * spriteHeight / (2f * pixelPerUnit);
        calculateSize = false;
    }
}
