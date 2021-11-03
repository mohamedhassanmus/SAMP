
public class CurveTestScript : DistributionTestScript {

	public RandomFromDistribution.Direction_e direction = RandomFromDistribution.Direction_e.Right;

	public float skew = 5.0f;
	
	protected override float GetRandomNumber(float min, float max) {
		
		return RandomFromDistribution.RandomRangeSlope(min, max, skew, direction);
	}

}
