using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleGenerate : MonoBehaviour {

    public float AddX = 0.375f;
    public float AddZ = 0.21f;

    public int lengthSide = 7;
    public GameObject prefab;
    public Transform parent;



	public void GenerateTriangleWorld()
    {
        int newlength;
        if (lengthSide % 2 > 0)
            newlength = (lengthSide - 1) / 2;
        else
            newlength = lengthSide / 2;


        for (int i = -newlength; i <= newlength ; i++)
        {

        }
    }
}
