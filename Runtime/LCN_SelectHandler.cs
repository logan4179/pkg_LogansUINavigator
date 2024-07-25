using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace LogansUINavigator
{
    public class LCN_SelectHandler : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private bool amDebuggingScript;

        [Header("[------------------INIT------------------]")]
        public UnityEvent OnInit;
        [Header("[------------------SELECT------------------]")]
        public UnityEvent OnSelect_UserAdded;
        [Header("[------------------DESELECT------------------]")]
        public UnityEvent OnDeSelect_UserAdded;

        private void Start()
        {
            OnInit.Invoke();
            OnInit.RemoveAllListeners();
        }

        public void OnSelect(BaseEventData eventData) //This event isn't fired from "clicking" the button like it sounds, but rather, when something makes this button the event system's selected object, which is commonly used to highlight the button, for example.
        {
            OnSelect_UserAdded.Invoke();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            OnDeSelect_UserAdded.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            EventSystem.current.SetSelectedGameObject(gameObject); //This forces the eventsystem to consider this object "selected" upon mouse over. Doing this will also indirectly call OnSelect()
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //PV_Debug.LogWithConsoleConditional( $"OnPointerExit('{gameObject.name}')", amDebuggingScript, PV_LogFormatting.UserMethod );
        }
    }
}