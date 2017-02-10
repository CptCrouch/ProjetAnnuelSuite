using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancerBehavior : MonoBehaviour {
    public GenerateInEditor generateInEditor;
    [HideInInspector]
    public CellType cleanCellType;
    [HideInInspector]
    public CellType cancerCellType;

    [SerializeField]
    public CellType cellTypeOnWait;

    private DestructionBehavior destructionBehav;
    private WorldGenerate worldGenerate;

    public GameObject dissolveCancer;
    public float timeToWaitForRespawn = 0.5f;
    


    // Use this for initialization
    void Start () {
        destructionBehav = FindObjectOfType<DestructionBehavior>();
        worldGenerate = FindObjectOfType<WorldGenerate>();

        // VERSION CRADE A MODIFIER
        for (int i = 0; i < generateInEditor.cellTypes.Count; i++)
        {
            
            if (generateInEditor.cellTypes[i].isCancer == true && generateInEditor.cellTypes[i].onWaitForCancer == false)
            {
                cancerCellType = generateInEditor.cellTypes[i];
                Debug.Log("Cancer Found");
                return;
            }
            if(generateInEditor.cellTypes[i].name == "CouleursAutomne")
            {
                cleanCellType = generateInEditor.cellTypes[i];
                Debug.Log("Clean Found");
            }
        }
        

    }
	
	public IEnumerator DestroyCellWithCancer(CellTwo cell)
    {
        Vector3 firstPos = cell.transform.position;

        if (CheckGroupClean(cell) == true)
        {
            CleanGroup(cell);
        }
        else
        {
            UpdateForcingMaterial(cell, cellTypeOnWait);
        }

        GameObject feedBackDissolve = Instantiate(dissolveCancer, firstPos, Quaternion.identity) as GameObject;
        Renderer mat = feedBackDissolve.GetComponent<Renderer>();
        mat.material.SetFloat("_Didi", 1);
        mat.material.SetVector("_ObjectPosition", new Vector4(transform.position.x, 1, transform.position.z, 1));
        //Debug.Log(mat.sharedMaterial.GetFloat("_Didi"));
        while (mat.material.GetFloat("_Didi") >= 0.5f)
        {
            //Debug.Log(mat.sharedMaterial.GetFloat("_Didi"));
            float time = mat.material.GetFloat("_Didi") - Time.deltaTime * destructionBehav.speedFeedbackDissolveAlt1;
            mat.material.SetFloat("_Didi", time);
            yield return null;
        }

        
       
        feedBackDissolve.GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(1f);
        Destroy(feedBackDissolve);
    }

    public bool CheckGroupClean(CellTwo cell)
    {
        for (int i = 0; i < cell.groupOfCancerImWith.Length; i++)
        {
            CellTwo cellTemp = cell.groupOfCancerImWith[i].GetComponent<CellTwo>();
            if (cellTemp.cellType.onWaitForCancer == false && cellTemp.cellType.isCancer == true && cellTemp != cell)
            {
                return false;
            }
        }
        return true;
    }

    public void CleanGroup(CellTwo cell)
    {
        for (int i = 0; i < cell.groupOfCancerImWith.Length; i++)
        {
            CellTwo cellTemp = cell.groupOfCancerImWith[i].GetComponent<CellTwo>();
            UpdateForcingMaterial(cellTemp, cleanCellType);
        }
    }

    public void DestroyAllCellCancerClose(CellTwo center)
    {
        for (int i = 0; i < center.neighbours.Count; i++)
        {
            if (center.neighbours[i].cellType.isCancer == true && center.neighbours[i].cellType.onWaitForCancer == false)
            {
                StartCoroutine(DestroyCellWithCancer(center.neighbours[i]));
            }
        }
    }
    public void ResetCellCancer(CellTwo cell)
    {
    
        UpdateForcingMaterial(cell,cancerCellType);

    } 
    public void ResetAllCancer()
    {
        for (int i = 0; i < worldGenerate.transform.childCount; i++)
        {
            CellTwo cell = worldGenerate.transform.GetChild(i).GetComponent<CellTwo>();
            if (cell.cellType.onWaitForCancer == true)
            {
                ResetCellCancer(cell);
            }
        }
    }
    
    public void UpdateForcingMaterial (CellTwo cell, CellType cellType)
    {
        cell.cellType = cellType;
        cell.UpdateCellType();
        cell.GetComponent<MeshRenderer>().material = cell.cellType.mat;
        cell.currentMat = cell.cellType.mat;
        cell.startMat = cell.cellType.mat;
    }
}
