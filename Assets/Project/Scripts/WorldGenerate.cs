using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WorldGenerate : MonoBehaviour {

   
    [Header("[ Create Box World ]")]
    public int length;
    public int width;

    [Space(10)]
    [Header("[ Variable a ne pas toucher ]")]
    public bool isHexagonWorld = false;
    public int height;
    public float tailleYBigHexagon;
    public float increaseZ = 1.3f;
    public float increaseX = 0.75f;


    //public int diametreGrille = 10;


    [Space(10)]
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private GameObject prefabHexagon;
    [SerializeField]
    private GameObject prefabWall;
    [SerializeField]
    private GameObject prefabWallHexagon;
    [SerializeField]
    private GameObject pool;
    public int howManyCellBonusInPool;

    private int poolCount;
    private int rayon;
    //public List<GameObject> cells;

    /*public void GenerateWorld()
    {
       
                for (int j = 0; j < length; j++)
                {
                    for (int k = 0; k < width; k++)
                    {
                        Vector3 pos = new Vector3(-width / 2 + k, - height/2, -length / 2 + j);
                        //GameObject newObject = pool.transform.GetChild(poolCount).gameObject;
                        GameObject newObject = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
                        newObject.transform.position = pos;
                        newObject.transform.localScale = new Vector3(1, height, 1);
                        newObject.transform.SetParent(transform);
                        newObject.SetActive(true);
                        newObject.name = newObject.transform.position.x + " / " + newObject.transform.position.y + " / " + newObject.transform.position.z;
                        newObject.AddComponent<Cell>();
                        newObject.GetComponent<Cell>().startPosY = -height / 2;
                        newObject.GetComponent<Cell>().startScaleY = height;
                        poolCount++;
                        //Debug.Log(poolCount);
                    }
                }

        for (int i = 0; i < howManyCellBonusInPool; i++)
        {
            GameObject newObject = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            newObject.transform.SetParent(pool.transform);
            newObject.SetActive(false);
        }
        for (int i = 0; i < 4; i++)
        {
            
            GameObject newObject = Instantiate(prefabWall, Vector3.zero, Quaternion.identity) as GameObject;
            if (i == 0)
            {
                Vector3 pos = new Vector3(-0.5f,0,length/2+width/2-0.5f);
                newObject.transform.position = pos;
                newObject.transform.localScale = new Vector3(width*3, 1000, width);
            }
            if (i == 1)
            {
                Vector3 pos = new Vector3(length / 2 + width / 2 -0.5f, 0, -0.5f);
                newObject.transform.position = pos;
                newObject.transform.localScale = new Vector3(length, 1000, length*3);

            }
            if (i == 2)
            {
                Vector3 pos = new Vector3(-0.5f, 0, -(length / 2 + width / 2 + 0.5f));
                newObject.transform.position = pos;
                newObject.transform.localScale = new Vector3(width*3, 1000, width);
            }
            if (i == 3)
            {
                Vector3 pos = new Vector3(-(length / 2 + width / 2 + 0.5f), 0, -0.5f);
                newObject.transform.position = pos;
                newObject.transform.localScale = new Vector3(length, 1000, length*3);
            }
        }
            
        
    }*/

    public void GenerateHexagonWorld(int diametre, MaterialFeedBackVariables matFeedBacks)
    {
        rayon = (diametre - 1) / 2;

        for (int i = -rayon; i <= rayon; i++)
        {
            int posX = diametre - Mathf.Abs(i);
            if(posX % 2 == 0)
            {
                for (int j = -posX+1; j <= posX -1; j=j+2)
                {
                    Vector3 pos = new Vector3(j  * increaseX, /*-height*/ (tailleYBigHexagon * 50) / 2, i * increaseZ);

                    
                    GameObject newObject = Instantiate(prefabHexagon, Vector3.zero, Quaternion.identity) as GameObject;
                    newObject.transform.position = pos;
                    //newObject.transform.localScale = new Vector3(newObject.transform.localScale.x, height, newObject.transform.localScale.z);
                    //newObject.GetComponent<MaterialPropertyBlockAdder>().PropertyVectors[0].PropertyValue.y = height * 0.66f;
                    
                    newObject.transform.SetParent(transform);
                    newObject.SetActive(true);
                    newObject.name = "Cell";
                    newObject.AddComponent<CellTwo>();
                    CellTwo cell = newObject.GetComponent<CellTwo>();
                    cell.startPosYbyWorldGenerate = /*-height*/ (tailleYBigHexagon * 50) / 2;
                    //cell.startScaleY = height;

                    newObject.GetComponent<MeshRenderer>().sharedMaterial.color = matFeedBacks.startCellColor;
                    //newObject.GetComponent<MeshRenderer>().material.SetVector("_ObjectPosition", new Vector3(transform.position.x, 1, transform.position.z));


                    cell.variables = matFeedBacks;
                    //cell.childTimeFeedback = cell.transform.GetChild(0).gameObject;
                    //cell.childTimeFeedback.SetActive(false);
                   

                    poolCount++;
                    
                }
            }
                //pair
                else
            {
                //impair
                int lel = (posX - 1) / 2;
                for (int j = -lel; j <= lel; j++)
                {
                    Vector3 pos = new Vector3(j * increaseX *2, /*-height*/ (tailleYBigHexagon * 50) / 2, i * increaseZ);

                    GameObject newObject = Instantiate(prefabHexagon, Vector3.zero, Quaternion.identity) as GameObject;
                    newObject.transform.position = pos;
                    //newObject.transform.localScale = new Vector3(newObject.transform.localScale.x, height, newObject.transform.localScale.z);
                    //newObject.GetComponent<MaterialPropertyBlockAdder>().PropertyVectors[0].PropertyValue.y = height * 0.66f;
                    newObject.transform.SetParent(transform);
                    newObject.SetActive(true);
                    newObject.name = "Cell";
                    newObject.AddComponent<CellTwo>();
                    CellTwo cell = newObject.GetComponent<CellTwo>();
                    cell.startPosYbyWorldGenerate = /*-height*/ (tailleYBigHexagon * 50) / 2;
                    //cell.startScaleY = height;

                    newObject.GetComponent<MeshRenderer>().sharedMaterial.color = matFeedBacks.startCellColor;
                    //newObject.GetComponent<MeshRenderer>().material.SetVector("_ObjectPosition", new Vector3(transform.position.x, 1, transform.position.z));

                    cell.variables = matFeedBacks;
                    //cell.childTimeFeedback = cell.transform.GetChild(0).gameObject;
                    //cell.childTimeFeedback.SetActive(false);

                    poolCount++;
                    
                }
            }
                
        }
    }

    public void CleanEditorWorld()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            DestroyImmediate(gameObject.transform.GetChild(i).gameObject);
            //Debug.Log(i);
        }
    }

    void OnDrawGizmos()
    {
        //Gizmos.DrawCube(Vector3.zero, new Vector3(diametreGrille, diametreGrille, diametreGrille));
    }
    void SpawnWallHexagon(Vector3 pos)
    {
        GameObject newObject = Instantiate(prefabWallHexagon, Vector3.zero, Quaternion.identity) as GameObject;
        newObject.transform.position = pos;
        newObject.transform.localScale = new Vector3(newObject.transform.localScale.x, height+100, newObject.transform.localScale.z);
        newObject.SetActive(true);
    }
    public void GetAllCellNeighbours()
    {
       for (int i = 0; i < transform.childCount; i++)
        {
            CellTwo cellTwoCenter = transform.GetChild(i).GetComponent<CellTwo>();
            Vector3 centerVector = new Vector3(cellTwoCenter.transform.position.x, 0, cellTwoCenter.transform.position.z);
            
            for (int j = 0; j < transform.childCount; j++)
            {
                CellTwo cellTwoAdjacent = transform.GetChild(j).GetComponent<CellTwo>();
                Vector3 targetVector = new Vector3(cellTwoAdjacent.transform.position.x, 0, cellTwoAdjacent.transform.position.z);
                float distanceFromCenterHexagon = Vector3.Distance(centerVector, targetVector);
                
                if (distanceFromCenterHexagon < 1.6f)
                {
                    cellTwoCenter.neighbours.Add(cellTwoAdjacent);
                    
                }
            }
        }
        

    }
	
	
}
