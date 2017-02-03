using UnityEngine;
//using UnityEditor;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public struct MaterialFeedBackVariables
{
    public Color startCellColor;

    public Color feedBackCellColor;

    public Color colorWhenGrow;
    

    [Space(10)]
    public Material materialWhenGrow;

    [Space(10)]
    public Material feedbackCellMaterial;
    
    public Material feedbackCellMaterialAlt1;
  
    public Material feedbackCellMaterialAlt2;
   
    public Material feedbackCellMaterialAlt3;

    
}

[ExecuteInEditMode]
public class GenerateInEditor : MonoBehaviour {

    public WorldGenerate worldGenerate;

    [Space(10)]
    [Header("[ Diametre de l'HexagonWorld ]")]

    public int diametreWorldHexagon;

    [Space(10)]
    [Header("[ FeedBacks Material and Colors ]")]
    public MaterialFeedBackVariables matColors = new MaterialFeedBackVariables();

    

    [HideInInspector]
    public List<CellType> cellTypes = new List<CellType>();

    private bool alreadyAWorld = false;

    public void CleanWorld()
    {
        while (worldGenerate.transform.childCount > 0)
        {
            worldGenerate.CleanEditorWorld();
        }
        alreadyAWorld = false;
    }
    public void CreateWorld()
    {
        if (alreadyAWorld == false)
        {
            worldGenerate.GenerateHexagonWorld(diametreWorldHexagon, matColors);
            worldGenerate.GetAllCellNeighbours();
            alreadyAWorld = true;
        }
        else
        {
            Debug.Log("Il y déjà un monde. Utilise le bouton Clean");
        }
    }

   
    
}
