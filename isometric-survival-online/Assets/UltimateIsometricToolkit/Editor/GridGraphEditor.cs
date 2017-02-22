using Assets.UltimateIsometricToolkit.Scripts.Pathfinding;
using UnityEditor;
using UnityEngine;

namespace Assets.UltimateIsometricToolkit.Editor {
	[CustomEditor(typeof(GridGraph))]
	public class GridGraphEditor : UnityEditor.Editor {
		public override void OnInspectorGUI() {
			DrawDefaultInspector();

			GridGraph myScript = (GridGraph)target;
			if (GUILayout.Button("Update Graph")) {
				myScript.UpdateGraph();
				EditorUtility.SetDirty(myScript);
			}
		}
	}
}
