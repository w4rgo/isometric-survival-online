using UnityEngine;
using Assets.UltimateIsometricToolkit.Scripts.physics;
using Assets.UltimateIsometricToolkit.Scripts.Pathfinding;
using Assets.UltimateIsometricToolkit.Scripts.Utils;

/// <summary>
/// Converts mouse input to 3d coordinates and invokes A* pathfinding
/// </summary>
public class AstarTestScript : MonoBehaviour {
	public AstarAgent AstarAgent;

	
	
	// Update is called once per frame
	void Update() {
		//raycast when mouse clicked
		if (Input.GetMouseButtonDown(0)) {
			var isoRay = Isometric.MouseToIsoRay();
			IsoRaycastHit isoRaycastHit;
			if (IsoPhysics.Raycast(isoRay, out isoRaycastHit)) {
				Debug.Log("Moving to " + isoRaycastHit.Point);
				AstarAgent.MoveTo(isoRaycastHit.Point);
			}
		}
	}
}
