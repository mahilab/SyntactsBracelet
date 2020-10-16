using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BraceletUtility
{
    //=========================================================================
    public static float Remap( float num, float low1, float high1, float low2, float high2 )
    {
        return low2 + ( num - low1 ) * ( high2 - low2 ) / ( high1 - low1 );
    }

    //=========================================================================
    public static float RemapClamped( float num, float low1, float high1, float low2, float high2 )
    {
        return Mathf.Clamp( Remap( num, low1, high1, low2, high2 ), Mathf.Min( low2, high2 ), Mathf.Max( low2, high2 ) );
    }

    //=========================================================================
    public static float Remap01(float num, float low, float high) {
        return ( num - low ) / ( high - low );
    }

    //=========================================================================
    public static float Remap01Clamped(float num, float low, float high) {
        return Mathf.Clamp01(( num - low ) / ( high - low ));
    }

    //=========================================================================
    public static float WrapTo360(float degrees)  {
        bool was_neg = degrees < 0;
        degrees = degrees % 360;
        if (was_neg)
            degrees += 360;
        return degrees;
    }
    
    //=========================================================================
    public static float WrapTo180(float degrees) {
        return WrapTo360(degrees + 180) - 180;
    }

    //=========================================================================
    public static void DrawPlane(Vector3 position, Vector3 normal, Color color, float scale = 1.0f) { 
        Vector3 v3;
        
        if (normal.normalized != Vector3.forward)
            v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
        else
            v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude;;
            
        var corner0 = position + v3 * scale;
        var corner2 = position - v3 * scale;
        var q = Quaternion.AngleAxis(90.0f, normal);
        v3 = q * v3;
        var corner1 = position + v3 * scale;
        var corner3 = position - v3 * scale;
        
        Debug.DrawLine(corner0, corner2, color);
        Debug.DrawLine(corner1, corner3, color);
        Debug.DrawLine(corner0, corner1, color);
        Debug.DrawLine(corner1, corner2, color);
        Debug.DrawLine(corner2, corner3, color);
        Debug.DrawLine(corner3, corner0, color);
        Debug.DrawRay(position, normal, color);
    }

    //=========================================================================
    public static Texture2D MakeTexture2D( int width, int height, Color color )
    {
        Color[] pix = new Color[width * height];
        for( int i = 0; i < pix.Length; ++i )
        {
            pix[ i ] = color;
        }
        Texture2D result = new Texture2D( width, height );
        result.SetPixels( pix );
        result.Apply();
        return result;
    }

    //=========================================================================
    public static bool AllSame<T>(T[] values) {
        for (int i = 1; i < values.Length; i++) {
            if(!EqualityComparer<T>.Default.Equals(values[i], values[i-1]))
                return false;
        }
        return true;
    }        
}
