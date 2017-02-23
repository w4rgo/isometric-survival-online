using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.CustomObjects
{
    [CustomEditor(typeof(MeshCreator))]
    public class MeshCreatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            MeshCreator myScript = (MeshCreator)target;
            if(GUILayout.Button("Copy arrays"))
            {
                Debug.Log(myScript.GetStringArrays());
                TextEditor te = new TextEditor();
                te.text = myScript.GetStringArrays();
                te.SelectAll();
                te.Copy();
            }
        }
    }
}