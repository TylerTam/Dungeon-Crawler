#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;
using System.Collections.Generic;

[EditorTool("Platform Tool")]
public class AnimatorControllerAssigner : EditorWindow
{
    public List<AnimationClip> m_animationList;
    private Object m_referenceController;
    private Object m_changedController;

    ScriptableObject target;
    SerializedObject so;
    SerializedProperty animationProperty;

    [MenuItem("Editor Tools/Example")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<AnimatorControllerAssigner>("Animator Overrider Assigner");
    }
    private void OnEnable()
    {
        //var so = new SerializedObject();
        //m_list = new ReorderableList(serializedObject)
        target = this;
        so = new SerializedObject(target);
        animationProperty = so.FindProperty("m_animationList");
    }
    private void OnGUI()
    {
        #region Get Controllers
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Reference Controller");
        m_referenceController = EditorGUILayout.ObjectField(m_referenceController, typeof(RuntimeAnimatorController), false);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Override Controller");
        m_changedController = EditorGUILayout.ObjectField(m_changedController, typeof(AnimatorOverrideController), false);
        EditorGUILayout.EndHorizontal();
        #endregion


        #region List
        so.ApplyModifiedProperties();
        EditorGUILayout.PropertyField(animationProperty, true);
        #endregion

        #region Buttons

        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Assign"))
        {
            AnimatorOverrideController temp = (AnimatorOverrideController)m_changedController;
            RuntimeAnimatorController reference = (RuntimeAnimatorController)m_referenceController;
            List<string> animationNames = new List<string>();
            Debug.Log(reference.animationClips.Length);
            foreach(AnimationClip clip in reference.animationClips)
            {
                animationNames.Add(clip.name);
            }

            animationNames.Sort();

            for (int i = 0; i < temp.animationClips.Length; i++)
            {
                temp[animationNames[i]] = m_animationList[i];
                Debug.Log(temp.animationClips[i].name);
                Debug.Log("Successful Change? | " + (temp[temp.animationClips[i]] == m_animationList[i]));
            }


        }

        if(GUILayout.Button("Clear"))
        {
            AnimatorOverrideController temp = (AnimatorOverrideController)m_changedController;
            for (int i = 0; i < temp.animationClips.Length; i++)
            {
                Debug.Log(temp.animationClips[i].name);
                temp[temp.animationClips[i].name] = null;
            }
        }
        EditorGUILayout.EndHorizontal();
        #endregion
    }
}
#endif

