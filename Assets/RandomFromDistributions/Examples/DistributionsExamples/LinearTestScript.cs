
public class LinearTestScript : DistributionTestScript {
	
	public float slope = 1.0f;
	
	protected override float GetRandomNumber(float min, float max) {
		
		return RandomFromDistribution.RandomRangeLinear(min, max, slope);
	}

}
