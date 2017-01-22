using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateInEditor))]
public class CustomCellsEditor : Editor {

    //Ce script correspond à la partie inspecteur du tool, on va modifier le script GenerateInEditor pour qu'il affiche ce que l'on veut

    //Correspond au script sur lequel on va appliquer la GUI a chaque instant
    GenerateInEditor m_targetedScript;

    // Forme d'update qui va appeler les principales fonction de GUI à chaque frame
    public override void OnInspectorGUI()
    {
        m_targetedScript = (GenerateInEditor)target;
        DrawDefaultInspector();
        DrawCellsEditInspector();
    }

    // principale Fonction qui va appeler le reste à chaque update
    void DrawCellsEditInspector()
    {
        GUILayout.Space(5);
        GUILayout.Label("[ Generate World Buttons ]", EditorStyles.boldLabel);
        // On draw les boutons de creation du monde
        DrawCreateCleanWorld();
        GUILayout.Space(5);
        GUILayout.Label("[ Cell Types ]", EditorStyles.boldLabel);
        GUILayout.Space(5);
        GUILayout.Label("[ Order : Name / IsWithFeedBackEmission/ IsWithFeedBackMaterial / Material / Mat Alt1 / Mat Alt2 / Mat Alt3 / SpeedGrow / Altitude ]");
        
        GUILayout.Space(5);

        // On draw autant de cellType qu'il y en a dans la liste du GenerateInEditor
        for (int i = 0; i < m_targetedScript.cellTypes.Count; i++)
        {
            DrawCellType(i);
                
        }

        // On draw le bouton permettant de créer un nouveau CellType
        DrawAddNewCellTypeButton();
    }

    void DrawCreateCleanWorld()
    {
        GUILayout.Space(5);
        if (GUILayout.Button("Generate World",GUILayout.Height(30)))
        {
            m_targetedScript.CreateWorld();
        }
        GUILayout.Space(5);
        if (GUILayout.Button("Clean World", GUILayout.Height(30)))
        {
            m_targetedScript.CleanWorld();
        }
    }

    // A chaque ligne, on va draw la liste des variables ciblées de CellType sous la forme de GUI pouvant etre modifié
    void DrawCellType(int index)
    {
        // permet de ne rien faire si la liste de CellType dans Generate un Editor est vide
        if(index < 0 || index >= m_targetedScript.cellTypes.Count)
        {
            return;
        }

        // On commence le draw
        GUILayout.BeginHorizontal();
        {

            
            // BeginChangeCheck et EndChangeCheck permet de voir si la variable a été modifié
            EditorGUI.BeginChangeCheck();
            string newName = GUILayout.TextField(m_targetedScript.cellTypes[index].name, GUILayout.Width(120));
            Texture tex = null;

            //Color newColor = EditorGUILayout.ColorField("",m_targetedScript.cellTypes[index].color, GUILayout.Width(120));

            bool newBoolFeedbackEmission = GUILayout.Toggle(m_targetedScript.cellTypes[index].feedBackOnEmission,tex, GUILayout.Width(20));
            bool newBoolFeedbackMaterial = GUILayout.Toggle(m_targetedScript.cellTypes[index].feedBackOnMaterial, tex, GUILayout.Width(20));

            Material newMaterial = (Material)EditorGUILayout.ObjectField( m_targetedScript.cellTypes[index].mat, typeof(Material), true, GUILayout.Width(80));
            Material newMaterialAlt1 = (Material)EditorGUILayout.ObjectField(m_targetedScript.cellTypes[index].matAlt1, typeof(Material), true, GUILayout.Width(80));
            Material newMaterialAlt2 = (Material)EditorGUILayout.ObjectField(m_targetedScript.cellTypes[index].matAlt2, typeof(Material), true, GUILayout.Width(80));
            Material newMaterialAlt3 = (Material)EditorGUILayout.ObjectField(m_targetedScript.cellTypes[index].matAlt3, typeof(Material), true, GUILayout.Width(80));

            

            float newSpeedUp = EditorGUILayout.FloatField("", m_targetedScript.cellTypes[index].speedUp, GUILayout.Width(40));
            
            int newStartY = EditorGUILayout.IntField("", m_targetedScript.cellTypes[index].diffWithBasePosY, GUILayout.Width(40));

            if(newBoolFeedbackEmission == true)
            {
                newBoolFeedbackMaterial = false;
            }
            if(newBoolFeedbackMaterial ==true)
            {
                newBoolFeedbackEmission = false;
            }

            // si une variable entre le Begin et le End a été modifié, on update Toute les cellules
            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_targetedScript, "Modified CellType");

                m_targetedScript.cellTypes[index].name = newName;
               // m_targetedScript.cellTypes[index].color = newColor;
                m_targetedScript.cellTypes[index].speedUp = newSpeedUp;
                m_targetedScript.cellTypes[index].mat = newMaterial;
                m_targetedScript.cellTypes[index].matAlt1= newMaterialAlt1;
                m_targetedScript.cellTypes[index].matAlt2 = newMaterialAlt2;
                m_targetedScript.cellTypes[index].matAlt3 = newMaterialAlt3;
                m_targetedScript.cellTypes[index].diffWithBasePosY = newStartY;
                m_targetedScript.cellTypes[index].feedBackOnEmission = newBoolFeedbackEmission;
                m_targetedScript.cellTypes[index].feedBackOnMaterial = newBoolFeedbackMaterial;

                EditorUtility.SetDirty(m_targetedScript);
                UpdateAllCellTypes();
                
            }
            EditorGUIHelper.SetBoldDefaultFont(false);

