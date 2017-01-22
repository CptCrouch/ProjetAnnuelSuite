using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CellType {

    public string name;
    public Color color = new Color(0,0,0,1);
    public Material mat;
    public Material matAlt1;
    public Material matAlt2;
    public Material matAlt3;
    public float speedUp;
    public int diffWithBasePosY;
    public bool imAppliedToCell = false;
    public bool feedBackOnMaterial = false;
    public bool feedBackOnEmission = false;
	
}
