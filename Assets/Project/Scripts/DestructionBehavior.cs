using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum DestroyState { Idle, OnDestroy, OnMove, AdjacentCell}

public class DestructionBehavior : MonoBehaviour {
    WorldGenerate worldGenerate; 
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

    [HideInInspector]
    public bool cancerInTheScene;



    // Use this for initialization
    void Start() {
        
        worldGenerate = FindObjectOfType<WorldGenerate>();
        punchHexagon = FindObjectOfType<PunchHexagon>();
        cancerBehavior = FindObjectOfType<CancerBehavior>();
        if(cancerBehavior != null)
        {
            cancerInTheScene = true;
        }
  
        for (int i = 0; i < worldGenerate.transform.childCount; i++)
        {
            CellTwo cell = worldGenerate.transform.GetChild(i).GetComponent<CellTwo>();
            if (cell.cellType.isCancer == false)
            listOfCellOnStart.Add(cell);
        }
        
        
    }

    public void DisableAllVirus()
    {
        for (int i = 0; i < worldGenerate.transform.childCount; i++)
        {
            CellTwo cell = worldGenerate.transform.GetChild(i).GetComponent<CellTwo>();
            cell.canLaunchVirus = false;
        }
    }
    public void DisableAllCellDestruction()
    {
        for(int i = 0; i < worldGenerate.transform.childCount; i++)
        {
            CellTwo cell = worldGenerate.transform.GetChild(i).GetComponent<CellTwo>();
            cell.DisableCell();
        }
    }
   
