using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CellTwo : MonoBehaviour
{
    public CellType cellType = new CellType();

    public MaterialFeedBackVariables variables = new MaterialFeedBackVariables();

    public DestroyState state = new DestroyState();

    private DestructionBehavior destructionBehavior;
    [HideInInspector]
    public GameObject childTimeFeedback;

    [HideInInspector]
    public bool _imMoving;
    [HideInInspector]
    public bool _imReturningStartPos;
    [HideInInspector]
    public bool imAtStartPos = true;
    [HideInInspector]
    public bool onDestroy;
    [HideInInspector]
    public bool canLaunchVirus;
    [HideInInspector]
    public bool isGrow = false;


    [HideInInspector]
    public float startPosYbyWorldGenerate;

    
    [HideInInspector]
    public float timeToGoBackToStartColor = 2f;
    [HideInInspector]
    public float speedUpByCellType;

    [HideInInspector]
    public Color startColor;
    [HideInInspector]
    public Color startEmissionColor;
    [HideInInspector]
    public Material startMat;
    


    private Color startColorTemp;
    
    private Color startEmissionColoTemp;
    
    private Material startMatTemp;

    private float timer = 0;
    private Color color = Color.black;

    
    [HideInInspector]
    public Coroutine destroyCoroutine;

    
    
    public List<CellTwo> neighbours = new List<CellTwo>();
    public List<CellTwo> cornerNeighbours = new List<CellTwo>();



    public int currentAltitude;

    [HideInInspector]
    public Material currentMat;

    private SoundManager soundManager;

    private FMOD.Studio.EventInstance eventInstance;

    #region CancerVariables

    
    public GameObject[] groupOfCancerImWith;


    #endregion

    void Start()
    {
        destructionBehavior = FindObjectOfType<DestructionBehavior>();
        state = DestroyState.Idle;
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();

        startMat = GetComponent<MeshRenderer>().material;
        startColor = startMat.color;
        startEmissionColor = startMat.GetColor("_EmissionColor");

        currentMat = startMat;

    }


    #region ChangeCellState
    // Sert dans l'editor
    public void UpdateCellType()
    {
        transform.name = cellType.name;
        //mat.set
        //GetComponent<MeshRenderer>().material.color = cellType.color;

        float lastDifference = transform.position.y - startPosYbyWorldGenerate;
        transform.position = new Vector3(transform.position.x, transform.position.y + cellType.diffWithBasePosY-lastDifference, transform.position.z);
        //GetComponent<MeshRenderer>().material = cellType.mat;

    }

    public IEnumerator ChangeColor()
    {

        if (cellType.feedBackOnEmission == false && cellType.feedBackOnMaterial == false)
            startMat.color = variables.feedBackCellColor;
        else if (cellType.feedBackOnMaterial == false)
            startMat.SetColor("_EmissionColor", variables.feedBackCellColor);
        else
        {
            if (currentAltitude == 1)
                GetComponent<MeshRenderer>().material = variables.feedbackCellMaterialAlt1;
            else if (currentAltitude == 2)
                GetComponent<MeshRenderer>().material = variables.feedbackCellMaterialAlt2;
            else if (currentAltitude >= 3)
                GetComponent<MeshRenderer>().material = variables.feedbackCellMaterialAlt3;
            else
                GetComponent<MeshRenderer>().material = variables.feedbackCellMaterial;
        }
        
        yield return new WaitForEndOfFrame();
        if (_imMoving == false)
        {
            
            if (cellType.feedBackOnEmission == false && cellType.feedBackOnMaterial == false)
                startMat.color = startColor;
            else if (cellType.feedBackOnMaterial == false)
                startMat.SetColor("_EmissionColor", startEmissionColor);
            else
                GetComponent<MeshRenderer>().material = currentMat;
           
        }
        
    }

    public void ChangeScaleTimeFeedback(bool isGrow)
    {
        if (isGrow == true)
        {
            childTimeFeedback.transform.localScale = new Vector3(childTimeFeedback.transform.localScale.x * 2, childTimeFeedback.transform.localScale.y * 2, childTimeFeedback.transform.localScale.z * 2);
            childTimeFeedback.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else
        {
            childTimeFeedback.transform.localScale = new Vector3(childTimeFeedback.transform.localScale.x / 2, childTimeFeedback.transform.localScale.y / 2, childTimeFeedback.transform.localScale.z / 2);
            childTimeFeedback.GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }

    public IEnumerator StartTimerDestruction(float timerToDestroyCell, bool launchByVirus)
    {
        state = DestroyState.OnDestroy;
        onDestroy = true;
        canLaunchVirus = true;

        color = Color.black;
        timer = 0;
        currentMat.SetColor("_EmissionColor", color);
        eventInstance = soundManager.StartGlowSound();

        while (timer<=timerToDestroyCell && onDestroy == true)
        {
            color = Color.Lerp(Color.black,destructionBehavior.colorToExplode,timer/timerToDestroyCell);
            currentMat.SetColor("_EmissionColor", color);
            timer += Time.deltaTime;
           
            yield return null;
        }
        if (onDestroy == true)
        {
            
            destructionBehavior.LaunchCellDestruction(this,launchByVirus);
            
 
        }
        currentMat.SetColor("_EmissionColor", Color.black);
        currentMat.SetFloat("_EmissionColor", 0);
        timer = 0;
        onDestroy = false;
        soundManager.StopGlowSound(eventInstance);
        
        yield return null;


    }

    public void DisableCell()
    {
        if (onDestroy == true)
        {
            StopCoroutine(destroyCoroutine);
            soundManager.StopGlowSound(eventInstance);
        }

        state = DestroyState.Idle;

        currentMat.SetColor("_EmissionColor", Color.black);
        currentMat.SetFloat("_EmissionColor", 0);
        timer = 0;
        color = Color.black;
        onDestroy = false;

    }


    public IEnumerator ReturnToStartPos(float speed,GameObject prefabDissolve,bool launchPassive, int chainParameter,bool launchByVirus,bool cleanCancer)
    {
        Vector3 firstPos = transform.position;
        Quaternion firstRot = transform.rotation;
        Vector3 firstScale = transform.localScale;
        Color firstColor = GetComponent<Renderer>().material.color;

        if(destructionBehavior.cancerInTheScene == true)
        {
            destructionBehavior.cancerBehavior.DestroyAllCellCancerClose(this);
        }

        if (launchByVirus == false)
        {
            soundManager.EmittDestroySound(chainParameter, currentAltitude);
        }
        else
        {
            soundManager.EmittDestroySound(0, currentAltitude);
        }
        
        currentAltitude = 0;
        if (cellType.feedBackOnMaterial == true)
        {
            currentMat = startMat;
            GetComponent<MeshRenderer>().material = currentMat;
            
        }
        state = DestroyState.Idle;
        if (cellType.imAppliedToCell == false)
        transform.position = new Vector3(transform.position.x, startPosYbyWorldGenerate, transform.position.z);
        else
        transform.position = new Vector3(transform.position.x, startPosYbyWorldGenerate + cellType.diffWithBasePosY, transform.position.z);

        imAtStartPos = true;
        GameObject feedBackDissolve = CreateFeedBackExplosion(firstPos, firstRot, firstScale, prefabDissolve);


        Renderer mat = feedBackDissolve.GetComponent<Renderer>();
        mat.material.SetFloat("_Didi", 1);
        mat.material.SetVector("_ObjectPosition", new Vector4(transform.position.x, 1, transform.position.z,1));
        mat.material.SetColor("_MainColor", firstColor);
        //Debug.Log(mat.sharedMaterial.GetFloat("_Didi"));
        while (mat.material.GetFloat("_Didi") >=0.5f)
        {
            //Debug.Log(mat.sharedMaterial.GetFloat("_Didi"));
            float time = mat.material.GetFloat("_Didi") - Time.deltaTime * speed;
            mat.material.SetFloat("_Didi",time);
            yield return null;
        }

        if (launchPassive == true && canLaunchVirus == true)
        {
            destructionBehavior.DisableAllVirus();
            destructionBehavior.ChooseRandomClosestCell(destructionBehavior.GetClosestCells(this));
        }
        if (destructionBehavior.cancerInTheScene == true)
        {
            if (cleanCancer == true)
            {
                destructionBehavior.cancerBehavior.ResetAllCancer();
            }
        }

        canLaunchVirus = false;
        feedBackDissolve.GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(1f);
        Destroy(feedBackDissolve);

    }




    public IEnumerator GetPunch(float strength, float speed, Vector3 direction, bool isByPlayer)
    {
        Vector3 firstPos = transform.position;
        DestroyState startState = state;
        imAtStartPos = false;


        if (currentAltitude == 3)
        {
            destructionBehavior.LaunchCellDestruction(this,false);
            destructionBehavior.DisableAllVirus();
            destructionBehavior.DisableAllCellDestruction();
            Debug.Log("pouet");
        }
        else
        {
            soundManager.EmittGrowSound(currentAltitude+1);
            if (isByPlayer == true)
            {
                destructionBehavior.DisableAllVirus();
                /*if (destructionBehavior.cellOnWaitForDestruction != null && destructionBehavior.cellOnWaitForDestruction.onDestroy == true)
                {
                    destructionBehavior.cellOnWaitForDestruction.DisableCell();
                }*/
                destructionBehavior.DisableAllCellDestruction();
                if (onDestroy == false)
                    destroyCoroutine = StartCoroutine(StartTimerDestruction(destructionBehavior.CalculateSpeedWithNumberOfCellUp(),false));
                else
                {
                    //DisableCell();


                    destroyCoroutine = StartCoroutine(StartTimerDestruction(destructionBehavior.CalculateSpeedWithNumberOfCellUp(),false));
                }

                destructionBehavior.cellOnWaitForDestruction = this;


            }
            else
                state = DestroyState.Idle;




            _imMoving = true;

            //if (destructionBehavior.generateInEditor.isPavageScene == false)
            //{
                while (transform.position.y <= firstPos.y + (strength * direction.y))
                {
                    transform.Translate(direction * Time.deltaTime * speed);
                    yield return null;
                }
                transform.position = new Vector3(transform.position.x, firstPos.y + (strength * direction.y), transform.position.z);
            //}
            /*else
            {
                while (transform.position.y <= firstPos.y + (strength ))
                {
                    transform.Translate(direction * Time.deltaTime * speed);
                    yield return null;
                }

                //transform.position = new Vector3(transform.position.x, transform.position.y, firstPos.z + (strength * direction.z));
                //transform.position = new Vector3(transform.position.x, firstPos.y + (strength * direction.y), transform.position.z);
            }*/

            _imMoving = false;


            currentAltitude++;



            if (cellType.feedBackOnEmission == false && cellType.feedBackOnMaterial == false)
                startMat.color = startColor;
            else if (cellType.feedBackOnMaterial == false)
                startMat.SetColor("_EmissionColor", startEmissionColor);
            else
            {
                if (currentAltitude == 1)
                    GetComponent<MeshRenderer>().material = cellType.matAlt1;
                else if (currentAltitude == 2)
                    GetComponent<MeshRenderer>().material = cellType.matAlt2;
                else if (currentAltitude >= 3)
                    GetComponent<MeshRenderer>().material = cellType.matAlt3;

                currentMat = GetComponent<MeshRenderer>().material;

            }
        }
    


    }


    public GameObject CreateFeedBackExplosion(Vector3 position, Quaternion rotation, Vector3 scale, GameObject basePrefab)
    {
        GameObject instantiate = Instantiate(basePrefab, position, rotation) as GameObject;
        instantiate.transform.localScale = scale*destructionBehavior.cellParentUsed.localScale.x;
        instantiate.GetComponent<MeshFilter>().mesh = GetComponent<MeshFilter>().mesh;



        return instantiate;
    }
    #endregion


}
