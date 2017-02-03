using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancerBehavior : MonoBehaviour {

    [HideInInspector]
    public CellType cleanCellType;
    [HideInInspector]
    public CellType cancerCellType;

    [SerializeField]
    public CellType cellTypeOnWait;

    private DestructionBehavior destructionBehav;
    private WorldGenerate worldGenerate;

    public GameObject dissolveCancer;


    // Use this for initialization
    void Start () {
        destructionBehav = FindObjectOfType<DestructionBehavior>();
        worldGenerate = FindObjectOfType<WorldGenerate>();
        for (int i = 0; i < worldGenerate.transform.childCount; i++)
        {
            CellTwo cell = worldGenerate.transform.GetChild(i).GetComponent<CellTwo>();
            if (cell.cellType.isCancer == true)
            {
                cancerCellType = cell.cellType;
                break;
            }
        }
        for (int i = 0; i < worldGenerate.transform.childCount; i++)
        {
            CellTwo cell = worldGenerate.transform.GetChild(i).GetComponent<CellTwo>();
            if (cell.cellType.isCancer == false)
            {
                cleanCellType = cell.cellType;
                break;
            }
        }

    }
	
	public IEnumerator DestroyCellWithCancer(CellTwo cell)
    {
        Vector3 firstPos = cell.transform.position;
        cell.cellType = cellTypeOnWait;
        cell.UpdateCellType();
        cell.GetComponent<MeshRenderer>().material = cell.cellType.mat;

        /*if (launchByVirus == false)
        {
            soundManager.EmittDestroySound(chainParameter, currentAltitude);
        }
        else
        {
            soundManager.EmittDestroySound(0, currentAltitude);
        }*/

        /*currentAltitude = 0;
        if (cellType.feedBackOnMaterial == true)
        {
            GetComponent<MeshRenderer>().material = startMat;
            currentMat = startMat;
        }*/

        /*if (cellType.imAppliedToCell == false)
            transform.position = new Vector3(transform.position.x, startPosYbyWorldGenerate, transform.position.z);
        else
            transform.position = new Vector3(transform.position.x, startPosYbyWorldGenerate + cellType.diffWithBasePosY, transform.position.z);*/


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
    
}
