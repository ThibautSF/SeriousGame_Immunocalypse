using UnityEngine;
using FYFY;
using FYFY_plugins.PointerManager;
using System.Collections;
using System.Collections.Generic;
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
		_selectedGO.addExitCallback(objectUnselection);
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
						GameObjectManager.setGameObjectTag(selectableObject, "Unselected");

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
			if (selector.isSelecting) {
				foreach (GameObject selectableObject in _selectableGO) {
					if (IsWithinSelectionBounds(selectableObject, selector)) {

					} else {

					}
				}

				/*
				foreach( var selectableObject in FindObjectsOfType<SelectableUnitComponent>() ) {
					if( IsWithinSelectionBounds( selectableObject.gameObject ) ) {
						if( selectableObject.selectionCircle == null ) {
							selectableObject.selectionCircle = Instantiate( selectionCirclePrefab );
							selectableObject.selectionCircle.transform.SetParent( selectableObject.transform, false );
							selectableObject.selectionCircle.transform.eulerAngles = new Vector3( 90, 0, 0 );
						}
					} else {
						if( selectableObject.selectionCircle != null ) {
							Destroy( selectableObject.selectionCircle.gameObject );
							selectableObject.selectionCircle = null;
						}
					}
				}
				*/
			}
		}


	}

	private bool IsWithinSelectionBounds(GameObject go, SelectorEntity selector) {
		if (!selector.isSelecting)
			return false;
		
		Camera camera = Camera.main;
		Bounds viewportBounds = Utils.GetViewportBounds(camera, selector.mousePosition1, Input.mousePosition);

		return viewportBounds.Contains(camera.WorldToViewportPoint(go.transform.position));
	}

	private void objectSelection(GameObject go) {
		//TODO add selection visual
	}

	private void objectUnselection(int gameObjectInstanceId) {
		//TODO remove selection visual (if object still exist)
	}
}
