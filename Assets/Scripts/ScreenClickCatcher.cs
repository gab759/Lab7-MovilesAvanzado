using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenClickCatcher : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private MenuController menuController;

    public void OnPointerClick(PointerEventData eventData)
    {

        if (menuController != null)
            menuController.OnScreenClick();
    }
}
