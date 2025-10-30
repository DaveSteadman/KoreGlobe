// <fileheader>

using System;

// Represents a triangle in 2D space defined by three points (A, B, C).
// Provides geometric utilities such as area, centroid, containment, and edge access.

namespace KoreCommon;

public struct KoreXYTriangle
{
    public KoreXYVector A { get; set; }
    public KoreXYVector B { get; set; }
    public KoreXYVector C { get; set; }

    public KoreXYLine LineAB => new KoreXYLine(A, B);
    public KoreXYLine LineBC => new KoreXYLine(B, C);
    public KoreXYLine LineCA => new KoreXYLine(C, A);

    // -------------------------------------------------------------------------------
    // MARK: Angle
    // -------------------------------------------------------------------------------

    // Internal angle at the corner formed by AB -> BC
    public double InternalAngleABRads() => KoreXYVectorOps.AngleBetweenRads(A, B, C);

    // Internal angle at the corner formed by BC -> CA
    public double InternalAngleBCRads() => KoreXYVectorOps.AngleBetweenRads(B, C, A);

    // Internal angle at the corner formed by CA -> AB
    public double InternalAngleCARads() => KoreXYVectorOps.AngleBetweenRads(C, A, B);

    // --------------------------------------------------------------------------------------------
    // MARK: Constructors
    // --------------------------------------------------------------------------------------------

    // Create a triangle from three points.
    public KoreXYTriangle(KoreXYVector a, KoreXYVector b, KoreXYVector c)
    {
        A = a;
        B = b;
        C = c;
    }

    public static KoreXYTriangle Zero { get => new KoreXYTriangle(new KoreXYVector(0, 0), new KoreXYVector(0, 0), new KoreXYVector(0, 0)); }

    // --------------------------------------------------------------------------------------------
    // MARK: Properties
    // --------------------------------------------------------------------------------------------

    // Returns the centroid (center point) of the triangle.
    public KoreXYVector CenterPoint() => new KoreXYVector((A.X + B.X + C.X) / 3.0, (A.Y + B.Y + C.Y) / 3.0);

    // Returns the area of the triangle.
    public double Area() => Math.Abs((A.X * (B.Y - C.Y) + B.X * (C.Y - A.Y) + C.X * (A.Y - B.Y)) / 2.0);

    // Returns the perimeter (sum of edge lengths) of the triangle.
    public double Perimeter() => LineAB.Length + LineBC.Length + LineCA.Length;

    // Returns true if the triangle is degenerate (area is zero or nearly zero).
    public bool IsDegenerate() => Area() < 1e-10;

    // A bounding box rectangle formed from the max and min X and Y coordinates of the triangle's vertices.
    public KoreXYRect AABB()
    {
        KoreXYVector topLeft = new KoreXYVector(KoreNumericUtils.Min3(A.X, B.X, C.X), KoreNumericUtils.Min3(A.Y, B.Y, C.Y));
        KoreXYVector bottomRight = new KoreXYVector(KoreNumericUtils.Max3(A.X, B.X, C.X), KoreNumericUtils.Max3(A.Y, B.Y, C.Y));
        return new KoreXYRect(topLeft, bottomRight);
    }

    public KoreXYCircle Circumcircle()
    {
        // The circumcircle is the unique circle that passes through all three vertices of the triangle.
        // Its center (circumcenter) is equidistant from all three vertices.
        // We find it by solving the system where the center point (h,k) satisfies:
        // (A.x - h)² + (A.y - k)² = (B.x - h)² + (B.y - k)² = (C.x - h)² + (C.y - k)² = r²

        // Using the determinant formula for circumcenter calculation:
        // This approach uses the fact that the circumcenter lies at the intersection of
        // the perpendicular bisectors of any two sides of the triangle.

        double ax = A.X, ay = A.Y;
        double bx = B.X, by = B.Y;
        double cx = C.X, cy = C.Y;

        // Calculate the determinant D = 2 * (ax(by - cy) + bx(cy - ay) + cx(ay - by))
        // This is twice the signed area of the triangle
        double d = 2.0 * (ax * (by - cy) + bx * (cy - ay) + cx * (ay - by));

        // Check for degenerate triangle (collinear points)
        if (Math.Abs(d) < 1e-10)
        {
            // For degenerate triangles, return a circle with infinite radius centered at centroid
            var centroid = CenterPoint();
            return new KoreXYCircle(centroid, double.PositiveInfinity);
        }

        // Calculate circumcenter coordinates using determinant formulas:
        // These formulas come from solving the linear system of perpendicular bisector equations

        // ux = (|A|²(by - cy) + |B|²(cy - ay) + |C|²(ay - by)) / D
        double aSq = ax * ax + ay * ay;  // |A|² (magnitude squared)
        double bSq = bx * bx + by * by;  // |B|² (magnitude squared)
        double cSq = cx * cx + cy * cy;  // |C|² (magnitude squared)

        double ux = (aSq * (by - cy) + bSq * (cy - ay) + cSq * (ay - by)) / d;

        // uy = (|A|²(cx - bx) + |B|²(ax - cx) + |C|²(bx - ax)) / D
        double uy = (aSq * (cx - bx) + bSq * (ax - cx) + cSq * (bx - ax)) / d;

        var circumcenter = new KoreXYVector(ux, uy);

        // Calculate the circumradius (distance from circumcenter to any vertex)
        // Since all vertices are equidistant from the circumcenter, we can use any vertex
        double circumradius = circumcenter.DistanceTo(A);

        return new KoreXYCircle(circumcenter, circumradius);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Utilities
    // --------------------------------------------------------------------------------------------

    // Returns true if the given point lies inside the triangle (or on its edge).
    // This is done by comparing the area of the triangle to the sum of the areas of three sub-triangles
    // formed by the test point and each pair of triangle vertices. If the sum of the sub-areas equals
    // the original area (within a small tolerance for floating-point precision), the point is inside or on the triangle.
    public bool Contains(KoreXYVector point)
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
    public KoreXYVector[] ToArray() => new[] { A, B, C };

    // Returns a string representation of the triangle.
    public override string ToString() => $"Triangle: A={A}, B={B}, C={C}, Area={Area():F2}, Perimeter={Perimeter():F2}, Centroid={CenterPoint()}";
}


