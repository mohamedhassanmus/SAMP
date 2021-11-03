The C# script RandomFromDistribution provides a series of static functions for generating a random number from a given distribution.

The simplest such functions are:

  RandomFromDistribution.RandomRangeLinear(min, max, slope)
  RandomFromDistribution.RandomRangeNormalDistribution(min, max, conf_level)
  RandomFromDistribution.RandomRangeSlope(min, max, skew, direction)             // sec^2(x)
  RandomFromDistribution.RandomRangeExponential(min, max, exponent, direction)   // x^exponent

These all behave like Unity's Random.Range(min, max), but they pull from the given distributions instead of from the normal distribution.
All of the functions are inclusive range [min, max].

The third argument to each function describes the shape of the distribution:
  slope - the slope of the line
  conf_level - the percentage of a standard distribution that is cut out and stretched to the [min,max] range.
  skew - this describes the difference between the the lowest and higest point on the curve.
  exponent - the curve will be y = x^exponent.


------------

Finally, there is a function for generating a random choice following a user-defined discrete proability function:

  RandomFromDistribution.RandomChoiceFollowingDistribution(probabilities)

Here, probabilities should be a list of non-negative (>= 0) floats.
This will return an integer index in the range [0, probabilities.Length).
The probabilities do not need to sum to 1.

For example: RandomFromDistribution.RandomChoiceFollowingDistribution([0.5, 1]) will be twice as likely to return 1 as it will be to return 0.


-------------

On top of these functions, there are some additional functions:

  RandomFromDistribution.RandomNormalDistribution(mean, std_dev) 
        -- pull a random number from a normal distribution described by mean and std_dev. 
	   Note that the range is [-inf,inf], but numbers will tend to be around the mean.

  RandomFromDistribution.RandomFromStandardNormalDistribution() 
        -- pull a random number from the standard normal distribution (ie mean = 0, std_dev = 1)

  RandomFromDistribution.RandomFromSlopedDistribution(skew, direction)
        -- like RandomRangeSlope(min, max, skew, direction), but the range is always [0,1] 

  RandomFromDistribution.RandomFromExponentialDistribution(exponent, direction)
        -- like RandomRangeExponential(min, max, exponent, direction), but the range is always [0,1] 

  RandomFromDistribution.RandomLinear(slope)
	-- like RandomRangeLinear(min, max, slope), but the range is always [0,1]

------------

The example scenes contain different scripts that make use of all the above functions.
