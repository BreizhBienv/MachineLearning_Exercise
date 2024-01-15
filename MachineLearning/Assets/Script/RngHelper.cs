

using System;

public class RngHelper
{
    //Choose a if random value is lower than separation and b if random is superior than separation
    //@param separation must be between 0 and 1;
    public static T Choose<T>(float pSeparation, ref T a, ref T b)
    {
        float rand = UnityEngine.Random.Range(0, 1);
        return pSeparation < rand ? a : b;
    }

    public static float RandomHalfExtent(uint pRange)
    {
        return UnityEngine.Random.Range(-pRange, pRange);
    }

    public static ActivationFunction RandomActivationFunction()
    {
        Array values = Enum.GetValues(typeof(ActivationFunction));
        Random random = new Random();
        return (ActivationFunction)values.GetValue(random.Next(values.Length));
    }
}
