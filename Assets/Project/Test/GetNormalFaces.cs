using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetNormalFaces : MonoBehaviour {

    Mesh myMesh;

    Vector3[] normals;

    public int howManyFace; 
	// Use this for initialization
	void Start () {
        myMesh = GetComponent<MeshFilter>().mesh;
        normals = myMesh.normals;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public int GetHowManyFacesDoIHave(Vector3[] normals)
    {
        int howMany = 0;
        // direction between two normal points
        Vector3 dir = Vector3.zero;
        for (int i = 0; i < normals.Length; i++)
        {
            if(normals[i].y == transform.position.y)
            {
                 
            }
        }


        return howMany;
    }
}
