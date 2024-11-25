
public static class ButtonsSwitch
{
    private static ButtonSpriteSwap current_selected_btn;

    //internal static void Initialize()  PER RICORDO :)
    //{
    //    foreach(Selectable selectable in Selectable.allSelectablesArray)
    //    {
    //        if (selectable.GetType() == typeof(ButtonSpriteSwap)) current_buttons_in_scene.Add(selectable);
    //    }
    //}

    /// <summary>
    /// Imposta lo sprite di deselezione al bottone precedentemente selezionato
    /// </summary>
    /// <param name="new_selected_btn">Il bottone premuto in questo istante</param>
    internal static void SwapSelectedButton(ButtonSpriteSwap new_selected_btn)
    {
        if(current_selected_btn != null) current_selected_btn.SetAsDeselected();

        current_selected_btn = new_selected_btn;
    }
}
