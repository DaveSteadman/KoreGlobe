// KoreXYZSphereWedge: Class representing a 3D "wedge", based on an inner and outer radius, height, and start and end angles.
// Will have operations around the creation and manipulation of these wedges.

// Design Decisions:
// - Zero angle is "east" (3 o'clock) and angles increase anti-clockwise.
// - Anything beyond the core responsibilities will be in a separate "operations" class.
// - Class will be immutable, as operations will return new instances.

namespace KoreCommon;

public struct KoreXYZSphereWedge
{
    public KoreXYZVector Center { get; set; }
    public double InnerRadius { get; set; }
    public double OuterRadius { get; set; }
    public double StartAzRads { get; set; } // Using start and delta (clockwise) angles to better accomodate wrapping
    public double StartElRads { get; set; }
    public double DeltaAzRads { get; set; }
    public double DeltaElRads { get; set; }

    // --------------------------------------------------------------------------------------------
    // Read only / derived attributes (increasing complexity order)
    // --------------------------------------------------------------------------------------------

    public double StartAzDegs
    {
        get { return KoreValueUtils.RadsToDegs(StartAzRads); }
        set { StartAzRads = KoreValueUtils.DegsToRads(value); }
    }
    public double DeltaAzDegs
    {
        get { return KoreValueUtils.RadsToDegs(DeltaAzRads); }
        set { DeltaAzRads = KoreValueUtils.DegsToRads(value); }
    }
    public double StartElDegs
    {
        get { return KoreValueUtils.RadsToDegs(StartElRads); }
        set { StartElRads = KoreValueUtils.DegsToRads(value); }
    }
    public double DeltaElDegs
    {
        get { return KoreValueUtils.RadsToDegs(DeltaElRads); }
        set { DeltaElRads = KoreValueUtils.DegsToRads(value); }
    }

    public double EndAzRads { get { return StartAzRads + DeltaAzRads; } }
    public double EndAzDegs { get { return KoreValueUtils.RadsToDegs(EndAzRads); } }

    public double EndElRads { get { return StartElRads + DeltaElRads; } }
    public double EndElDegs { get { return KoreValueUtils.RadsToDegs(EndElRads); } }

    public KoreNumericRange<double> AzRangeDegs { get { return new KoreNumericRange<double>(StartAzDegs, EndAzDegs); } }
    public KoreNumericRange<double> ElRangeDegs { get { return new KoreNumericRange<double>(StartElDegs, EndElDegs); } }
    public KoreNumericRange<double> AzRangeRads { get { return new KoreNumericRange<double>(StartAzRads, EndAzRads); } }
    public KoreNumericRange<double> ElRangeRads { get { return new KoreNumericRange<double>(StartElRads, EndElRads); } }
    public KoreNumericRange<double> RadiusRange { get { return new KoreNumericRange<double>(InnerRadius, OuterRadius); } }

    public KoreXYZVector BaseStartInnerPoint { get { return Center.PlusPolarOffset(new(StartAzRads, StartElRads, InnerRadius)); } }
    public KoreXYZVector BaseEndInnerPoint { get { return Center.PlusPolarOffset(new(EndAzRads, StartElRads, InnerRadius)); } }
    public KoreXYZVector BaseStartOuterPoint { get { return Center.PlusPolarOffset(new(StartAzRads, StartElRads, OuterRadius)); } }
    public KoreXYZVector BaseEndOuterPoint { get { return Center.PlusPolarOffset(new(EndAzRads, StartElRads, OuterRadius)); } }

    public KoreXYZVector TopStartInnerPoint { get { return Center.PlusPolarOffset(new(StartAzRads, EndElRads, InnerRadius)); } }
    public KoreXYZVector TopEndInnerPoint { get { return Center.PlusPolarOffset(new(EndAzRads, EndElRads, InnerRadius)); } }
    public KoreXYZVector TopStartOuterPoint { get { return Center.PlusPolarOffset(new(StartAzRads, EndElRads, OuterRadius)); } }
    public KoreXYZVector TopEndOuterPoint { get { return Center.PlusPolarOffset(new(EndAzRads, EndElRads, OuterRadius)); } }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public KoreXYZSphereWedge() : this(KoreXYZVector.Zero, 0, 0, 0, 0, 0, 0) { }

    public KoreXYZSphereWedge(
        KoreXYZVector center, double innerRadius, double outerRadius,
        double startAzRads, double deltaAzRads, double startElRads, double deltaElRads)
    {
        // Swap the inner and outer radii if they are the wrong way around
        if (innerRadius > outerRadius)
            (innerRadius, outerRadius) = (outerRadius, innerRadius);

        Center = center;
        InnerRadius = innerRadius;
        OuterRadius = outerRadius;
        StartAzRads = startAzRads;
        DeltaAzRads = deltaAzRads;
        StartElRads = startElRads;
        DeltaElRads = deltaElRads;
    }

    public KoreXYZSphereWedge(KoreXYZSphereWedge wedge)
    {
        Center = wedge.Center;
        InnerRadius = wedge.InnerRadius;
        OuterRadius = wedge.OuterRadius;
        StartAzRads = wedge.StartAzRads;
        DeltaAzRads = wedge.DeltaAzRads;
        StartElRads = wedge.StartElRads;
        DeltaElRads = wedge.DeltaElRads;
    }

    public static KoreXYZSphereWedge Zero => new KoreXYZSphereWedge(KoreXYZVector.Zero, 0, 0, 0, 0, 0, 0);

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    public KoreXYZSphereWedge Offset(double x, double y, double z)
    {
        return new KoreXYZSphereWedge(Center.Offset(x, y, z), InnerRadius, OuterRadius, StartAzRads, DeltaAzRads, StartElRads, DeltaElRads);
    }

    // // Check if a point is within the wedge by checking distance from the center, angles, and height.
    public bool Contains(KoreXYZPolarOffset offset)
    {
        if (!AzRangeDegs.Contains(offset.AzDegs)) return false;
        if (!ElRangeDegs.Contains(offset.ElDegs)) return false;
        if (!RadiusRange.Contains(offset.Range)) return false;

        return true;
    }

    // --------------------------------------------------------------------------------------------
    // Operator overloads
    // --------------------------------------------------------------------------------------------

    // Implement operator overloads if necessary
}
