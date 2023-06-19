using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities.Editor;

public class MyTable : SerializedScriptableObject
{
    [TableList]
    public List<MyData> data = new List<MyData>();

    [System.Serializable]
    public class MyData
    {
        [TableColumnWidth(30)]
        public int id;

        [TableColumnWidth(200)]
        public string name;

        [TableColumnWidth(100)]
        public int value;
    }


    [OnInspectorGUI]
    private void MyInspectorGUI()
    {
 

    }

}
