using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace LogansUINavigator
{
    public class LCN_Action : MonoBehaviour
    {
        [SerializeField] private LCN_Manager _mgr;
        public LCN_Manager Manager 
        { 
            get{ return _mgr; }
            set { _mgr = value; } 
        }
        [SerializeField] private LCN_Listener myListener;

        public bool AmActive { get; set; } = true;

        [Tooltip("Keycodes that will trigger selecting this object.")]
        public List<KeyCode> KeyCodes;

        private bool hasInputs;
        /// <summary>Quickly and efficiently tells interested scripts if this LCN_Action has any keycode inputs.</summary>
        public bool HasInputs => hasInputs;
 
        //[Space(20)]

        //[Header("[-------------- INIT --------------]")]
        public UnityEvent OnInit;

        //[Header("[-------------- EXECUTE --------------]")]
        public UnityEvent OnExecute;
        [SerializeField] private LCN_Listener listenOnExecute;
        [Tooltip("Default GameObject that gets selected upon execution of this action.")]
        [SerializeField] private GameObject selectOnExecute;

        [Tooltip("This simply functions as user-supplied description to remind the user what this action does.")]
        public string description;

        [Header("[-------------- DEBUGGING --------------]")]
        public bool AmDebuggingScript;
        public RectTransform rt, RT_pointTo;

        private void Start( )
        {
            Init();

            OnInit.Invoke();
            OnInit.RemoveAllListeners();
        }

        [ContextMenu("Call Init()")]
        public void Init()
        {
            hasInputs = false;
            if (KeyCodes != null && KeyCodes.Count > 0)
            {
                hasInputs = true;
            }

            if (myListener != null)
            {
                myListener.AddAction(this);
            }

            if (_mgr != null)
            {
                _mgr.AddAction(this);
            }
            else
            {
                Debug.LogWarning($"LDC WARNING! This action: '{name}' doesn't have a manager assigned, so it won't be visualized with a gizmo.");
            }

            if (!TryGetComponent<RectTransform>(out rt))
            {
                Debug.LogWarning($"LDC WARNING! Looks like an action was put on an object without a rect transform. Won't be able to draw icon for it...");
            }
        }

        public void ActivateMe()
        {
            AmActive = true;
        }
        public void DeactivateMe()
        {
            AmActive = false;
        }

        /// <summary>
        /// Checks to see if any of the keycodes belonging to this action are being pressed.
        /// </summary>
        /// <returns>true if a keycode is currently being pressed, false if not.</returns>
        public bool WasMyKeycodePressed()
        {
            bool dbg = AmDebuggingScript;
            if (dbg) print($"CheckForKeycode() for subscribed InputListener: '{gameObject}'");

            foreach (KeyCode key in KeyCodes)
            {
                if (Input.GetKey(key))
                {
                    if (dbg) print($"    valid key found: '{key}'. AmEntered false, invoking OnEnter...");
                    return true;
                }
            }
            return false;
        }

        [ContextMenu("call Execute")]
        public void Execute()
        {
            bool dbg = AmDebuggingScript;
            if (dbg) print($"{gameObject}.Execute( selectOnExecute: '{(selectOnExecute == null ? "null" : selectOnExecute.name)}', " +
                $"listenOnExecute: '{(listenOnExecute == null ? "null" : listenOnExecute.name)}')");

            OnExecute.Invoke();

            if( selectOnExecute != null )
            {
                EventSystem.current.SetSelectedGameObject(selectOnExecute);
            }

            if( listenOnExecute != null )
            {
                _mgr.ChangeCurrentListener(listenOnExecute);
            }
        }

        public void Execute( GameObject selectNext_passed )
        {
            bool dbg = AmDebuggingScript;
            if (dbg) print($"{gameObject}.Execute( selectOnExecute: '{(selectOnExecute == null ? "null" : selectOnExecute.name)}', " +
                $"listenOnExecute: '{(listenOnExecute == null ? "null" : listenOnExecute.name)}')");

            OnExecute.Invoke();

            if( selectNext_passed != null )
            {
                EventSystem.current.SetSelectedGameObject(selectNext_passed);
            }
            else if (selectOnExecute != null)
            {
                EventSystem.current.SetSelectedGameObject(selectOnExecute);
            }

            if (listenOnExecute != null)
            {
                _mgr.ChangeCurrentListener(listenOnExecute); ;
            }
        }

        public void SetSelectOnExecute( GameObject selection_passed )
        {
            selectOnExecute = selection_passed;
        }
    }
}