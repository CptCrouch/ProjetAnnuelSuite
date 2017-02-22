using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum DestroyState { Idle, OnDestroy, OnMove, AdjacentCell}

public class DestructionBehavior : MonoBehaviour {
    WorldGenerate worldGenerate;
    GenerateInEditor generateInEditor;
    PunchHexagon punchHexagon;

    [HideInInspector]
    public CancerBehavior cancerBehavior;
     
    public float timeToWaitForDominoEffect;
    public float timeToDestroyCellMin =0.1f;
    public float timeToDestroyCellMax =2f;

    public int nombreChaineDestruction = 6;

    public bool alt1WithRepop;

    [Space(10)]
    [Header("[ FeedbackDissolve ]")]
    
    [SerializeField]
    public float speedFeedbackDissolveAlt1 = 0.5f;
    [SerializeField]
    public float speedFeedbackDissolveAlt2 = 0.5f;
    [SerializeField]
    public float speedFeedbackDissolveAlt3 = 0.5f;

    [SerializeField]
    public GameObject prefabDissolveAlt1;
    [SerializeField]
    public GameObject prefabDissolveAlt2;
    [SerializeField]
    public GameObject prefabDissolveAlt3;
    [SerializeField]
    public Color colorToExplode = Color.red;


    private List<CellTwo> cellToDissolve = new List<CellTwo>();

    [HideInInspector]
    public List<CellTwo> listOfCellOnStart = new List<CellTwo>();
    [HideInInspector]
    public CellTwo cellOnWaitForDestruction;

    [HideInInspector]
    public int currentLvlOnChainSound =0 ;

    
    public bool cancerInTheScene;
    //[HideInInspector]
    //public bool cancerInTheScene;

    private Transform cellParentUsed;




    // Use this for initialization
    void Start() {
        
        worldGenerate = FindObjectOfType<WorldGenerate>();
        punchHexagon = FindObjectOfType<PunchHexagon>();
        cancerBehavior = FindObjectOfType<CancerBehavior>();
        generateInEditor = FindObjectOfType<GenerateInEditor>();

         if(generateInEditor.isPavageScene == false)
        {
            cellParentUsed = worldGenerate.transform;
        }
         else
        {
            cellParentUsed = generateInEditor.pavageParentToCustom;
        }

       
  
        for (int i = 0; i < cellParentUsed.childCount; i++)
        {
            CellTwo cell = cellParentUsed.GetComponent<CellTwo>();
            if (cancerInTheScene == true)
            {
                if (cell.cellType.isCancer == false)
                    listOfCellOnStart.Add(cell);
            }
            else
            {
                listOfCellOnStart.Add(cell);
            }
        }
        
        
    }

    public void DisableAllVirus()
    {
        for (int i = 0; i < cellParentUsed.childCount; i++)
        {
            CellTwo cell = cellParentUsed.GetChild(i).GetComponent<CellTwo>();
            cell.canLaunchVirus = false;
        }
    }
    public void DisableAllCellDestruction()
    {
        for(int i = 0; i < cellParentUsed.childCount; i++)
        {
            CellTwo cell = cellParentUsed.GetChild(i).GetComponent<CellTwo>();
            cell.DisableCell();
        }
    }
   
    public float CalculateSpeedWithNumberOfCellUp()
    {
        float speed;
        int numOfCellUp = 0;
        for (int i = 0; i < cellParentUsed.childCount; i++)
        {
            if(cellParentUsed.GetChild(i).GetComponent<CellTwo>().imAtStartPos == false)
            {
                numOfCellUp++;
            }
        }
        //speed = timeToDestroyCell - (timeToDestroyCell-(timeToDestroyCell/10)*(numOfCellUp / worldGenerate.transform.childCount));
        speed = timeToDestroyCellMax - (timeToDestroyCellMax -timeToDestroyCellMin) * numOfCellUp / worldGenerate.transform.childCount;
        //speed = numOfCellUp / worldGenerate.transform.childCount;
        //Debug.Log(speed);
       

        return speed;
    }

    public bool CheckIfThisWillLaunchChain(CellTwo cell)
    {
        for (int i = 0; i < cell.neighbours.Count; i++)
        {
            if(cell.neighbours[i].cellType.isCancer == false)
            {
                return true;
            }
        }
        return false;

    }



