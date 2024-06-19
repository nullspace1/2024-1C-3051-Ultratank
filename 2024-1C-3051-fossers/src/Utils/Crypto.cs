using System;

namespace WarSteel.Utils;

public static class Crypto
{

    public static double GetRandomNumber(double minimum, double maximum)
    {
        Random random = new();
        return random.NextDouble() * (maximum - minimum) + minimum;
    }

}