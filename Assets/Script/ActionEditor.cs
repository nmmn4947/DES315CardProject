using UnityEngine;
using UnityEditor;

namespace CardProject
{
    [CustomEditor(typeof(Action))]
    public class ActionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            
            base.OnInspectorGUI();
            EditorGUILayout.LabelField("Action");
        }
    }
}
