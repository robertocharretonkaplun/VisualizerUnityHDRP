using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Controlador de items para drag and drop. Permite spawnear objetos mediante el arrastre y soltado desde la interfaz.
/// </summary>
public class DragDropItemController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int itemID; // ID del item
    private LevelEditor_Manager levelEditor_Manager; // Manager del editor de niveles
    private Canvas canvas; // Canvas principal para las UI
    private RectTransform rectTransform; // RectTransform del item
    private CanvasGroup canvasGroup; // CanvasGroup para controlar la opacidad y la interacci�n
    private Camera mainCamera; // C�mara principal para convertir posiciones de pantalla a mundo

    private Vector2 originalPosition; // Posici�n original del item en la UI

    /// <summary>
    /// Inicializa referencias.
    /// </summary>
    private void Start()
    {
        // Referencia al manager del editor de niveles
        levelEditor_Manager = LevelEditor_Manager.Instance;

        // Obtiene referencias a componentes necesarios
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        mainCamera = Camera.main;

        // Guarda la posici�n original del item
        originalPosition = rectTransform.anchoredPosition;
    }

    /// <summary>
    /// Inicia el arrastre del item.
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Cambia la opacidad para indicar que el objeto est� siendo arrastrado
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Durante el arrastre, mueve el item con el cursor del rat�n.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        // Actualiza la posici�n del RectTransform basado en el movimiento del rat�n
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    /// <summary>
    /// Finaliza el arrastre y spawnea el objeto en el mundo si es soltado en un �rea v�lida.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        // Restablece la opacidad y la interacci�n del objeto
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;

        // Crear un rayo desde la c�mara a la posici�n del mouse
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // Crear un plano en la altura deseada
        Plane plane = new Plane(Vector3.up, new Vector3(0, levelEditor_Manager.PlaneHeight, 0));
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            // Calcular la posici�n en el mundo donde se solt� el objeto
            Vector3 worldPosition = ray.GetPoint(distance);

            // Llama al m�todo en el manager para spawnear el objeto en la posici�n del mundo
            levelEditor_Manager.SpawnItemAtPosition(itemID, worldPosition);
        }

        // Vuelve a la posici�n original en la UI
        rectTransform.anchoredPosition = originalPosition;
    }
}
