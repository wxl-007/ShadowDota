using UnityEngine;
using System.Collections;
using AW.War;

public class QuaternionWrap
{
    public float x,y,z,w;
    public QuaternionWrap()
    {

    }

    public static QuaternionWrap QuaternionToWrap(Quaternion qua)
    {
        QuaternionWrap wrap = new QuaternionWrap();
        wrap.x = qua.x;
        wrap.y = qua.y;
        wrap.z = qua.z;
        wrap.w = qua.w;
        return wrap;
    }

    public static Quaternion WrapToQuaternion(QuaternionWrap wrap)
    {
        Quaternion qua = new Quaternion(wrap.x, wrap.y, wrap.z, wrap.w);
        return qua;
    }

    public override string ToString()
    {
        return string.Format("[QuaternionWrap:({0},{1},{2},{3})]", x, y, z, w);
    }

    public static LpcQuaternion ToLpcQuaternion(Quaternion qua)
    {
        LpcQuaternion lpcQua = new LpcQuaternion();
        lpcQua.x = qua.x;
        lpcQua.y = qua.y;
        lpcQua.z = qua.z;
        lpcQua.w = qua.w;
        return lpcQua;
    }

    public static Quaternion ToQuaternion(LpcQuaternion lpcQua)
    {
        return new Quaternion(lpcQua.x, lpcQua.y, lpcQua.z, lpcQua.w);
    }
       
}

public class VectorWrap
{
    public static Vector ToVector(Vector3 vec3)
    {
        Vector vec = new Vector();
        vec.x = vec3.x;
        vec.y = vec3.y;
        vec.z = vec3.z;

        return vec;
    }

    public static Vector3 ToVector3(Vector vec)
    {
        return new Vector3(vec.x, vec.y, vec.z);
    }
}

public class DataStructWrapper{

}