    public void LaunchCellDestruction(CellTwo cell, bool launchByVirus)
    {
        int alt = cell.currentAltitude;
        if (alt == 1)
        {
            StartCoroutine(cell.ReturnToStartPos(speedFeedbackDissolveAlt1, prefabDissolveAlt1, true, currentLvlOnChainSound, launchByVirus,true));
        }
        if (alt == 2)
        {
            if (CheckIfThisWillLaunchChain(cell) == true)
            {
                StartCoroutine(cell.ReturnToStartPos(speedFeedbackDissolveAlt2, prefabDissolveAlt2, false, currentLvlOnChainSound, launchByVirus,false));
            }
            else
            {
                StartCoroutine(cell.ReturnToStartPos(speedFeedbackDissolveAlt2, prefabDissolveAlt2, true, currentLvlOnChainSound, launchByVirus,true));
            }
        }
        if (alt >= 3)
        {
            if (CheckIfThisWillLaunchChain(cell) == true)
            {
                StartCoroutine(cell.ReturnToStartPos(speedFeedbackDissolveAlt3, prefabDissolveAlt3, false, currentLvlOnChainSound, launchByVirus,false));
            }
            else
            {
                StartCoroutine(cell.ReturnToStartPos(speedFeedbackDissolveAlt3, prefabDissolveAlt3, true, currentLvlOnChainSound, launchByVirus,true));
            }
        }


        List<CellTwo> cellListTemp = new List<CellTwo>();
        cellListTemp.Add(cell);
        //GetAreaDisableFeedbacks(1, cellListTemp, false);
        listOfCellOnStart.Add(cell);
        ChooseAndLaunchProperty(alt, cell);
    }
  


    public void ChooseAndLaunchProperty(int altitude, CellTwo target)
    {
        if(altitude ==1)
        {
            if(alt1WithRepop == true)
            UpOneCellRandom(target);
           
        }
        else if(altitude == 2)
        {      
            StartCoroutine(WaitForDominosEffect(target, nombreChaineDestruction));
        }
        else if(altitude >= 3)
        {
            StartCoroutine(WaitForAreaEffect( target, altitude));
        }
    }

    public void UpOneCellRandom(CellTwo cellToAvoid)
    {
        if (listOfCellOnStart.Count > 1)
        {
            int random = Random.Range(0, listOfCellOnStart.Count);
            //Debug.Log(random);
            while (listOfCellOnStart[random] == cellToAvoid)
            {
                random = Random.Range(0, listOfCellOnStart.Count);
            }
            CellTwo cellTwoTemp = listOfCellOnStart[random];

            StartCoroutine(cellTwoTemp.GetPunch(1, punchHexagon.speedScaleCellUp, Vector3.up, false));
            listOfCellOnStart.Remove(cellTwoTemp);
        }
    }

    public void LaunchChainDestruction(CellTwo center, int currentIndex)
    {
        if (currentIndex > 0)
        {
            List<CellTwo> listCloseCell = new List<CellTwo>();
            for (int i = 0; i < center.neighbours.Count; i++)
            {
                if(center.neighbours[i]._imMoving == false && center.neighbours[i].imAtStartPos == false)
                {
                    listCloseCell.Add(center.neighbours[i]);
                }
            }
            /*Vector3 hitVector = new Vector3(center.transform.position.x, 0, center.transform.position.z);
            for (int i = 0; i < worldGenerate.transform.childCount; i++)
            {

                CellTwo cellTwo = worldGenerate.transform.GetChild(i).GetComponent<CellTwo>();
                Vector3 targetVector = new Vector3(cellTwo.transform.position.x, 0, cellTwo.transform.position.z);
                float distanceFromCenterHexagon = Vector3.Distance(hitVector, targetVector);

                if (distanceFromCenterHexagon < 1.6f)
                {

                    if (cellTwo._imMoving == false && cellTwo.imAtStartPos == false )
                    {
                        listCloseCell.Add(cellTwo);
                    }
                }
                //}
            }*/
            if(listCloseCell.Count > 0)
            {
                int random = Random.Range(0, listCloseCell.Count - 1);
                if(listCloseCell[random].currentAltitude ==1 || listCloseCell[random].currentAltitude == 3)
                ChooseAndLaunchProperty(listCloseCell[random].currentAltitude, listCloseCell[random]);
                else
                {
                    currentIndex = nombreChaineDestruction;
                }
                bool cleanCancer = false;

                if (listCloseCell[random].currentAltitude == 1)
                    StartCoroutine(listCloseCell[random].ReturnToStartPos(speedFeedbackDissolveAlt1, prefabDissolveAlt1, false, currentLvlOnChainSound,false,cleanCancer));
                if (listCloseCell[random].currentAltitude == 2)
                    StartCoroutine(listCloseCell[random].ReturnToStartPos(speedFeedbackDissolveAlt2, prefabDissolveAlt2, false, currentLvlOnChainSound,false,cleanCancer));
                if (listCloseCell[random].currentAltitude >= 3)
                    StartCoroutine(listCloseCell[random].ReturnToStartPos(speedFeedbackDissolveAlt3, prefabDissolveAlt3, false, currentLvlOnChainSound,false,cleanCancer));

                listOfCellOnStart.Add(listCloseCell[random]);
                StartCoroutine(WaitForDominosEffect(listCloseCell[random], currentIndex - 1));
            }
            else
            {
                ChooseRandomClosestCell(GetClosestCells(center));
                currentLvlOnChainSound = -1;
                if (cancerInTheScene == true)
                {
                    cancerBehavior.ResetAllCancer();
                }


            }

        }
        else
        {
            ChooseRandomClosestCell(GetClosestCells(center));
            currentLvlOnChainSound = -1;
            if (cancerInTheScene == true)
            {
                cancerBehavior.ResetAllCancer();
            }

        }

    }


