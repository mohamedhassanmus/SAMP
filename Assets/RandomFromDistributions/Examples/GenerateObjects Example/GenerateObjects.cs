// C# example:
using UnityEditor;
using UnityEngine;


[ExecuteInEditMode]
public class GenerateObjects : MonoBehaviour {

	public GameObject object_to_instantiate;

	
	public float pos_range = 1000f;
	
	public float min_size = 10f;
	public float max_size = 100f;

	public int count = 100;


	public enum SizeRangeType_e { Uniform, LinearRight, LinearLeft, Normal, CurveRight, CurveLeft };
	public SizeRangeType_e size_range_type;

	// Add an option to context menu to run the script!
	[ContextMenu("Generate Objects!")]
	public GameObject Generate () {

		// Create objects grouped by "Generated Objects" GameObject.
		GameObject parent_object = new GameObject("Generated Objects");

		// Create the object under same parent as this script.
		parent_object.transform.parent = gameObject.transform.parent;


		// Create the objects with randomized position and scale.
		for (int i = 0; i <= count; ++i) {
			GameObject instantiated_obj = Instantiate(object_to_instantiate) as GameObject;

			instantiated_obj.transform.position = new Vector3(Random.Range(-pos_range, pos_range),
			                                                  Random.Range(-pos_range, pos_range),
			                                                  Random.Range(-pos_range, pos_range));

			float scale;
			switch (size_range_type) {
			case SizeRangeType_e.Uniform :
				scale = Random.Range(min_size, max_size);
				break;
			case SizeRangeType_e.LinearRight :
				scale = RandomFromDistribution.RandomRangeLinear(min_size, max_size, 1.0f);
				break;
			case SizeRangeType_e.LinearLeft :
				scale = RandomFromDistribution.RandomRangeLinear(min_size, max_size, -1.0f);
				break;
			case SizeRangeType_e.Normal :
				scale = RandomFromDistribution.RandomRangeNormalDistribution(min_size, max_size, RandomFromDistribution.ConfidenceLevel_e._999);
				break;
			case SizeRangeType_e.CurveRight :
				scale = RandomFromDistribution.RandomRangeSlope(min_size, max_size, 10.0f, RandomFromDistribution.Direction_e.Right);
				break;
			case SizeRangeType_e.CurveLeft :
				scale = RandomFromDistribution.RandomRangeSlope(min_size, max_size, 10.0f, RandomFromDistribution.Direction_e.Left);
				break;
			default :
				scale = Random.Range(min_size, max_size);
				break;
			}
			instantiated_obj.transform.localScale = new Vector3(scale, scale, scale);

			instantiated_obj.transform.parent = parent_object.transform;
		}

		return parent_object;
	}

}







