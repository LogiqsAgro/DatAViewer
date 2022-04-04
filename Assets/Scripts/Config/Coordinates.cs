using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;


// Unity uses different axis layout and scale, and datatype.
// | Unity | Dat-A |
// | X     | X     |
// | Y     | Z     |
// | Z     | Y     |
public static class Coordinates
{


    /// <summary> mm to meter conversion </summary>
    public const double scale = 0.001;
    public static UnityEngine.Vector3 ToUnity(double x_length, double y_width, double z_height)
    {
        return new UnityEngine.Vector3(
            (float)(x_length * scale),
            (float)(z_height * scale),
            (float)(y_width * scale)
        );
    }
    public static UnityEngine.Vector3 ToUnity(this Pnt3 v)
    {
        return new UnityEngine.Vector3(
            (float)(v.X * scale),
            (float)(v.Z * scale),
            (float)(v.Y * scale)
        );
    }

    public static UnityEngine.Vector3 ToUnity(this Vec3 v)
    {
        return new UnityEngine.Vector3(
            (float)(v.X * scale),
            (float)(v.Z * scale),
            (float)(v.Y * scale)
        );
    }

    public static Axis GetAxisAlignment(UnityEngine.Vector3 v)
    {
        if (v.x != 0 && v.y == 0 && v.z == 0)
            return Axis.X;

        if (v.y != 0 && v.x == 0 && v.z == 0)
            return Axis.Y;

        if (v.z != 0 && v.x == 0 && v.y == 0)
            return Axis.Z;

        return Axis.None;
    }

    public static float GetComponent(UnityEngine.Vector3 v, Axis axis)
    {
        switch (axis)
        {
            case Axis.X: return v.x;
            case Axis.Y: return v.y;
            case Axis.Z: return v.z;
            default: return 0;
        }
    }

    public static UnityEngine.Vector3 SetComponent(UnityEngine.Vector3 v, Axis axis, float value)
    {
        switch (axis)
        {
            case Axis.X: v.x = value; return v;
            case Axis.Y: v.y = value; return v;
            case Axis.Z: v.z = value; return v;
            default: return v;
        }
    }

    // returns a value b that is on the line b0->b1 as a is on the line a0->a1
    public static float Lerp(float a, float a0, float a1, float b0, float b1)
    {
        var dist_a0_a1 = a1 - a0;
        var dist_a0_a = a - a0;
        var dist_a_a1 = a1 - a;
        return b0 * dist_a_a1 + b1 * dist_a0_a / dist_a0_a1;
    }
}

public enum Axis { None = -1, X = 0, Y = 1, Z = 2 }




public struct Vec3
{
    public double X, Y, Z;

    public Vec3 Clone()
    {
        return new Vec3 { X = X, Y = Y, Z = Z };
    }

    [XmlIgnore]
    public bool IsEmpty
    {
        get { return X == 0 && Y == 0 & Z == 0; }
    }
}


public struct Pnt3
{
    public double X, Y, Z;
    public Pnt3 Clone()
    {
        return new Pnt3 { X = X, Y = Y, Z = Z };
    }

    [XmlIgnore]
    public bool IsEmpty
    {
        get { return X == 0 && Y == 0 & Z == 0; }
    }
}

public struct PhysicalBounds
{
    [XmlAttribute]
    public double X;

    [XmlAttribute]
    public double Y;

    [XmlAttribute]
    public double Z;

    [XmlAttribute]
    public double Length;

    [XmlAttribute]
    public double Width;

    [XmlAttribute]
    public double Height;

    [XmlIgnore]
    public Pnt3 Location
    {
        get => new Pnt3 { X = X, Y = Y, Z = Z };
        set { X = value.X; Y = value.Y; Z = value.Z; }
    }


    [XmlIgnore]
    public Vec3 Size
    {
        get => new Vec3 { X = Length, Y = Width, Z = Height };
        set { Length = value.X; Width = value.Y; Height = value.Z; }
    }


    [XmlIgnore]
    public bool IsEmpty
    {
        get
        {
            return X == 0 && Y == 0 & Z == 0 &&
                Length == 0 && Width == 0 && Height == 0;
        }
    }
}

