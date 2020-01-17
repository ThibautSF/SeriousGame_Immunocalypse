using UnityEngine;
using FYFY;

public class GrowthAndLiveSystem : FSystem {
	private Family _growingGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(Life), typeof(Growth))
	);

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
		foreach (GameObject go in _growingGO) {
			Growth gr = go.GetComponent<Growth>();

			//First an antibodies drift grow (during a fixed time) to his max size
			//After the drift "live" during a fixed time before being destroyed
			switch (gr.hasGrewUp) {
				case true:
					lives(go);
					break;
				
				case false:
					grows(go);
					break;
			}
		}
	}

	private void grows(GameObject go) {
		Transform tr = go.GetComponent<Transform>();
		Growth gr = go.GetComponent<Growth>();

		gr.growthProgress += Time.deltaTime;

		float percent = 0f;
		float size = gr.baseSize;
		if (gr.growthProgress > 0f) {
			percent = gr.growthProgress/gr.growthTime;
			size += (gr.finalSize - gr.baseSize) * percent;
			if (size > gr.finalSize) size = gr.finalSize;
		}

		tr.localScale = new Vector3(size, size, size);

		if (gr.growthProgress >= gr.growthTime) {
			gr.hasGrewUp = true;
		}
	}

	private void lives(GameObject go) {
		Life lf = go.GetComponent<Life>();

		lf.lifeProgress += Time.deltaTime;

		if (lf.lifeProgress >= lf.lifeTime) {
			GameObjectManager.unbind(go);
			Object.Destroy(go);
		}
	}
}
