using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TCPPacket : MonoBehaviour
{
    public Pose pose;
    public byte[] bytes;
    
    public TCPPacket(Pose pose)
    {
        this.pose = pose;
    }

    public TCPPacket(byte[] bytes)
    {
        this.bytes = bytes;
    }

    public Pose AsPose()
    {
        if (pose != Pose.identity) return pose;
        
        float[] floats = new float[7];
        int floatSize = (sizeof(float));
        for (int i = 0; i < 7; i++)
        {
            floats[i] = (System.BitConverter.ToSingle(bytes, i * floatSize));
        }

        pose = new Pose(
            new Vector3(floats[0], floats[1], floats[2]),
            new Quaternion(floats[3], floats[4], floats[5], floats[6])
        );

        return pose;
    }
    public byte[] AsByte()
    {
        if (bytes != null) return bytes;
        
        float[] array = new[]
        {
            pose.position.x,
            pose.position.y,
            pose.position.z,
            pose.rotation.x,
            pose.rotation.y,
            pose.rotation.z,
            pose.rotation.w
        };
        int floatSize = (sizeof(float));
        int size = (sizeof(float) * 7);
        byte[] result = new byte[size];
        int index = 0;
        for (int i = 0; i < 7; i++)
        {
            Buffer.BlockCopy(array,i,result, index, floatSize);
            index += floatSize;
        }

        bytes = result;
        return result;
    }
}