    IEnumerator WaitForAreaEffect(CellTwo center,int startAltitude)
    {
        yield return new WaitForSeconds(timeToWaitForDominoEffect);
        GetAreaOfCellAndLaunchReturn(center,startAltitude);
    }
    IEnumerator WaitForDominosEffect( CellTwo center, int currentIndex)
    {
        yield return new WaitForSeconds(timeToWaitForDominoEffect);
        LaunchChainDestruction(center, currentIndex);
        Debug.Log(currentLvlOnChainSound);
        if(currentLvlOnChainSound < 30)
        currentLvlOnChainSound++;
    }


    void GetAreaOfCellAndLaunchReturn( CellTwo center,int startAltitude )
    {
        Vector3 hitVector = new Vector3(center.transform.position.x, 0, center.transform.position.z);
        bool checkIfThisAffect = false;

        for (int i = 0; i < center.neighbours.Count; i++)
        {
            CellTwo cellTwo = center.neighbours[i];
            if (cellTwo._imMoving == false && cellTwo.imAtStartPos == false)
            {
                //cellToDissolve.Add(punchHexagon.worldGenerateObject.transform.GetChild(i).GetComponent<CellTwo>());
                int alt = cellTwo.currentAltitude;

                bool cleanCancer;

                if (CheckIfThisWillLaunchChain(cellTwo) == true)
                {
                    cleanCancer = false;
                    if (alt == 1)
                        StartCoroutine(cellTwo.ReturnToStartPos(speedFeedbackDissolveAlt1, prefabDissolveAlt1, true, currentLvlOnChainSound, false, cleanCancer));
                    if (alt == 2)
                        StartCoroutine(cellTwo.ReturnToStartPos(speedFeedbackDissolveAlt2, prefabDissolveAlt2, true, currentLvlOnChainSound, false, cleanCancer));
                    if (alt >= 3)
                        StartCoroutine(cellTwo.ReturnToStartPos(speedFeedbackDissolveAlt3, prefabDissolveAlt3, true, currentLvlOnChainSound, false, cleanCancer));
                }
                else
                {
                    cleanCancer = true;
                    Debug.Log("NOP");
                    if (alt == 1)
                        StartCoroutine(cellTwo.ReturnToStartPos(speedFeedbackDissolveAlt1, prefabDissolveAlt1, true, currentLvlOnChainSound, false, cleanCancer));
                    if (alt == 2)
                        StartCoroutine(cellTwo.ReturnToStartPos(speedFeedbackDissolveAlt2, prefabDissolveAlt2, true, currentLvlOnChainSound, false, cleanCancer));
                    if (alt >= 3)
                        StartCoroutine(cellTwo.ReturnToStartPos(speedFeedbackDissolveAlt3, prefabDissolveAlt3, true, currentLvlOnChainSound, false, cleanCancer));
                    ChooseRandomClosestCell(GetClosestCells(center));
                }




                listOfCellOnStart.Add(cellTwo);

                ChooseAndLaunchProperty(alt, cellTwo);
                checkIfThisAffect = true;


                //StartCoroutine(WaitForDominoEffect(1, cellTwo.transform,alt));
            }
        }

        
          
        
        if (checkIfThisAffect == false && cancerInTheScene == true)
        {
            cancerBehavior.ResetAllCancer();
        }
    }


    public List<CellTwo> GetClosestCells(CellTwo currentCell)
    {
        float closestDistance = Mathf.Infinity;
        Transform closestPosition = null;
        Vector3 hitVector = new Vector3(currentCell.transform.position.x, 0, currentCell.transform.position.z);
        for (int i = 0; i < cellParentUsed.childCount; i++)
        {
            CellTwo cell = cellParentUsed.GetChild(i).GetComponent<CellTwo>();
           if(cell.imAtStartPos == false && cell != currentCell )
            {
                Vector3 targetVector = new Vector3(cell.transform.position.x, 0, cell.transform.position.z);
                float distanceFromCenterHexagon = Vector3.Distance(hitVector, targetVector);
                if(distanceFromCenterHexagon < closestDistance)
                {
                    closestDistance = distanceFromCenterHexagon;
                    closestPosition = cell.transform;
                }
            }
        }

        List<CellTwo> newList = new List<CellTwo>();
        for (int i = 0; i < cellParentUsed.childCount; i++)
        {
            CellTwo cell = cellParentUsed.GetChild(i).GetComponent<CellTwo>();
            if (cell.imAtStartPos == false)
            {
                if (cell.transform.position == closestPosition.position && cell.state != DestroyState.OnDestroy)
                {
                    newList.Add(cell);
                }
            }
        }

        
        return newList;
    }

    public void ChooseRandomClosestCell(List<CellTwo> list)
    {
        if (list.Count >= 1)
        {
            int random = Random.Range(0, list.Count);

            /*if(cellOnWaitForDestruction.onDestroy == true)
            cellOnWaitForDestruction.DisableCell();*/

            list[random].destroyCoroutine = StartCoroutine(list[random].StartTimerDestruction(CalculateSpeedWithNumberOfCellUp(),true));
            cellOnWaitForDestruction = list[random];
            //GetAreaEnableFeedbacks(1, list[random]);
        }
    }

}
           
