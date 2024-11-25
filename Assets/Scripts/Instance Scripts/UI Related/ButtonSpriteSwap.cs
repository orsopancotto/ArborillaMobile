using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonSpriteSwap : Button
{
    [SerializeField] private Sprite selected_sprite, deselected_sprite;
    private Image image_component;

    protected override void Start()
    {
        image_component = GetComponent<Image>();

        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            interactable = false;
            GetComponentsInChildren<Image>(true)[1].gameObject.SetActive(true);     //maschera sovrapposta al bottone attivata
        }
    }

    public override void OnSelect(BaseEventData eventData)
    {
        ButtonsSwitch.SwapSelectedButton(this);

        image_component.sprite = selected_sprite;
    }

    /// <summary>
    /// Imposta lo sprite di deselezione al pulsante
    /// </summary>
    internal void SetAsDeselected()
    {
        image_component.sprite = deselected_sprite;
    }
}
