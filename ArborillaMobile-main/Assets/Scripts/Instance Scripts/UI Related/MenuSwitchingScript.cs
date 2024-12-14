using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class MenuSwitchingScript : MonoBehaviour
{
    [SerializeField] private GameObject scroll_menu_canvas, pollen_scroll_menu, fruits_scroll_menu, fruits_btn, pollen_btn;

    private void Start()
    {
        fruits_btn.GetComponent<Button>().onClick.AddListener(delegate
        {
            ChangeInventoryMenu(fruits_scroll_menu);
        });

        pollen_btn.GetComponent<Button>().onClick.AddListener(delegate
        {
            ChangeInventoryMenu(pollen_scroll_menu);
        });
    }

    private void ChangeInventoryMenu(GameObject menu_to_open)
    {
        scroll_menu_canvas.GetComponentInChildren<ScrollRect>().gameObject.SetActive(false);

        menu_to_open.SetActive(true);
    }

}
