// <fileheader>

using System;

// KoreXYZVector: A class to hold an XYZ vector. Units are abstract, for the consumer to decide the context.
// This class is immutable, so all operations return a new object.

// An XYZVector is an abstract, relative difference in 3D position. An XYZPoint is an absolute position.
// - So you can't scale a position, but you can scale a vector. etc.

namespace KoreCommon;

public struct KoreXYZVectorF
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    // --------------------------------------------------------------------------------------------

    public KoreXYZVectorF(double xm, double ym, double zm)
    {
        this.X = (float)xm;
        this.Y = (float)ym;
        this.Z = (float)zm;
    }

    public KoreXYZVectorF(float xm, float ym, float zm)
    {
        this.X = xm;
        this.Y = ym;
        this.Z = zm;
    }

    // --------------------------------------------------------------------------------------------

    // Convenience constructor to create a vector from a point
    public KoreXYZVectorF(KoreXYZVector point)
    {
        this.X = (float)point.X;
        this.Y = (float)point.Y;
        this.Z = (float)point.Z;
    }

    public KoreXYZVectorF(KoreXYZVectorF point)
    {
        this.X = point.X;
        this.Y = point.Y;
        this.Z = point.Z;
    }

    // --------------------------------------------------------------------------------------------

    // Return a zero point as a default value
    // Example: KoreXYZVector newPos = KoreXYZVector.Zero();
    public static KoreXYZVectorF Zero => new KoreXYZVectorF(0, 0, 0);


}
