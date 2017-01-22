using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

// permet d'avoir les custom inspector modifié en gras
public class EditorGUIHelper  {

    static MethodInfo boldFontMethodInfo = null;

    public static void SetBoldDefaultFont ( bool value)
    {
        if (boldFontMethodInfo == null)
        {
            boldFontMethodInfo = typeof(EditorGUIUtility).GetMethod("SetBoldDefaultFont", BindingFlags.Static | BindingFlags.NonPublic);
        }
        boldFontMethodInfo.Invoke(null, new[] { value as object });
    }
}
