using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Collections.Generic;

[InitializeOnLoad]
public class DrawGUILayout : Editor {

    // Script permettant d'avoir de la GUI dans la scène

    public static GenerateInEditor generateInEditor;

    public static bool isInCancerMode
    {
        get { return EditorPrefs.GetBool("isInCancerMode"); }
        set { EditorPrefs.SetBool("isInCancerMode", value); }

    }

    public static int SelectedType
    {
        get { return EditorPrefs.GetInt("SelectedType", 0); }
        set { EditorPrefs.SetInt("SelectedType", value); }
    }

    // A chaque fois que la class DrawGUILayout est appelé, effectue cette fonction
    static DrawGUILayout()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        SceneView.onSceneGUIDelegate += OnSceneGUI;
       
    }

    // Forme d'update
    static void OnSceneGUI(SceneView _sceneView)
    {
        if(generateInEditor == null)
        {
            generateInEditor = FindObjectOfType<GenerateInEditor>();
        }
        // Ici on va afficher la GUI uniquement si on a selectionner des cellules uniquement
        if (Selection.gameObjects.Length >0 && CheckObjectSelection(Selection.gameObjects) == true )
        {
            DrawToolsMenu(_sceneView.position);
            DrawButtonActivateCancerMode(_sceneView.position);
            SceneView.RepaintAll();
            DrawCustomBlockButtons(_sceneView);

            if (!isInCancerMode)
                return;
            SceneView.RepaintAll();
            DrawCancerGUIButtons(_sceneView);
            
            /*for (int i = 0; i < generateInEditor.cancerGroups.Count; i++)
            {
                for (int j = 0; j < generateInEditor.cancerGroups[i].Length; j++)
                {
                    DrawFeedbackCancerGroup(generateInEditor.cancerGroups[i][j].transform.position);
                }
                
            }*/


        }
        

        
    }
    // Check si tous les objets selectionné sont bien des cellules 
    static bool CheckObjectSelection(GameObject[] objSelected)
    {
        for (int i = 0; i < objSelected.Length; i++)
        {
            if (objSelected[i].tag != "Cell" && objSelected[i].tag != "WorldGenerate")
            {
                return false;
            }
        }
        return true;
    }
    static void DrawButtonActivateCancerMode(Rect _position)
    {
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(0, _position.height /*-618*/-35 ,_position.width, 30), EditorStyles.whiteLabel);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        
        isInCancerMode = GUILayout.Toggle(
            isInCancerMode,
            "isWithCancer",
            EditorStyles.toolbarButton);
        
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        Handles.EndGUI();
    }


    static void DrawFeedbackCancerGroup(Vector3 pos)
    {
        Vector2 screenPos = HandleUtility.WorldToGUIPoint(pos);
        Handles.SphereCap(0, screenPos, Quaternion.identity, 1);
    }


    static void DrawCancerGUIButtons(SceneView _sceneView)
    {
        Handles.BeginGUI();

        GUI.Box(new Rect(750, 250, 180, _sceneView.position.height -300), GUIContent.none, EditorStyles.textArea);
        GUILayout.BeginArea(new Rect(750, 250, 180, _sceneView.position.height - 300));
        GUILayout.FlexibleSpace();
        
        for (int i = 0; i < generateInEditor.cancerGroups.Count; i++)
        {
            
            GUILayout.Label( "Groupe " + i, GUIStyle.none);
            //GUILayout.FlexibleSpace();
            if(GUILayout.Button("Remove"))
            {
                EditorApplication.Beep();
                if (EditorUtility.DisplayDialog("Really?", "Do you really want to remove the Group " + i + " ?", "Yes", "No") == true)
                {

                    Undo.RecordObject(generateInEditor, "");

                    for (int j = 0; j < generateInEditor.cancerGroups[i].Length; j++)
                    {
                        CellTwo cell = generateInEditor.cancerGroups[i][j].GetComponent<CellTwo>();
                        Undo.RecordObject(cell, "Apply On Cell");
                        cell.groupOfCancerImWith = null;
                        EditorUtility.SetDirty(cell);
                    }

                    generateInEditor.cancerGroups.RemoveAt(i);
                    EditorUtility.SetDirty(generateInEditor);
                }
            }
        }
        
        if (GUILayout.Button("Create Cancer Group Selection", EditorStyles.toolbarButton))
        {
            Undo.RecordObject(generateInEditor, "New Cancer Group Created");
            generateInEditor.cancerGroups.Add(CreateNewCancerGroup(Selection.gameObjects));
            EditorUtility.SetDirty(generateInEditor);

        }
        if (GUILayout.Button("ClearAll", EditorStyles.toolbarButton))
        {

            for (int i = 0; i < generateInEditor.worldGenerate.transform.childCount; i++)
            {
                CellTwo cell = generateInEditor.worldGenerate.transform.GetChild(i).GetComponent<CellTwo>();
                Undo.RecordObject(cell, "Clear Cancer Array");
                cell.groupOfCancerImWith = null;
                EditorUtility.SetDirty(cell);
            }
            Undo.RecordObject(generateInEditor, "Clear Editor");
            generateInEditor.cancerGroups.Clear();
            EditorUtility.SetDirty(generateInEditor);
        }

        GUILayout.EndArea();
        Handles.EndGUI();
    }
    static GameObject[] CreateNewCancerGroup(GameObject[] selection)
    {
        List<GameObject> listTemp = new List<GameObject>();
        // On tri parmi les cellules pour retirer celle qui ne sont pas cancereuse
        for (int i = 0; i < selection.Length; i++)
        {
            CellTwo cell = selection[i].GetComponent<CellTwo>();
            if(cell.cellType.isCancer == true)
            {
                listTemp.Add(cell.gameObject);
            }
        }
        //on crée le tableau temporaire 
        GameObject[] temp = new GameObject[listTemp.Count];
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = listTemp[i];
        }
        // on attribue le tableau temporaire a chaque cellule 
        for (int i = 0; i < temp.Length; i++)
        {
            CellTwo cell = temp[i].GetComponent<CellTwo>();
            Undo.RecordObject(cell, "Changed Group Cancer");
            cell.groupOfCancerImWith = temp;
            EditorUtility.SetDirty(cell);
        }


        return temp;
    }






    #region CellType

    static void DrawToolsMenu(Rect _position)
    {
        //On commence à dessiner dans la scène
        Handles.BeginGUI();

        //Here we draw a toolbar at the bottom edge of the SceneView
        GUILayout.BeginArea(new Rect(0, _position.height - 35, _position.width, 30 ), EditorStyles.whiteLabel);
        GUILayout.BeginHorizontal();
        
        if(GUILayout.Button("APPLY To Selection",EditorStyles.toolbarButton))
        {
            ApplyNewCellType(SelectedType, Selection.gameObjects);
        }
        GUILayout.Space(5);
        if (GUILayout.Button("APPLY to All", EditorStyles.toolbarButton))
        {
            ApplyNewCellTypeToAll(SelectedType);
        }
        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
       
        Handles.EndGUI();
    }

    void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }
    
    static void DrawCustomBlockButtons(SceneView _sceneView)
    {
        Handles.BeginGUI();

        GUI.Box(new Rect(0, 0, 90, _sceneView.position.height - 35), GUIContent.none, EditorStyles.textArea);

        for (int i = 0; i < generateInEditor.cellTypes.Count; ++i)
        {
            DrawCustomBlockButton(i);
        }

        Handles.EndGUI();
    }

    static void DrawCustomBlockButton(int index)
    {
        bool _isActive = index == SelectedType;
 
        GUIStyle toggleStyle = GUI.skin.button;

        GUIStyle _text = EditorStyles.boldLabel;
        if(generateInEditor.cellTypes[index].mat != null)
        _text.normal.textColor = generateInEditor.cellTypes[index].mat.color;
        GUI.Label(new Rect(12, index * 98 + 8, 100, 20), generateInEditor.cellTypes[index].name, _text);

        bool _isToggleDown = GUI.Toggle(new Rect(12, index * 98 + 25, 60, 60), _isActive, "",toggleStyle);
        

        //check si l'utilisateur a appuyer sur un nouveau CellType
        if (_isToggleDown && !_isActive)
            SelectedType = index;
    }

    // correspond à la fonction appelée lorsque l'on appuie sur le bouton Apply
    // On applique le current type selected à toute les cellules sélectionnée
    public static void ApplyNewCellType(int indexCellTypes, GameObject[] selection)
    {
        for (int i = 0; i < selection.Length; i++)
        {
            if (selection[i].tag == "Cell")
            {
                CellTwo cellTwo = selection[i].GetComponent<CellTwo>();
                Undo.RecordObject(cellTwo, "Update Cell");

                cellTwo.cellType.name = generateInEditor.cellTypes[indexCellTypes].name;
                cellTwo.cellType.isCancer = generateInEditor.cellTypes[indexCellTypes].isCancer;
                //cellTwo.cellType.color = generateInEditor.cellTypes[indexCellTypes].color;
                //cellTwo.cellType.speedUp = generateInEditor.cellTypes[indexCellTypes].speedUp;
                cellTwo.cellType.diffWithBasePosY = generateInEditor.cellTypes[indexCellTypes].diffWithBasePosY;
                cellTwo.GetComponent<MeshRenderer>().material = generateInEditor.cellTypes[indexCellTypes].mat;
                cellTwo.cellType.matAlt1 = generateInEditor.cellTypes[indexCellTypes].matAlt1;
                cellTwo.cellType.matAlt2 = generateInEditor.cellTypes[indexCellTypes].matAlt2;
                cellTwo.cellType.matAlt3 = generateInEditor.cellTypes[indexCellTypes].matAlt3;
                cellTwo.cellType.feedBackOnEmission = generateInEditor.cellTypes[indexCellTypes].feedBackOnEmission;
                cellTwo.cellType.feedBackOnMaterial = generateInEditor.cellTypes[indexCellTypes].feedBackOnMaterial;
                cellTwo.cellType.imAppliedToCell = true;


                cellTwo.UpdateCellType();
                EditorUtility.SetDirty(cellTwo);
            }
        }
    }
    public static void ApplyNewCellTypeToAll(int indexCellTypes)
    {
        for (int i = 0; i < generateInEditor.worldGenerate.transform.childCount; i++)
        {
            if (generateInEditor.worldGenerate.transform.GetChild(i).tag == "Cell")
            {
                CellTwo cellTwo = generateInEditor.worldGenerate.transform.GetChild(i).GetComponent<CellTwo>();
                Undo.RecordObject(cellTwo, "Update Cell");

                cellTwo.cellType.name = generateInEditor.cellTypes[indexCellTypes].name;
                cellTwo.cellType.isCancer = generateInEditor.cellTypes[indexCellTypes].isCancer;
                //cellTwo.cellType.color = generateInEditor.cellTypes[indexCellTypes].color;
                //cellTwo.cellType.speedUp = generateInEditor.cellTypes[indexCellTypes].speedUp;
                cellTwo.cellType.diffWithBasePosY = generateInEditor.cellTypes[indexCellTypes].diffWithBasePosY;
                cellTwo.GetComponent<MeshRenderer>().material = generateInEditor.cellTypes[indexCellTypes].mat;
                cellTwo.cellType.matAlt1 = generateInEditor.cellTypes[indexCellTypes].matAlt1;
                cellTwo.cellType.matAlt2 = generateInEditor.cellTypes[indexCellTypes].matAlt2;
                cellTwo.cellType.matAlt3 = generateInEditor.cellTypes[indexCellTypes].matAlt3;
                cellTwo.cellType.feedBackOnEmission = generateInEditor.cellTypes[indexCellTypes].feedBackOnEmission;
                cellTwo.cellType.feedBackOnMaterial = generateInEditor.cellTypes[indexCellTypes].feedBackOnMaterial;
                cellTwo.cellType.imAppliedToCell = true;


                cellTwo.UpdateCellType();
                EditorUtility.SetDirty(cellTwo);
            }
        }
    }

    #endregion
}
