using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Collections.Generic;

[InitializeOnLoad]
public class DrawGUILayout : Editor {

    // Script permettant d'avoir de la GUI dans la scène

    public static GenerateInEditor generateInEditor;
 
    
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
            SceneView.RepaintAll();
            DrawCustomBlockButtons(_sceneView);

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


}
