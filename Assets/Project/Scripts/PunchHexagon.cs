using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class PunchHexagon : MonoBehaviour {

    [SerializeField]
    private float rangeEarthTool = 10f;
    
    [SerializeField, Range(0, 3)]
    private float distanceElevation = 1f;
    
    [SerializeField]
    public float speedScaleCellUp = 1f;
    
    

    [SerializeField]
    public GameObject worldGenerateObject;
    
    [SerializeField]
    public DestructionBehavior destroyManager;
    [SerializeField]
    public bool isWithPinceau;



    private PauseInGame pauseScript;
    private GenerateInEditor generateInEditor;
    
    private bool holdMouseButtonLeft;
    private bool holdMouseButtonRight;
    private GameObject lastTargetedCell;
    private GameObject lastTargetedFeedbackCell;
    private List<GameObject> lastFeedBackCells = new List<GameObject>();
    private Vector3 choosedTool;
    

    private List<GameObject> cellTargeted = new List<GameObject>();
    private int currentGeneralAltitude;

    private Transform cellParentUsed;

    // Use this for initialization
    void Start()
    {
        pauseScript = FindObjectOfType<PauseInGame>();
        generateInEditor = FindObjectOfType<GenerateInEditor>();

        if (generateInEditor.isPavageScene == false)
        {
            cellParentUsed = worldGenerateObject.transform;
        }
        else
        {
            cellParentUsed = generateInEditor.pavageParentToCustom;
        }
    }


    // Update is called once per frame
    void Update()
    {
        //Debug.Log(FindObjectOfType<GenerateInEditor>().cancerGroups.Count);
        currentGeneralAltitude = GetGeneralAltitude();
       
        

       
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerCell = 8;
        LayerMask layerMaskCell = 1 << layerCell;
        int layerBall = 9;
        LayerMask layerMaskBall = 1 << layerBall;

        if (pauseScript.isActive == false)
        {
            if (isWithPinceau == true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    holdMouseButtonLeft = true;

                }
               
            }
            
                #region FeedBack
                 
            if (Physics.Raycast(ray, out hit, rangeEarthTool, layerMaskCell))
            {
                   
                if(hit.collider.gameObject.GetComponent<CellTwo>()._imMoving == false && hit.collider.gameObject.GetComponent<CellTwo>().cellType.isCancer == false)
                    StartCoroutine(hit.collider.gameObject.GetComponent<CellTwo>().ChangeColor());

                #endregion


                #region Elevation Terrain
                if (isWithPinceau == true && holdMouseButtonLeft == true)
                {
                    if (destroyManager.generateInEditor.isPavageScene == true)
                        choosedTool = Vector3.forward;
                    else
                        choosedTool = Vector3.up;

                    CellTwo cellHit = hit.collider.GetComponent<CellTwo>();


                    if (cellHit._imMoving == false && hit.collider.gameObject != lastTargetedCell && cellHit.cellType.isCancer == false)
                    {
                        float hitPosX = hit.collider.transform.position.x;
                        float hitPosY = hit.collider.transform.position.y;
                        float hitPosZ = hit.collider.transform.position.z;

                        lastTargetedCell = hit.collider.transform.gameObject;

                            
                        destroyManager.listOfCellOnStart.Remove(cellHit);
                            

                        StartCoroutine(cellHit.GetPunch(distanceElevation, speedScaleCellUp, choosedTool,true));

                        cellTargeted.Add(hit.collider.gameObject);
                        //cellHit.EmittGrowSound();

                        
                    }
                        
                }
                    
                else if (Input.GetMouseButtonDown(0))
                {
                    
                    choosedTool = Vector3.up;

                    CellTwo cellHit = hit.collider.GetComponent<CellTwo>();



                    if (cellHit._imMoving == false && hit.collider.gameObject != lastTargetedCell && cellHit.cellType.isCancer == false && cellHit.cellType.isTriggerDestruction == false)
                    {
                        float hitPosX = hit.collider.transform.position.x;
                        float hitPosY = hit.collider.transform.position.y;
                        float hitPosZ = hit.collider.transform.position.z;

                        lastTargetedCell = hit.collider.transform.gameObject;

                            
                        destroyManager.listOfCellOnStart.Remove(cellHit);
                        StartCoroutine(cellHit.GetPunch(distanceElevation, speedScaleCellUp, choosedTool,true));

                        cellTargeted.Add(hit.collider.gameObject);
                        //cellHit.EmittGrowSound();

                        // dans le cas ou l'on a une aire de force supérieur ou égale à 1
                        
                        
                    }
                }
                else if(Input.GetMouseButtonDown(1))
                {
                    CellTwo cellHit = hit.collider.GetComponent<CellTwo>();
                    if(cellHit.imAtStartPos == false && cellHit._imMoving == false && cellHit.cellType.isCancer == false && cellHit.cellType.isTriggerDestruction == false)
                    {
                        
                        cellHit.OnlyDestroyCell();
                        
                    }
                    if(cellHit.cellType.isTriggerDestruction == true)
                        StartCoroutine(cellHit.DestroyTrigger(destroyManager.speedFeedbackDissolveAlt1, destroyManager.prefabDissolve));
                }
                
                #endregion

            }
            if (Input.GetMouseButtonUp(0) )
            {
                holdMouseButtonLeft = false;
                lastTargetedCell = null;
            }
            if(Input.GetMouseButtonUp(1))
            {
                holdMouseButtonRight = false;
            }
            if(Input.GetMouseButton(0) == false)
            {
                holdMouseButtonLeft = false;
                lastTargetedCell = null;
            }

        }
        
    }

    

    
    public int GetGeneralAltitude()
    {
        int altitude =0;
        for (int i = 0; i < cellParentUsed.childCount; i++)
        {
            int altCell = cellParentUsed.GetChild(i).GetComponent<CellTwo>().currentAltitude;
            altitude = altitude + altCell;
        }
        return altitude;
    }

    public bool CheckCellPosValidity(int randomNumber)
    {
        for (int i = 0; i < cellTargeted.Count; i++)
        {
            if (cellParentUsed.GetChild(randomNumber).gameObject == cellTargeted[i])
            {
                return false;
            }
        }
        return true;
    }
    


    /// <summary>
    /// Rounding float value with defined digit precision.
    /// </summary>
    /// <param name ="num">Number to be rounded</param>
    /// <param name ="precision">Number of digit after comma (100 will be 0.00, 1000 will be 0.000 etc..)</param>
    /// <returns> Rounded float value </returns>
    public static float RoundValue(float num, float precision)
    {
        return Mathf.Floor(num * precision + 0.5f) / precision;
    }
}
