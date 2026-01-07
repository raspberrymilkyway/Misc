using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace IHatJHat.UI
{
    //source: https://www.youtube.com/watch?v=RpfmzHWNp80
    //unused: https://medium.com/@Code_With_K/creating-a-custom-unity-button-with-editor-extension-2650f2a6abda
    public class LRClickButton : Selectable, IPointerClickHandler{
        public UnityEvent OnLeftClick = new UnityEvent();
        public UnityEvent OnRightClick = new UnityEvent();

        private Coroutine _resetRoutine;

        protected override void Reset(){
            base.Reset();

            var img = GetComponent<UnityEngine.UI.Image>();
            if (img == null){
                img = gameObject.AddComponent<UnityEngine.UI.Image>();
            }

            targetGraphic = img;
        }

        public void OnPointerClick(PointerEventData e){
            DoStateTransition(SelectionState.Pressed, true);

            switch (e.button){
                default:
                case PointerEventData.InputButton.Left:
                    OnLeftClick?.Invoke();
                    break;
                case PointerEventData.InputButton.Right:
                    OnRightClick?.Invoke();
                    break;
                //.Middle and OnMiddleClick? for middle mouse
            }

            if (_resetRoutine != null){
                StopCoroutine(OnFinishSubmit());
            }

            _resetRoutine = StartCoroutine(OnFinishSubmit());
        }

        private IEnumerator OnFinishSubmit(){
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while (elapsedTime < fadeTime){
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }
    }
}