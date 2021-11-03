using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;

public static class TrainingUtils
{
    public static void Normalize(ref Tensor X, Tensor Xmean, Tensor Xstd)
    {
        for (int i = 0; i < X.length; i++)
        {
            X[i] = (X[i] - Xmean[i]) / Xstd[i];
        }
    }

    public static void UnNormalize(ref Tensor X, Tensor Xmean, Tensor Xstd)
    {
        for (int i = 0; i < X.length; i++)
        {
            X[i] = X[i] * Xstd[i] + Xmean[i];
        }
    }

    public static float[] ReadBinary(string fn, int size)
    {
        if (File.Exists(fn))
        {
            float[] buffer = new float[size];
            BinaryReader reader = new BinaryReader(File.Open(fn, FileMode.Open));
            for (int i = 0; i < size; i++)
            {
                try
                {
                    buffer[i] = reader.ReadSingle();
                }
                catch
                {
                    Debug.Log("There were errors reading file at path " + fn + ".");
                    reader.Close();
                    return null;
                }
            }
            reader.Close();
            return buffer;
        }
        else
        {
            Debug.Log("File at path " + fn + " does not exist.");
            return null;
        }
    }

    public static float[,] Text2FloatArray(string fn, int colSize)
    {
        string[] lines = File.ReadAllLines(fn);

        float[,] data = new float[lines.Length, colSize];
        for (int i = 0; i < lines.Length; i++)
        {
            string[] currlinedata = lines[i].Split(' ');

            for (int j = 0; j < colSize; j++)
            {
                data[i, j] = float.Parse(currlinedata[j]);
            }
        }
        return data;
    }


}
