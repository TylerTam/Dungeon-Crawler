#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;
using System.Collections.Generic;
using System.Text.RegularExpressions;

[EditorTool("Platform Tool")]
public class DungeonRoomAssigner : EditorWindow
{
    public TextAsset m_referencedTextFile;
    private Object m_ref;
    private Object m_assignRoomLayout;

    ScriptableObject target;
    SerializedObject so;
    SerializedProperty refText;

    [MenuItem("Editor Tools/Dungeon Room Assigner")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<DungeonRoomAssigner>("Dungeom Room Assigner");
    }
    private void OnEnable()
    {
        //var so = new SerializedObject();
        //m_list = new ReorderableList(serializedObject)
        target = this;
        so = new SerializedObject(target);
        refText = so.FindProperty("m_referencedTextFile");
    }
    private void OnGUI()
    {
        #region Get Controllers
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Reference Text File");
        m_ref = EditorGUILayout.ObjectField(m_ref, typeof(TextAsset), false);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Override Controller");
        m_assignRoomLayout = EditorGUILayout.ObjectField(m_assignRoomLayout, typeof(DungeonGeneration_RoomLayout), false);
        EditorGUILayout.EndHorizontal();
        #endregion


        #region List
        so.ApplyModifiedProperties();
        EditorGUILayout.PropertyField(refText, true);
        #endregion

        #region Buttons

        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Assign"))
        {

            if (m_referencedTextFile && m_assignRoomLayout)
            {

                DungeonGeneration_RoomLayout room = ((DungeonGeneration_RoomLayout)m_assignRoomLayout);
                room.m_northExit = room.m_southExit = room.m_westExit = room.m_eastExit = false;

                List<DungeonGeneration_RoomLayout.RoomGridRow> returnRow = new List<DungeonGeneration_RoomLayout.RoomGridRow>();
                DungeonGeneration_RoomLayout.RoomGridData newData;
                DungeonGeneration_RoomLayout.RoomGridRow newRow;

                string[] fLines = Regex.Split(m_referencedTextFile.text, "\n");
                string[] splitLines = Regex.Split(fLines[0], " "); ;
                for (int x = 0; x < fLines.GetLength(0); x++)
                {
                    newRow = new DungeonGeneration_RoomLayout.RoomGridRow();
                    newRow.m_roomRowData = new List<DungeonGeneration_RoomLayout.RoomGridData>();
                    splitLines = Regex.Split(fLines[x], " ");
                    for (int y = 0; y < splitLines.GetLength(0); y++)
                    {
                        newData = new DungeonGeneration_RoomLayout.RoomGridData();

                        newData.m_gridPosition = new Vector2Int(y,x);
                        int integer = 0;
                        if (int.TryParse(splitLines[y], out integer))
                        {
                            newData.m_cellType = int.Parse(splitLines[y]);
                        }
                        else
                        {
                            newData.m_cellType = 1;
                            if(splitLines[y] == "n" || splitLines[y] == "n\r")
                            {
                                room.m_northExit = true;
                                room.m_northExitPos = new Vector2Int(y, x);
                            }
                            if (splitLines[y] == "s" || splitLines[y] == "s\r")
                            {
                                room.m_southExit = true;
                                room.m_southExitPos = new Vector2Int(y, x);
                            }
                            if (splitLines[y] == "e" || splitLines[y] == "e\r")
                            {
                                room.m_eastExit = true;
                                room.m_eastExitPos = new Vector2Int(y, x);
                            }
                            if (splitLines[y] == "w" || splitLines[y] == "w\r")
                            {
                                room.m_westExit = true;
                                room.m_westExitPos = new Vector2Int(y, x);
                            }
                        }
                        
                        newRow.m_roomRowData.Add(newData);
                    }
                    returnRow.Add(newRow);
                }
                room.m_roomGridData = returnRow;
                room.m_roomSize = new Vector2Int(splitLines.Length, fLines.Length);
            }

        }

        if (GUILayout.Button("Clear"))
        {
            DungeonGeneration_RoomLayout reff = (DungeonGeneration_RoomLayout)m_assignRoomLayout;
            reff.m_northExit = reff.m_southExit = reff.m_westExit = reff.m_eastExit = false;
            reff.m_northExitPos = reff.m_southExitPos = reff.m_eastExitPos = reff.m_westExitPos = Vector2Int.zero;
            reff.m_roomGridData.Clear();
        }
        EditorGUILayout.EndHorizontal();
        #endregion
    }
}
#endif

