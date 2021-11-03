using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterSpawner : MonoBehaviour {

	public GameObject[] monsters;    // These need to have the same size in order to function correctly.
	public List<float> frequencies;  // Think of them together as being like a Map<GameObect,float> (but maps don't work in editor).

	public int maxNumMonsters;
	private int currNumMonsters = 0;

	// Update is called once per frame
	void Update () {
		if (currNumMonsters < maxNumMonsters) {
			int index = RandomFromDistribution.RandomChoiceFollowingDistribution(frequencies);

			GameObject monster = Instantiate(monsters[index]) as GameObject;
			currNumMonsters++;

			// Randomize position
			float x = RandomFromDistribution.RandomRangeNormalDistribution(-5,5, RandomFromDistribution.ConfidenceLevel_e._90);
			float y = RandomFromDistribution.RandomRangeNormalDistribution(-5,5, RandomFromDistribution.ConfidenceLevel_e._90);

			monster.transform.position = new Vector3(x,y,0);
		}
	}

	public void RemoveMonster() {
		currNumMonsters--;
	}
}
