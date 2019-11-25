using UnityEngine;
using FYFY;

public class LymphocyteSystem : FSystem {
	private Family _lymphocyteGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(Lymphocyte), typeof(Factory))
	);

	// Use this to update member variables when system pause. 
	// Advice: avoid to update your families inside this function.
	protected override void onPause(int currentFrame) {
	}

	// Use this to update member variables when system resume.
	// Advice: avoid to update your families inside this function.
	protected override void onResume(int currentFrame) {
	}

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
		foreach (GameObject go in _lymphocyteGO) {
			Transform tr = go.GetComponent<Transform>();
			Factory factory = go.GetComponent<Factory>();

			factory.reloadProgress += Time.deltaTime;
			if (factory.reloadProgress >= factory.reloadTime) {
				//Instantiate and bind to FYFY a new instance of antibodies drift (factory prefab)
				GameObject myDrift = Object.Instantiate<GameObject>(factory.prefab, tr.position, tr.rotation);
				GameObjectManager.bind(myDrift);

				//Set some parameters of the antibodies drift
				myDrift.GetComponent<Growth>().baseSize = tr.localScale.x * 0.5f;
				float f = myDrift.GetComponent<Growth>().baseSize;
				myDrift.GetComponent<Transform>().localScale = new Vector3(f, f, f);
				myDrift.SetActive(true);

				factory.reloadProgress = 0f;
			}
		}
	}
}
