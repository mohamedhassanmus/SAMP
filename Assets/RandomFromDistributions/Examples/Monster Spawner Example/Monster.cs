using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour {

	float lifetime = 0;
	public float SecondsToLive = 3;

	// Update is called once per frame
	void Update () {
		lifetime += Time.deltaTime;
		if (lifetime >= SecondsToLive) {
			FindObjectOfType<MonsterSpawner>().RemoveMonster();
			Destroy(gameObject);
		}
	}
}
