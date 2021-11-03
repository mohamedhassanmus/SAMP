
public class NormalDistributionTestScript : DistributionTestScript {

	public RandomFromDistribution.ConfidenceLevel_e conf_level = RandomFromDistribution.ConfidenceLevel_e._95;
	

	protected override float GetRandomNumber(float min, float max) {

		return RandomFromDistribution.RandomRangeNormalDistribution(min, max, conf_level);
	}
	
}
