using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using LogansUINavigator;


[CustomEditor(typeof(LCN_Action))]

    public class LCN_Action_inspector : Editor
    {
        [SerializeField] public VisualTreeAsset CustomInspectorPrefab;
        /// <summary>This will be a reference to the main visualization for the custom inspector that gets created representing an entire LCN_Action script.</summary>
        private VisualElement customInspectorInstance;

        LCN_Action _action;
        SerializedObject _action_so;


        public override VisualElement CreateInspectorGUI()
        {
            _action = (LCN_Action)target;
            _action_so = new SerializedObject(_action);

            //Create the main custom inspector element...
            customInspectorInstance = new VisualElement();
            CustomInspectorPrefab.CloneTree(customInspectorInstance);

            VisualElement Foldout_Init_instance = customInspectorInstance.Q<VisualElement>("Foldout_Init");
            Button initButton = customInspectorInstance.Q<Button>("Button_Init");
            initButton.clicked += _action.Init;

            //EditorGUI.BeginChangeCheck(); //I don't think this did anything...

            return customInspectorInstance;
        }

        /*public override void OnInspectorGUI()
        {
            _lcn_so.Update();
            //EditorGUI.BeginChangeCheck();

            //_lcn_so.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                _lcn_so.ApplyModifiedProperties();
                EditorUtility.SetDirty(_lcn); //Trying recordobject now.
                PrefabUtility.RecordPrefabInstancePropertyModifications(_lcn);

                //Undo.RecordObject(target, "Changed LLM editor");

                //Debug.Log("was changed, yo!");
            }

        }*/
    }
