// <fileheader>

using System;

namespace KoreCommon;

public static class KoreXYZLineOps
{
    public static bool IsParallel(this KoreXYZLine l1, KoreXYZLine l2)
    {
        // create the two direction vectors
        KoreXYZVector l1Direction = l1.DirectionUnitVector;
        KoreXYZVector l2Direction = l2.DirectionUnitVector;

        // check if the two direction vectors are parallel.
        // The dot product returns the cosine of the angle between the two vectors, so parallel equals very close to 1.
        double dp = KoreXYZVector.DotProduct(l1Direction, l2Direction);
        return (Math.Abs(dp - 1) < KoreConsts.ArbitrarySmallDouble);
    }
}
