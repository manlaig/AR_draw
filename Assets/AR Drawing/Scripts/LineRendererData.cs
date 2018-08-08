using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Vector3Ser
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }

    public Vector3Ser(float xx, float yy, float zz)
    {
        x = xx;
        y = yy;
        z = zz;
    }

    public static implicit operator Vector3(Vector3Ser rValue)
     {
         return new Vector3(rValue.x, rValue.y, rValue.z);
     }
     
     public static implicit operator Vector3Ser(Vector3 rValue)
     {
         return new Vector3Ser(rValue.x, rValue.y, rValue.z);
     }
}


[System.Serializable]
public class QuaternionSer
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
    public float w { get; set; }

    public QuaternionSer(float xx, float yy, float zz, float ww)
    {
        x = xx;
        y = yy;
        z = zz;
        w = ww;
    }

    public static implicit operator Quaternion(QuaternionSer rValue)
     {
         return new Quaternion(rValue.x, rValue.y, rValue.z, rValue.w);
     }
     
     public static implicit operator QuaternionSer(Quaternion rValue)
     {
         return new QuaternionSer(rValue.x, rValue.y, rValue.z, rValue.w);
     }
}


[System.Serializable]
public class ColorSer
{
    public float r { get; set; }
    public float g { get; set; }
    public float b { get; set; }
    public float a { get; set; }

    public ColorSer(float xx, float yy, float zz, float ww)
    {
        r = xx;
        g = yy;
        b = zz;
        a = ww;
    }

    public static implicit operator Color(ColorSer rValue)
     {
         return new Color(rValue.r, rValue.g, rValue.b, rValue.a);
     }
     
     public static implicit operator ColorSer(Color rValue)
     {
         return new ColorSer(rValue.r, rValue.g, rValue.b, rValue.a);
     }
}


[System.Serializable]
public enum MaterialsEnum
{
    standard = 0, wave = 1, brush = 2, rainbow = 3
}


[System.Serializable]
public class SingleLineRendererData
{
    public Vector3Ser position { get; set; }
    public QuaternionSer rotation { get; set; }
    public Vector3Ser scale { get; set; }
    public List<Vector3Ser> points { get; set; }
    public ColorSer startColor { get; set; }
    public MaterialsEnum material { get; set; }
    public float startWidth { get; set; }
    public float endWidth { get; set; }
    public int cornerVertices { get; set; }
}


[System.Serializable]
public class LineRendererData
{
    public List<SingleLineRendererData> lines { get; set; }
}
