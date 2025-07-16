using System;
using System.Text.Json.Serialization;

// KoreXYZPoint: A class to hold an XYZ position. Units are abstract, for the consumer to decide the context.
// This class is immutable, so all operations return a new object.

namespace KoreCommon;

public struct KoreXYZPoint
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    [JsonIgnore]
    public double Magnitude
    {
        get
        {
            return Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
        }
        set
        {
            double currMag = Magnitude; // store, avoid caclulating twice
            if (currMag < KoreConsts.ArbitraryMinDouble) // if too close to a div0
            {
                X = value; Y = 0; Z = 0;
            }
            else
            {
                double scaleFactor = value / currMag;
                X *= scaleFactor;
                Y *= scaleFactor;
                Z *= scaleFactor;
            }
        }
    }

    [JsonIgnore]
    public double Length => Magnitude;

    // --------------------------------------------------------------------------------------------

    public KoreXYZPoint(double xm, double ym, double zm)
    {
        this.X = xm;
        this.Y = ym;
        this.Z = zm;
    }

    // Return a zero point as a default value
    // Example: KoreXYZPoint newPos = KoreXYZPoint.Zero();
    public static KoreXYZPoint Zero => new KoreXYZPoint(0, 0, 0);

    // --------------------------------------------------------------------------------------------
    // Basic operations
    // --------------------------------------------------------------------------------------------

    public KoreXYZPoint Offset(double x, double y, double z) => new KoreXYZPoint(X + x, Y + y, Z + z);
    public KoreXYZPoint Offset(KoreXYZPoint shiftXYZ) => new KoreXYZPoint(X + shiftXYZ.X, Y + shiftXYZ.Y, Z + shiftXYZ.Z);
    public KoreXYZPoint Subtract(KoreXYZPoint inputXYZ) => new KoreXYZPoint(X - inputXYZ.X, Y - inputXYZ.Y, Z - inputXYZ.Z);
    public KoreXYZPoint Scale(double scaleFactor) => new KoreXYZPoint(X * scaleFactor, Y * scaleFactor, Z * scaleFactor);

    public KoreXYZPoint Offset(KoreXYZVector shiftXYZ) => new KoreXYZPoint(X + shiftXYZ.X, Y + shiftXYZ.Y, Z + shiftXYZ.Z);

    public KoreXYZPoint Normalize()
    {
        double mag = Magnitude;
        if (mag < KoreConsts.ArbitraryMinDouble)
            return new KoreXYZPoint(1, 0, 0);
        else
            return new KoreXYZPoint(X / mag, Y / mag, Z / mag);
    }

    // --------------------------------------------------------------------------------------------

    public KoreXYZVector XYZTo(KoreXYZPoint remoteXYZ)
    {
        return new KoreXYZVector(remoteXYZ.X - X, remoteXYZ.Y - Y, remoteXYZ.Z - Z);
    }

    public KoreXYZVector VectorTo(KoreXYZPoint remoteXYZ)
    {
        return new KoreXYZVector(remoteXYZ.X - X, remoteXYZ.Y - Y, remoteXYZ.Z - Z);
    }

    public KoreXYZVector UnitVectorTo(KoreXYZPoint remoteXYZ)
    {
        KoreXYZVector diff = XYZTo(remoteXYZ);
        return diff.Normalize();
    }


    public double DistanceTo(KoreXYZPoint inputXYZ)
    {
        double diffX = X - inputXYZ.X;
        double diffY = Y - inputXYZ.Y;
        double diffZ = Z - inputXYZ.Z;

        return Math.Sqrt(diffX * diffX + diffY * diffY + diffZ * diffZ);
    }

    // --------------------------------------------------------------------------------------------
    // Polar Vectors
    // --------------------------------------------------------------------------------------------

    public KoreXYZPolarOffset PolarOffsetTo(KoreXYZPoint p)
    {
        KoreXYZVector diff = XYZTo(p);

        KoreXYZPolarOffset newOffset = KoreXYZPolarOffset.FromXYZ(diff);

        return newOffset;
    }

    public KoreXYZPoint PlusPolarOffset(KoreXYZPolarOffset offset)
    {
        KoreXYZVector diff = offset.ToXYZ();
        return new KoreXYZPoint(X + diff.X, Y + diff.Y, Z + diff.Z);
    }

    // --------------------------------------------------------------------------------------------
    // operator overloads
    // --------------------------------------------------------------------------------------------

    // + operator overload
    public static KoreXYZPoint operator +(KoreXYZPoint a, KoreXYZPoint b) { return new KoreXYZPoint(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }

    // - operator overload for subtracting points
    public static KoreXYZPoint operator -(KoreXYZPoint a, KoreXYZPoint b) { return new KoreXYZPoint(a.X - b.X, a.Y - b.Y, a.Z - b.Z); }

    // * operator overload for scaling a
    public static KoreXYZPoint operator *(KoreXYZPoint a, double scaleFactor) { return new KoreXYZPoint(a.X * scaleFactor, a.Y * scaleFactor, a.Z * scaleFactor); }
    public static KoreXYZPoint operator *(double scaleFactor, KoreXYZPoint a) { return new KoreXYZPoint(a.X * scaleFactor, a.Y * scaleFactor, a.Z * scaleFactor); }

    // / operator overload for scaling a point
    public static KoreXYZPoint operator /(KoreXYZPoint a, double scaleFactor) { return new KoreXYZPoint(a.X / scaleFactor, a.Y / scaleFactor, a.Z / scaleFactor); }
    public static KoreXYZPoint operator /(double scaleFactor, KoreXYZPoint a) { return new KoreXYZPoint(a.X / scaleFactor, a.Y / scaleFactor, a.Z / scaleFactor); }

    // Vector operators
    public static KoreXYZPoint operator -(KoreXYZPoint a, KoreXYZVector b) { return new KoreXYZPoint(a.X - b.X, a.Y - b.Y, a.Z - b.Z); }
    public static KoreXYZPoint operator +(KoreXYZPoint a, KoreXYZVector b) { return new KoreXYZPoint(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }

    // --------------------------------------------------------------------------------------------
    // Conversion
    // --------------------------------------------------------------------------------------------

    public override string ToString()
    {
        return $"({X:F2}, {Y:F2}, {Z:F2})(Mag {Magnitude:F2})";
    }

    // ========================================================================
    // static Ops
    // ========================================================================

    public static KoreXYZPoint Diff(KoreXYZPoint inputXYZ1, KoreXYZPoint inputXYZ2)
    {
        double diffX = inputXYZ2.X - inputXYZ1.X;
        double diffY = inputXYZ2.Y - inputXYZ1.Y;
        double diffZ = inputXYZ2.Z - inputXYZ1.Z;

        return new KoreXYZPoint(diffX, diffY, diffZ);
    }

    public static KoreXYZPoint Sum(KoreXYZPoint inputXYZ1, KoreXYZPoint inputXYZ2)
    {
        double sumX = inputXYZ2.X + inputXYZ1.X;
        double sumY = inputXYZ2.Y + inputXYZ1.Y;
        double sumZ = inputXYZ2.Z + inputXYZ1.Z;

        return new KoreXYZPoint(sumX, sumY, sumZ);
    }

    public static KoreXYZPoint Scale(KoreXYZPoint inputXYZ1, double scaleFactor)
    {
        double scaledX = inputXYZ1.X * scaleFactor;
        double scaledY = inputXYZ1.Y * scaleFactor;
        double scaledZ = inputXYZ1.Z * scaleFactor;

        return new KoreXYZPoint(scaledX, scaledY, scaledZ);
    }

    // Static method to calculate the distance between two points
    public static double DistanceBetween(KoreXYZPoint inputXYZ1, KoreXYZPoint inputXYZ2)
    {
        double diffX = inputXYZ2.X - inputXYZ1.X;
        double diffY = inputXYZ2.Y - inputXYZ1.Y;
        double diffZ = inputXYZ2.Z - inputXYZ1.Z;

        return Math.Sqrt(diffX * diffX + diffY * diffY + diffZ * diffZ);
    }
}
