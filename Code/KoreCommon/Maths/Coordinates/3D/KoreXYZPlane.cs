// <fileheader>

using System;

namespace KoreCommon;

public struct KoreXYZPlane
{
    // Three points defining the plane
    public KoreXYZVector PntOrigin { get; }
    public KoreXYZVector VecNormal { get; }
    public KoreXYZVector VecX { get; }
    public KoreXYZVector VecY { get; }

    // ---------------------------------------------------------------------------------------------
    // MARK: Constructors
    // ---------------------------------------------------------------------------------------------

    // Constructor
    public KoreXYZPlane(KoreXYZVector pO, KoreXYZVector vN, KoreXYZVector vX, KoreXYZVector vY)
    {
        PntOrigin = pO;
        VecNormal = vN;
        VecX      = vX;
        VecY      = vY;
    }

    // ---------------------------------------------------------------------------------------------

    // Zero default constructor
    public static KoreXYZPlane Zero => new KoreXYZPlane(KoreXYZVector.Zero, KoreXYZVector.Zero, KoreXYZVector.Zero, KoreXYZVector.Zero);

    // ---------------------------------------------------------------------------------------------

    // Given a normal and a Y (Up) axis, create the X axis and the plane.
    // Also normalises everything
    public static KoreXYZPlane MakePlane(KoreXYZVector pO, KoreXYZVector vN, KoreXYZVector vY)
    {
        // Normalize the normal vector
        vN = vN.Normalize();
        vY = vY.Normalize();

        // Make vY orthogonal to vN (project vY onto the plane perpendicular to vN)
        double dotProduct = KoreXYZVector.DotProduct(vY, vN);
        vY = (vY - vN * dotProduct).Normalize();

        // Create the X axis vector using the correct cross product order
        // vX should be perpendicular to both vY and vN, lying in the plane
        KoreXYZVector vX = KoreXYZVector.CrossProduct(vY, vN).Normalize();

        return new KoreXYZPlane(pO, vN, vX, vY);
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Validation
    // ---------------------------------------------------------------------------------------------

    // Validate the inputs, make sure everything is perpendicular
    public bool IsValid()
    {
        // We can validate the plane, through the following criteria:
        // 1 - The normal vector and the X-axis and Y-axis vectors are all perpendicular to each other, resulting in a dot product of zero.

        double xAxisDotProduct = KoreXYZVector.DotProduct(VecNormal, VecX);
        double yAxisDotProduct = KoreXYZVector.DotProduct(VecNormal, VecY);
        double xyAxisDotProduct = KoreXYZVector.DotProduct(VecX, VecY);

        if (!KoreValueUtils.IsZero(xAxisDotProduct) ||
             !KoreValueUtils.IsZero(yAxisDotProduct) ||
             !KoreValueUtils.IsZero(xyAxisDotProduct))
            return false;

        return true;
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: 2D Projection
    // ---------------------------------------------------------------------------------------------

    // take a 2D point, with reference to the plane, and project it to 3D
    public KoreXYZVector Project2DTo3D(KoreXYVector pnt2D)
    {
        // Convert the 2D point to 3D using the plane's axes and origin
        // X and Y are the local coordinates in the plane
        return PntOrigin
            .Offset(VecX.Scale(pnt2D.X))
            .Offset(VecY.Scale(pnt2D.Y));
    }

    // ---------------------------------------------------------------------------------------------

    // Take a 3D Point and project it to 2D - assuming any deviation from the 3D plane is parallel to the plane normal
    public KoreXYVector Project3DTo2D(KoreXYZVector pnt3D)
    {
        // Get the vector from the origin to the point
        KoreXYZVector vecToPoint = pnt3D.XYZTo(PntOrigin);

        // Project the vector onto the plane's X and Y axes
        double x = KoreXYZVector.DotProduct(vecToPoint, VecX);
        double y = KoreXYZVector.DotProduct(vecToPoint, VecY);

        return new KoreXYVector(x, y);
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: 2D Polar Projection
    // ---------------------------------------------------------------------------------------------

    public KoreXYZVector Project2DPolarTo3D(KoreXYPolarOffset pnt2DPolar)
    {
        // Convert the polar coordinates to Cartesian coordinates
        double x = pnt2DPolar.Radius * Math.Cos(pnt2DPolar.AngleRads);
        double y = pnt2DPolar.Radius * Math.Sin(pnt2DPolar.AngleRads);

        // Now project the 2D Cartesian point to 3D
        return Project2DTo3D(new KoreXYVector(x, y));
    }

    public KoreXYPolarOffset Project3DTo2DPolar(KoreXYZVector pnt3D)
    {
        // First project the 3D point to 2D Cartesian coordinates
        KoreXYVector pnt2D = Project3DTo2D(pnt3D);

        // Now convert the 2D Cartesian coordinates to polar coordinates
        double angleRads = Math.Atan2(pnt2D.Y, pnt2D.X);
        double distance = Math.Sqrt(pnt2D.X * pnt2D.X + pnt2D.Y * pnt2D.Y);

        return new KoreXYPolarOffset(angleRads, distance);
    }

}
