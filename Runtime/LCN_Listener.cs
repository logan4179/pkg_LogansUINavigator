using System.Collections.Generic;
using UnityEngine;

namespace LogansUINavigator
{
    public class LCN_Listener : MonoBehaviour
    {
        [SerializeField] private LCN_Manager _mgr;

        [Tooltip("Makes the manager immediately listen to this listener.")]
        [SerializeField] private bool listenImmediately;
        public bool ListenImmediately => listenImmediately;

        private Dictionary<KeyCode, LCN_Action> keycodesToAction;

        //[Header("[-------------- DEBUGGING --------------]")]
        [HideInInspector] public RectTransform rt;


        private void Start()
        {
            Init();
        }

        [ContextMenu("call Diag()")]
        public void Diag()
        {
            if( keycodesToAction == null || keycodesToAction.Count <= 0 )
            {
                print("no keys or actions logged");
            }
            else
            {
                print($"this listener has: '{keycodesToAction.Count}' keycodeToAction entries.");
                foreach( KeyValuePair<KeyCode, LCN_Action> kvp in keycodesToAction )
                {
                    if( kvp.Value != null )
                    {
                        Debug.Log($"key: '{kvp.Key}', action: '{kvp.Value}', actin desc: '{kvp.Value.description}'");
                    }
                    else
                    {
                        Debug.Log($"key: '{kvp.Key}', action was null");
                    }
                }

            }
        }

        [ContextMenu("Call Init()")]
        public void Init()
        {
            //print($"Init(). mgr null?: '{_mgr == null}'");
            if (_mgr != null)
            {
                _mgr.AddListener(this);
                if (listenImmediately)
                {
                    _mgr.ChangeCurrentListener(this);
                }
            }
            else
            {
                Debug.LogError($"LDC ERROR! You tried to initialize a listener named: '{name}' with no manager!");
            }

            if( !TryGetComponent<RectTransform>(out rt) )
            {
                Debug.LogWarning($"LDC WARNING! Looks like an action was put on an object without a rect transform. Won't be able to draw icon for it...");
            }
        }

        [ContextMenu("call MakeKosher()")]
        public void MakeKosher()
        {
            print($"MakeKosher(), keycodesToActions null?: '{keycodesToAction == null}'");
            if ( keycodesToAction != null )
            {
                print($"keycodesToActions count: '{keycodesToAction.Count}'");
                List<KeyCode> badKeys = new List<KeyCode>();
                foreach ( KeyValuePair<KeyCode,LCN_Action> kvp in keycodesToAction )
                {
                    if ( kvp.Value == null )
                    {
                        badKeys.Add(kvp.Key);
                    }
                }

                foreach ( KeyCode key in badKeys )
                {
                    if (Application.isEditor)
                    {
                        Debug.Log($"removing null keyvaluepair for key: '{key}' in listener: '{name}'...");
                    }
                    keycodesToAction.Remove(key);
                }
            }
        }

        public void AddAction( LCN_Action action_passed )
        {
            if( keycodesToAction == null )
            {
                keycodesToAction = new Dictionary<KeyCode, LCN_Action>();
            }

            if ( !action_passed.HasInputs )
            {
                Debug.LogWarning($"LDC WARNING! You tried to register an LCN_Action ('{action_passed.name}') with an LCN_Listener, but there are no inputs to " +
                $"listen to. Was this intentional? Returning early...");
                return;
            }

            foreach ( KeyCode kc in action_passed.KeyCodes )
            {
                if ( !keycodesToAction.ContainsKey(kc) )
                {
                    keycodesToAction.Add( kc, action_passed );
                }
                else if ( keycodesToAction[kc] == null )
                {
                    keycodesToAction[kc] = action_passed;
                }
                else
                {
                    Debug.LogWarning($"LDC WARNING! You can't add an action at an already existing keycode on this listener.");
                }
            }
        }

        public void RemoveAction( LCN_Action action_passed )
        {
            if( keycodesToAction == null )
            {
                if( Application.isEditor )
                {
                    Debug.Log("Listener had no actions, so there's no need to call RemoveAction(). Returning early...");
                }
                return;
            }

            foreach( KeyCode kc in action_passed.KeyCodes )
            {
                if( keycodesToAction.ContainsKey(kc) && keycodesToAction[kc] == action_passed )
                {
                    keycodesToAction.Remove(kc);
                }
            }
        }

        public LCN_Action GetActionAtCurrentKeycode()
        {
            foreach( KeyValuePair<KeyCode, LCN_Action> kvp in keycodesToAction )
            {
                if( kvp.Value != null && kvp.Value != null )
                {
                    if(kvp.Value.AmActive && kvp.Value.WasMyKeycodePressed() )
                    {
                        return kvp.Value;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Used in cases where you can't trigger this listener the 'normal' way (which would be setting this listener to an LCN_Action's 
        /// 'listenOnExecute' reference so that it happens automatically when this action is triggered), but instead need to force listen 
        /// to this LCN_Listener via a Unity editor event or code.
        /// </summary>
        public void MakeMeCurrentListener()
        {
            _mgr.ChangeCurrentListener(this);
        }


        #region DEBUG ----------------------------------------------------------
        [ContextMenu("call DebugDictionary")]
        public void DebugDictionary()
        {
            print($"keycodesToAction null?: '{keycodesToAction == null}'");
            if(keycodesToAction != null )
            {
                print($"keycodesToAction count: '{keycodesToAction.Count}'");

            }
        }
        #endregion
    }
}