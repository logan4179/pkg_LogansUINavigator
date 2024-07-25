using System.Collections.Generic;
using UnityEngine;

namespace LogansUINavigator
{
    public class LCN_Manager : MonoBehaviour
    {
        /// <summary>This gets set in the start if any keycodes are assigned. Efficient way for this script to decide if it should listen for keystrokes.</summary>
        public bool HasKeycodes;
        /// <summary>This allows external scripts to selectively turn on and off the ability of this navigator to allow input.</summary>
        public bool AmCurrentlyAllowingInput { get; set; } = true;

        public LCN_Listener currentListener;

        [SerializeField] private List<LCN_Action> LCN_actions_all;

        [SerializeField] private List<LCN_Listener> lcnListeners_all;

        [Header("[-------------- DEBUGGING --------------]")]
        [SerializeField] private bool amDebuggingScript;

        private void Awake()
        {
            LCN_actions_all = new List<LCN_Action>();
            lcnListeners_all = new List<LCN_Listener>();
            HasKeycodes = false;
        }

        private void Update()
        {
            if ( !HasKeycodes || !AmCurrentlyAllowingInput ) return;

            if ( Input.anyKeyDown )
            {
                //print("anykeydown");
                LCN_Action action = currentListener.GetActionAtCurrentKeycode();

                LCN_Listener prevListener = currentListener;

                if ( action != null && action.AmActive )
                {
                    print($"found action: '{action.description}'. current listener: '{currentListener.name}'");
                    action.Execute();
                }
                else
                {
                    //print("action not found");
                }

                if( currentListener != prevListener )
                {
                    print($"current listener changed to: '{currentListener}'");
                }
            }
        }

        [ContextMenu("call MakeAllKosher()")]
        public void MakeAllKosher()
        {
            if( LCN_actions_all != null )
            {
                List<LCN_Action> badActions = new List<LCN_Action>();
                foreach ( LCN_Action action in LCN_actions_all )
                {
                    if ( action == null || action.Manager == null )
                    {
                        badActions.Add( action );
                    }
                    else
                    {
                        action.Init();
                    }
                }
                
                foreach (LCN_Action action in badActions )
                {
                    if( Application.isEditor )
                    {
                        Debug.Log($"removing null action...");
                    }
                    LCN_actions_all.Remove(action);
                }
            }

            if( lcnListeners_all != null )
            {
                List<LCN_Listener> badListeners = new List<LCN_Listener>();
                foreach( LCN_Listener listener in lcnListeners_all )
                {
                    if( listener == null )
                    {
                        badListeners.Add( listener );
                    }
                    else
                    {
                        listener.Init();
                        listener.MakeKosher();
                    }
                }

                foreach( LCN_Listener listener in badListeners )
                {
                    if (Application.isEditor)
                    {
                        Debug.Log($"removing null listener...");
                    }
                    lcnListeners_all.Remove(listener);
                }
            }
        }

        public void AddListener( LCN_Listener listener_passed )
        {
            if( lcnListeners_all == null )
            {
                lcnListeners_all = new List<LCN_Listener>();
            }

            if( !lcnListeners_all.Contains( listener_passed ) )
            {
                lcnListeners_all.Add(listener_passed);
                if ( !Application.isPlaying )
                {
                    Debug.Log($"Succesfully logged listener to manager");
                }
            }
            else if ( !Application.isPlaying )
            {
                Debug.Log($"Manager already contained listener.");
            }

            HasKeycodes = true;
        }

        public void ChangeCurrentListener( LCN_Listener listener_passed )
        {
            currentListener = listener_passed;
        }

        public void AddAction( LCN_Action action_passed )
        {
            if (LCN_actions_all == null)
            {
                LCN_actions_all = new List<LCN_Action>();
            }

            if (!LCN_actions_all.Contains(action_passed))
            {
                LCN_actions_all.Add(action_passed);
                if ( !Application.isPlaying )
                {
                    Debug.Log($"Succesfully logged action to manager");
                }
            }
            else if ( !Application.isPlaying )
            {
                Debug.Log($"Manager already contained action.");
            }
        }

        private void OnDrawGizmos()
        {
            if( lcnListeners_all != null && lcnListeners_all.Count > 0 )
            {
                foreach( LCN_Listener listener in lcnListeners_all )
                {
                    if( listener == null )
                    {
                        lcnListeners_all.Remove( listener );
                        break;
                    }

                    if( listener.rt == null )
                    {
                        listener.Init();
                    }

                    Gizmos.DrawIcon( listener.rt.position, "Ear_Listening_withAlpha", false, (listener.ListenImmediately ? Color.green : new Color(0.8f, 0.8f, 0.8f)) );
                }
            }

            if ( LCN_actions_all != null && LCN_actions_all.Count > 0 )
            {
                foreach ( LCN_Action action in LCN_actions_all )
                {
                    if( action == null )
                    {
                         LCN_actions_all.Remove( action );
                        break;
                    }

                    if( action.rt == null )
                    {
                        action.Init();
                    }

                    Gizmos.DrawIcon( action.rt.position, "clapboard", false, new Color(0.8f, 0.8f, 0.8f) );
                }
            }
        }
    }
}