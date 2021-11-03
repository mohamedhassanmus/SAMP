
public class ExponentialTestScript : DistributionTestScript {

	public RandomFromDistribution.Direction_e direction = RandomFromDistribution.Direction_e.Right;

	public float exponent = 2.0f;
	
	protected override float GetRandomNumber(float min, float max) {
		
		return RandomFromDistribution.RandomRangeExponential(min,max, exponent, direction);
	}

}
