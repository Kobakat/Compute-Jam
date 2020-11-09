using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// From https://www.ronja-tutorials.com/2020/07/26/compute-shader.html
/// </summary>
 
public class Nebula : MonoBehaviour
{
    public GameObject star;
    public ComputeShader shader;
    public int starCount;
    [Range(-1, 1)]
    public float speed;

    ComputeBuffer starBuffer;
    Transform[] instances;
    Vector3[] output;
    int kernel;
    uint threadGroupSize;
   

    #region Unity Event Functions
    void Start()
    {
        SetKernelBuffer();
        CreateStars();
    }
    void Update()
    {
        FillBuffer();
        SetStarPositions();             
    }

    void OnDestroy()
    {
        starBuffer.Dispose();
    }
    #endregion

    #region Logic
    void FillBuffer()
    {
        shader.SetBuffer(kernel, "data", starBuffer);
        int threadGroups = (int)((starCount + (threadGroupSize - 1)) / threadGroupSize);
        shader.Dispatch(kernel, threadGroups, 1, 1);
        starBuffer.GetData(output);
        shader.SetFloat("Time", Time.time * speed);
    }

    void SetStarPositions()
    {
        for (int i = 0; i < instances.Length; i++)
            instances[i].localPosition = output[i];
    }

    void SetKernelBuffer()
    {
        kernel = shader.FindKernel("CSMain");
        shader.GetKernelThreadGroupSizes(kernel, out threadGroupSize, out _, out _);

        starBuffer = new ComputeBuffer(starCount, Star.GetSize());

        output = new Vector3[starCount];
    }

    void CreateStars()
    {
        instances = new Transform[starCount];
        for (int i = 0; i < starCount; i++)
        {
            instances[i] = Instantiate(star, transform).transform;
        }
    
    }    
        #endregion
}


public struct Star
{
    public Vector3 position;

    public static int GetSize()
    {
        return sizeof(float) * 3;
    }
}