            // Pour finir, on draw le bouton remove et affiche une fenetre de confirmation
            if(GUILayout.Button("Remove"))
            {
                EditorApplication.Beep();
                if(EditorUtility.DisplayDialog("Really?", "Do you really want to remove the cellType" + m_targetedScript.cellTypes[index].name + "?", "Yes", "No") == true)
                {
                    Undo.RecordObject(m_targetedScript, "Delete CellType");
                    m_targetedScript.cellTypes.RemoveAt(index);
                    EditorUtility.SetDirty(m_targetedScript);
                }
            }
        }
        // On finis le draw
        GUILayout.EndHorizontal();
    }
    // fonction permettant de draw le bouton pour creer un nouveau CellType et de l'ajouter à la mainList
    void DrawAddNewCellTypeButton()
    {
        if(GUILayout.Button("Add New CellType", GUILayout.Height(30)))
        {
            Undo.RecordObject(m_targetedScript, "Add new CellType");

            m_targetedScript.cellTypes.Add(new CellType { name = "New CellType" });

            EditorUtility.SetDirty(m_targetedScript);
        }
    }

   // Ici On update toute les cellules lorsque une variable a été modifié dans la GUI
    public void UpdateAllCellTypes()
    {
        for (int i = 0; i < m_targetedScript.worldGenerate.transform.childCount; i++)
        {
            if (m_targetedScript.worldGenerate.transform.GetChild(i).name != "Cell")
            {
                for (int j = 0; j < m_targetedScript.cellTypes.Count; j++)
                {
                    if (m_targetedScript.worldGenerate.transform.GetChild(i).GetComponent<CellTwo>().cellType.name == m_targetedScript.cellTypes[j].name)
                    {
                        CellTwo cellTwoTemp = m_targetedScript.worldGenerate.transform.GetChild(i).GetComponent<CellTwo>();
                        CellType cellTypeTemp = m_targetedScript.cellTypes[j];

                        Undo.RecordObject(cellTwoTemp, "Update Cell");
                        //cellTwoTemp.cellType.color = cellTypeTemp.color;
                        cellTwoTemp.cellType.speedUp = cellTypeTemp.speedUp;
                        cellTwoTemp.cellType.diffWithBasePosY = cellTypeTemp.diffWithBasePosY;
                        cellTwoTemp.cellType.feedBackOnEmission = cellTypeTemp.feedBackOnEmission;
                        cellTwoTemp.cellType.feedBackOnMaterial = cellTypeTemp.feedBackOnMaterial;
                        cellTwoTemp.UpdateCellType();
                        EditorUtility.SetDirty(cellTwoTemp);

                    }
                }
            }
        }
    }

}
