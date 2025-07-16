using System;

// Represents a triangle in 2D space defined by three points (A, B, C).
// Provides geometric utilities such as area, centroid, containment, and edge access.

namespace KoreCommon;

public struct KoreXYTriangle
{
    public KoreXYPoint A { get; set; }
    public KoreXYPoint B { get; set; }
    public KoreXYPoint C { get; set; }

    public KoreXYLine LineAB => new KoreXYLine(A, B);
    public KoreXYLine LineBC => new KoreXYLine(B, C);
    public KoreXYLine LineCA => new KoreXYLine(C, A);

    // -------------------------------------------------------------------------------
    // MARK: Angle Properties
    // -------------------------------------------------------------------------------

    // Internal angle at the corner formed by AB -> BC
    public double InternalAngleABRads() => KoreXYPointOps.AngleBetweenRads(A, B, C);

    // Internal angle at the corner formed by BC -> CA
    public double InternalAngleBCRads() => KoreXYPointOps.AngleBetweenRads(B, C, A);

    // Internal angle at the corner formed by CA -> AB
    public double InternalAngleCARads() => KoreXYPointOps.AngleBetweenRads(C, A, B);

    // --------------------------------------------------------------------------------------------
    // MARK: Constructors
    // --------------------------------------------------------------------------------------------

    // Create a triangle from three points.
    public KoreXYTriangle(KoreXYPoint a, KoreXYPoint b, KoreXYPoint c)
    {
        A = a;
        B = b;
        C = c;
    }

    public static KoreXYTriangle Zero { get => new KoreXYTriangle(new KoreXYPoint(0, 0), new KoreXYPoint(0, 0), new KoreXYPoint(0, 0)); }

    // --------------------------------------------------------------------------------------------
    // MARK: Triangle Properties
    // --------------------------------------------------------------------------------------------

    // Returns the centroid (center point) of the triangle.
    public KoreXYPoint CenterPoint() => new KoreXYPoint((A.X + B.X + C.X) / 3.0, (A.Y + B.Y + C.Y) / 3.0);

    // Returns the area of the triangle.
    public double Area() => Math.Abs((A.X * (B.Y - C.Y) + B.X * (C.Y - A.Y) + C.X * (A.Y - B.Y)) / 2.0);

    // Returns the perimeter (sum of edge lengths) of the triangle.
    public double Perimeter() => LineAB.Length + LineBC.Length + LineCA.Length;

    // Returns true if the triangle is degenerate (area is zero or nearly zero).
    public bool IsDegenerate() => Area() < 1e-10;

    // A bounding box rectangle formed from the max and min X and Y coordinates of the triangle's vertices.
    public KoreXYRect AABB()
    {
        KoreXYPoint topLeft = new KoreXYPoint(KoreNumericUtils.Min3(A.X, B.X, C.X), KoreNumericUtils.Min3(A.Y, B.Y, C.Y));
        KoreXYPoint bottomRight = new KoreXYPoint(KoreNumericUtils.Max3(A.X, B.X, C.X), KoreNumericUtils.Max3(A.Y, B.Y, C.Y));
        return new KoreXYRect(topLeft, bottomRight);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Triangle Utilities
    // --------------------------------------------------------------------------------------------

    // Returns true if the given point lies inside the triangle (or on its edge).
    // This is done by comparing the area of the triangle to the sum of the areas of three sub-triangles
    // formed by the test point and each pair of triangle vertices. If the sum of the sub-areas equals
    // the original area (within a small tolerance for floating-point precision), the point is inside or on the triangle.
    public bool Contains(KoreXYPoint point)
    {
        double area = Area();
        double area1 = new KoreXYTriangle(point, B, C).Area();
        double area2 = new KoreXYTriangle(A, point, C).Area();
        double area3 = new KoreXYTriangle(A, B, point).Area();
        return Math.Abs(area - (area1 + area2 + area3)) < 1e-10; // Allow for floating-point precision issues
    }

    public KoreXYTriangle Inset(double inset)
    {
        // Construct lines for each edge
        var ab = new KoreXYLine(A, B);
        var bc = new KoreXYLine(B, C);
        var ca = new KoreXYLine(C, A);

        // Inward-offset each line w.r.t. the opposite point
        var abIn = KoreXYLineOps.OffsetInward(ab, C, inset);
        var bcIn = KoreXYLineOps.OffsetInward(bc, A, inset);
        var caIn = KoreXYLineOps.OffsetInward(ca, B, inset);

        // Intersect adjacent pairs
        if (!KoreXYLineOps.TryIntersect(abIn, bcIn, out var i1)) return this;
        if (!KoreXYLineOps.TryIntersect(bcIn, caIn, out var i2)) return this;
        if (!KoreXYLineOps.TryIntersect(caIn, abIn, out var i3)) return this;

        return new KoreXYTriangle(i1, i2, i3);
    }

    // Returns a new triangle translated by the given offset.
    public KoreXYTriangle Translate(double dx, double dy) => new KoreXYTriangle(A.Offset(dx, dy), B.Offset(dx, dy), C.Offset(dx, dy));

    // --------------------------------------------------------------------------------------------
    // MARK: Triangle Management
    // --------------------------------------------------------------------------------------------

    // Returns the triangle's vertices as an array.
    public KoreXYPoint[] ToArray() => new[] { A, B, C };

    // Returns a string representation of the triangle.
    public override string ToString() => $"Triangle: A={A}, B={B}, C={C}, Area={Area():F2}, Perimeter={Perimeter():F2}, Centroid={CenterPoint()}";
}


