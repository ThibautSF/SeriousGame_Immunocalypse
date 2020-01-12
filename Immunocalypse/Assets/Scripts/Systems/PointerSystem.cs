using UnityEngine;
using FYFY;
using FYFY_plugins.PointerManager;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Based on code by Jeff Zimmer (https://hyunkell.com/blog/rts-style-unit-selection-in-unity-5/)
public class PointerSystem : FSystem {
	private Family _selectorGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(SelectorEntity), typeof(PointerSensitive), typeof(PointerOver))
	);

	private Family _selectableGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(SelectableEntity))
	);

	private Family _selectedGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(SelectableEntity)),
		new AnyOfTags("Selected")
	);

	private Family _unselectedGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(SelectableEntity)),
		new NoneOfTags("Selected")
	);

	public PointerSystem() {
		_selectedGO.addEntryCallback(objectSelection);
		//_selectedGO.addExitCallback(objectUnselection);
		_unselectedGO.addEntryCallback(objectUnselection);
	}

	// Use this to update member variables when system pause. 
	// Advice: avoid to update your families inside this function.
	protected override void onPause(int currentFrame) {
	}

	// Use this to update member variables when system resume.
	// Advice: avoid to update your families inside this function.
	protected override void onResume(int currentFrame){
	}

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
		GameObject go = _selectorGO.First();
		
		if (go != null) {
			SelectorEntity selector = go.GetComponent<SelectorEntity>();

			// If we press the right mouse button, move units
			if (Input.GetMouseButtonDown(1) && !selector.isSelecting) {
				foreach (GameObject selectedObjects in _selectedGO) {
					Move move = selectedObjects.GetComponent<Move>();

					move.targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					move.newTargetPosition = true;
				}
			}

			// If we press the left mouse button, begin selection and remember the location of the mouse
			if (Input.GetMouseButtonDown(0)) {
				selector.isSelecting = true;
				selector.mousePosition1 = Input.mousePosition;

				foreach (GameObject selectableObject in _selectableGO) {
					SelectableEntity selectable = selectableObject.GetComponent<SelectableEntity>();
					selectable.isSelected = false;
					GameObjectManager.setGameObjectTag(selectableObject, "Unselected");
				}
			}
			
			// If we let go of the left mouse button, end selection
			if (Input.GetMouseButtonUp(0)) {
				List<GameObject> selectedObjects = new List<GameObject>();

				foreach (GameObject selectableObject in _selectableGO) {
					if (IsWithinSelectionBounds(selectableObject, selector)) {
						SelectableEntity selectable = selectableObject.GetComponent<SelectableEntity>();
						selectable.isSelected = false;
						GameObjectManager.setGameObjectTag(selectableObject, "Selected");

						selectedObjects.Add(selectableObject);
					}
				}
				
				var sb = new StringBuilder();
				sb.AppendLine(string.Format("Selecting [{0}] Units", selectedObjects.Count));
				foreach (GameObject selectedObject in selectedObjects)
					sb.AppendLine("-> " + selectedObject.name);
				Debug.Log(sb.ToString());
				
				selector.isSelecting = false;
			}
			
			// Highlight all objects within the selection box
			foreach (GameObject selectableObject in _selectableGO) {
				Renderer r = selectableObject.GetComponentInChildren<Renderer>();
				if (IsWithinSelectionBounds(selectableObject, selector)) {
					if (r != null)
						r.material.color = Color.green;
				} else {
					if (r != null)
						r.material.color = Color.white;
				}
			}
		}
	}

	private bool IsWithinSelectionBounds(GameObject go, SelectorEntity selector) {
		if (!selector.isSelecting)
			return false;
		
		Camera camera = Camera.main;
		Bounds viewportBounds = Utils.GetViewportBounds(camera, selector.mousePosition1, Input.mousePosition);

		//For simple selection
		bool objectSelected = false;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit2D[] hit2d = Physics2D.GetRayIntersectionAll(ray, Mathf.Infinity, LayerMask.GetMask("Default"));
		foreach (RaycastHit2D hit in hit2d) {
			if (hit.collider != null && hit.collider.transform == go.transform) {
				// raycast hit this gameobject
				objectSelected = true;
				break;
			}
		}

		return viewportBounds.Contains(camera.WorldToViewportPoint(go.transform.position)) || objectSelected;
	}

	private void objectSelection(GameObject go) {
		//TODO add selection visual
		SelectableEntity selectable = go.GetComponent<SelectableEntity>();
		if (selectable.selectionVisual != null) {
			GameObjectManager.setGameObjectState(selectable.selectionVisual, true);
		}
	}

	//private void objectUnselection(int gameObjectInstanceId) {
	private void objectUnselection(GameObject go) {
		//TODO remove selection visual (if object still exist)
		SelectableEntity selectable = go.GetComponent<SelectableEntity>();
		if (selectable.selectionVisual != null) {
			GameObjectManager.setGameObjectState(selectable.selectionVisual, false);
		}
	}
}