    public float CalculateSpeedWithNumberOfCellUp()
    {
        float speed;
        int numOfCellUp = 0;
        for (int i = 0; i < worldGenerate.transform.childCount; i++)
        {
            if(worldGenerate.transform.GetChild(i).GetComponent<CellTwo>().imAtStartPos == false)
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
        Vector3 hitVector = new Vector3(cell.transform.position.x, 0, cell.transform.position.z);
        for (int i = 0; i < worldGenerate.transform.childCount; i++)
        {
            CellTwo cellTwo = worldGenerate.transform.GetChild(i).GetComponent<CellTwo>();
            Vector3 targetVector = new Vector3(cellTwo.transform.position.x, 0, cellTwo.transform.position.z);
            float distanceFromCenterHexagon = Vector3.Distance(hitVector, targetVector);
            if (distanceFromCenterHexagon < 1.6f )
            {
                return true;
            }
        }
        return false;
    }



    public void LaunchCellDestruction(CellTwo cell, bool launchByVirus)
    {
        int alt = cell.currentAltitude;
        if(alt ==1)
            StartCoroutine(cell.ReturnToStartPos(speedFeedbackDissolveAlt1, prefabDissolveAlt1,true,currentLvlOnChainSound,launchByVirus));
        if (alt == 2)
        {
            if(CheckIfThisWillLaunchChain(cell) == true)
                StartCoroutine(cell.ReturnToStartPos(speedFeedbackDissolveAlt2, prefabDissolveAlt2, false, currentLvlOnChainSound,launchByVirus));
            else
                StartCoroutine(cell.ReturnToStartPos(speedFeedbackDissolveAlt2, prefabDissolveAlt2, true, currentLvlOnChainSound,launchByVirus));
        }
        if (alt >= 3)
        {
            if (CheckIfThisWillLaunchChain(cell) == true)
                StartCoroutine(cell.ReturnToStartPos(speedFeedbackDissolveAlt3, prefabDissolveAlt3, false, currentLvlOnChainSound,launchByVirus));
            else
                StartCoroutine(cell.ReturnToStartPos(speedFeedbackDissolveAlt3, prefabDissolveAlt3, true, currentLvlOnChainSound,launchByVirus));
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
            StartCoroutine(WaitForAreaEffect(1, target, altitude));
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
            Vector3 hitVector = new Vector3(center.transform.position.x, 0, center.transform.position.z);
            List<CellTwo> listCloseCell = new List<CellTwo>();
            for (int i = 0; i < worldGenerate.transform.childCount; i++)
            {

                CellTwo cellTwo = worldGenerate.transform.GetChild(i).GetComponent<CellTwo>();
                Vector3 targetVector = new Vector3(cellTwo.transform.position.x, 0, cellTwo.transform.position.z);
                float distanceFromCenterHexagon = Vector3.Distance(hitVector, targetVector);

                if (distanceFromCenterHexagon < 1.6f)
                {

                    if (cellTwo._imMoving == false && cellTwo.imAtStartPos == false /*&& cellTwo.currentAltitude < startAltitude*/)
                    {
                        listCloseCell.Add(cellTwo);
                    }
                }
                //}
            }
            if(listCloseCell.Count > 0)
            {
                int random = Random.Range(0, listCloseCell.Count - 1);
                if(listCloseCell[random].currentAltitude ==1 || listCloseCell[random].currentAltitude == 3)
                ChooseAndLaunchProperty(listCloseCell[random].currentAltitude, listCloseCell[random]);
                else
                {
                    currentIndex = nombreChaineDestruction;
                }

                if (listCloseCell[random].currentAltitude == 1)
                    StartCoroutine(listCloseCell[random].ReturnToStartPos(speedFeedbackDissolveAlt1, prefabDissolveAlt1, false, currentLvlOnChainSound,false));
                if (listCloseCell[random].currentAltitude == 2)
                    StartCoroutine(listCloseCell[random].ReturnToStartPos(speedFeedbackDissolveAlt2, prefabDissolveAlt2, false, currentLvlOnChainSound,false));
                if (listCloseCell[random].currentAltitude >= 3)
                    StartCoroutine(listCloseCell[random].ReturnToStartPos(speedFeedbackDissolveAlt3, prefabDissolveAlt3, false, currentLvlOnChainSound,false));

                listOfCellOnStart.Add(listCloseCell[random]);
                StartCoroutine(WaitForDominosEffect(listCloseCell[random], currentIndex - 1));
            }
            else
            {
                ChooseRandomClosestCell(GetClosestCells(center));
                currentLvlOnChainSound = -1;
                Debug.Log("Pouet");
            }

        }
        else
        {
            ChooseRandomClosestCell(GetClosestCells(center));
            currentLvlOnChainSound = -1;
            Debug.Log("Prout");
        }

    }


    IEnumerator WaitForAreaEffect(int area, CellTwo center,int startAltitude)
    {
        yield return new WaitForSeconds(timeToWaitForDominoEffect);
        GetAreaOfCellAndLaunchReturn(area,center,startAltitude);
    }
    IEnumerator WaitForDominosEffect( CellTwo center, int currentIndex)
    {
        yield return new WaitForSeconds(timeToWaitForDominoEffect);
        LaunchChainDestruction(center, currentIndex);
        Debug.Log(currentLvlOnChainSound);
        if(currentLvlOnChainSound < 30)
        currentLvlOnChainSound++;
    }


    void GetAreaOfCellAndLaunchReturn(int areaForce, CellTwo center,int startAltitude )
    {
        Vector3 hitVector = new Vector3(center.transform.position.x, 0, center.transform.position.z);
        for (int h = 1; h <= areaForce; h++)
        {

            for (int i = 0; i < worldGenerate.transform.childCount; i++)
            {
                //if (CheckCellOnList(i) == true)
                //{
                CellTwo cellTwo = worldGenerate.transform.GetChild(i).GetComponent<CellTwo>();
                Vector3 targetVector = new Vector3(cellTwo.transform.position.x, 0, cellTwo.transform.position.z);
                float distanceFromCenterHexagon = Vector3.Distance(hitVector, targetVector);
                if (distanceFromCenterHexagon < 1.6f * h)
                {
                    
                    if (cellTwo._imMoving == false && cellTwo.imAtStartPos == false /*&& cellTwo.currentAltitude < startAltitude*/)
                    {
                        //cellToDissolve.Add(punchHexagon.worldGenerateObject.transform.GetChild(i).GetComponent<CellTwo>());
                        int alt = cellTwo.currentAltitude;

                        if (alt == 1)
                            StartCoroutine(cellTwo.ReturnToStartPos(speedFeedbackDissolveAlt1, prefabDissolveAlt1, true, currentLvlOnChainSound,false));
                        if (alt == 2)
                            StartCoroutine(cellTwo.ReturnToStartPos(speedFeedbackDissolveAlt2, prefabDissolveAlt2, true, currentLvlOnChainSound,false));
                        if (alt >= 3)
                            StartCoroutine(cellTwo.ReturnToStartPos(speedFeedbackDissolveAlt3, prefabDissolveAlt3, true, currentLvlOnChainSound,false));

                        listOfCellOnStart.Add(cellTwo);
                        ChooseAndLaunchProperty(alt, cellTwo);

                        //StartCoroutine(WaitForDominoEffect(1, cellTwo.transform,alt));
                    }
                }
                //}
            }
          
        }
    }

    public void GetAreaEnableFeedbacks(int areaForce, CellTwo cellCenter)
    {
        Vector3 hitVector = new Vector3(cellCenter.transform.position.x, 0, cellCenter.transform.position.z);
        for (int h = 1; h <= areaForce; h++)
        {
           

            for (int i = 0; i < worldGenerate.transform.childCount; i++)
            {
                
                CellTwo cellTwo = worldGenerate.transform.GetChild(i).GetComponent<CellTwo>();
                if (cellTwo != cellCenter)
                {
                    Vector3 targetVector = new Vector3(cellTwo.transform.position.x, 0, cellTwo.transform.position.z);
                    float distanceFromCenterHexagon = Vector3.Distance(hitVector, targetVector);
                    if (distanceFromCenterHexagon < 1.6f * h)
                    {
                        cellTwo.state = DestroyState.AdjacentCell;
                        
                        cellTwo.childTimeFeedback.SetActive(true);
                    }
                }
                
            }
        }
    }
    public List<CellTwo> GetAllCellOnDestroyInArea(int areaForce,CellTwo center)
    {
        List<CellTwo> getList = new List<CellTwo>();
        Vector3 hitVector = new Vector3(center.transform.position.x, 0, center.transform.position.z);
        for (int h = 1; h <= areaForce; h++)
        {
           

            for (int i = 0; i < worldGenerate.transform.childCount; i++)
            {

                CellTwo cellTwo = worldGenerate.transform.GetChild(i).GetComponent<CellTwo>();
                Vector3 targetVector = new Vector3(cellTwo.transform.position.x, 0, cellTwo.transform.position.z);
                float distanceFromCenterHexagon = Vector3.Distance(hitVector, targetVector);
                if (distanceFromCenterHexagon < 1.6f * h)
                {

                    if (cellTwo.state == DestroyState.OnDestroy || cellTwo.state == DestroyState.OnMove)
                    {
                        getList.Add(cellTwo);
                    }

                }

            }
        }
        return getList;
    }

    public void GetAreaDisableFeedbacks(int areaForce, List<CellTwo> listCellTwoToDisable, bool isDisablingByPlayer)
    {
        for (int j = 0; j < listCellTwoToDisable.Count; j++)
        {


            if (isDisablingByPlayer == true)
            {
                listCellTwoToDisable[j].state = DestroyState.AdjacentCell;

                listCellTwoToDisable[j].childTimeFeedback.SetActive(true);

                if (listCellTwoToDisable[j].isGrow == true)
                {
                    //listCellTwoToDisable[j].ChangeScaleTimeFeedback(false);
                    listCellTwoToDisable[j].isGrow = false;
                }
                
                // reset time feedback si besoin
            }

            for (int h = 1; h <= areaForce; h++)
            {
                Vector3 hitVector = new Vector3(listCellTwoToDisable[j].transform.position.x, 0, listCellTwoToDisable[j].transform.position.z);

                for (int i = 0; i < worldGenerate.transform.childCount; i++)
                {

                    CellTwo cellTwo = worldGenerate.transform.GetChild(i).GetComponent<CellTwo>();
                    Vector3 targetVector = new Vector3(cellTwo.transform.position.x, 0, cellTwo.transform.position.z);
                    float distanceFromCenterHexagon = Vector3.Distance(hitVector, targetVector);
                    if (distanceFromCenterHexagon < 1.6f * h)
                    {

                        cellTwo.state = DestroyState.Idle;
                        cellTwo.childTimeFeedback.SetActive(false);

                    }

                }
            }
        }
    }


    bool CheckCellOnList(int index)
    {
        for (int i = 0; i < cellToDissolve.Count; i++)
        {
            if(cellToDissolve[i] == punchHexagon.worldGenerateObject.transform.GetChild(index))
            {
                return false;
            }
        }
        return true;
    }

   


   

    public List<CellTwo> GetClosestCells(CellTwo currentCell)
    {
        float closestDistance = Mathf.Infinity;
        Transform closestPosition = null;
        Vector3 hitVector = new Vector3(currentCell.transform.position.x, 0, currentCell.transform.position.z);
        for (int i = 0; i < worldGenerate.transform.childCount; i++)
        {
            CellTwo cell = worldGenerate.transform.GetChild(i).GetComponent<CellTwo>();
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
        for (int i = 0; i < worldGenerate.transform.childCount; i++)
        {
            CellTwo cell = worldGenerate.transform.GetChild(i).GetComponent<CellTwo>();
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
           
