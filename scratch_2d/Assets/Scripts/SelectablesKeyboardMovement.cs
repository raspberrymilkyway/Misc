// TabNextSelection.cs
// An adaptation of an adaptation by halley from a forum posting by SirRogers:
//   https://discussions.unity.com/t/tab-between-input-fields/547817/95

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// supports using tab between selectables (requried)
// supports using shift tab to go backwards between selectables (optional)
// supports using arrow keys between selectables (optional)

public class TabNextSelection: MonoBehaviour
{
    [Tooltip("Also support Shift+Tab to move backwards to prior selection")]
    public bool backtab = true;
    [Tooltip("Also supports arrow key movement")]
    public bool arrowkeys = true;

    private EventSystem system;

    private void OnEnable()
    {
        system = EventSystem.current;
    }

    private bool WasTabPressed(){
        return Keyboard.current.tabKey.wasPressedThisFrame;
    }
    private bool IsShiftPressed(){
        return Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
    }

    private bool WasUpPressed(){
        return Keyboard.current.upArrowKey.wasPressedThisFrame;
    }
    private bool WasDownPressed(){
        return Keyboard.current.downArrowKey.wasPressedThisFrame;
    }
    private bool WasLeftPressed(){
        return Keyboard.current.leftArrowKey.wasPressedThisFrame;
    }
    private bool WasRightPressed(){
        return Keyboard.current.rightArrowKey.wasPressedThisFrame;
    }

    private Selectable PriorSelectable(Selectable current){
        Selectable prior = current.FindSelectableOnLeft();
        if (prior == null)
            prior = current.FindSelectableOnUp();
        return prior;
    }
    private Selectable NextSelectable(Selectable current){
        Selectable next = current.FindSelectableOnRight();
        if (next == null)
            next = current.FindSelectableOnDown();
        return next;
    }

    private Selectable UpSelectable(Selectable current){
        return current.FindSelectableOnUp();
    }
    private Selectable DownSelectable(Selectable current){
        return current.FindSelectableOnDown();
    }
    private Selectable LeftSelectable(Selectable current){
        return current.FindSelectableOnLeft();
    }
    private Selectable RightSelectable(Selectable current){
        return current.FindSelectableOnRight();
    }

    private void Update()
    {
        if (system == null)
            return;

        GameObject selected = system.currentSelectedGameObject;
        if (selected == null)
            return;

        bool arrow = true;
        bool upa = false;
        bool downa = false;
        bool lefta = false;
        bool righta = false;
        if (arrowkeys){
            upa = WasUpPressed();
            downa = WasDownPressed();
            lefta = WasLeftPressed();
            righta = WasRightPressed();
            if (!upa && !downa && !lefta && !righta){
                arrow = false;
            }
        }
        else{
            arrow = false;
        }
        if (!WasTabPressed() && !arrow)
            return;

        Selectable current = selected.GetComponent<Selectable>();
        if (current == null)
            return;
        
        Selectable next = null;
        if (!arrow){
            bool up = IsShiftPressed();
            next = up? PriorSelectable(current) : NextSelectable(current);
            // Wrap from end to beginning, or vice versa.
            if (next == null)
            {
                next = current;
                Selectable pnext;
                if (up)
                    while ((pnext = NextSelectable(next)) != null)
                        next = pnext;
                else
                    while ((pnext = PriorSelectable(next)) != null)
                        next = pnext;
            }
        }
        else{
            if (upa){
                next = UpSelectable(current);
            }
            else if (downa){
                next = DownSelectable(current);
            }
            else if (lefta){
                next = LeftSelectable(current);
            }
            else if (righta){
                next = RightSelectable(current);
            }
        }

        if (next == null)
            return;

        // Simulate mouse click for InputFields.
        InputField inputfield = next.GetComponent<InputField>();
        if (inputfield != null)
            inputfield.OnPointerClick(new PointerEventData(system));

        // Select the next item in the tab-order of our direction.
        system.SetSelectedGameObject(next.gameObject);
    }
}