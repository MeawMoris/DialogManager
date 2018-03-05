using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class test : MonoBehaviour
{
    private bool editMode;
    [SerializeField] private Entry_ComponentsEntryTemplate entry;


	void Start () {
    }
	
	// Update is called once per frame
	void Update ()
	{

	    if (Input.GetKeyDown(KeyCode.Z))
	    {
	        initializeEntry();
            entry.GetNewWindow().ShowUtility();
	    }
	    if (Input.GetKeyDown(KeyCode.X))	    
	        entry.AddObserver().GetNewWindow().Show();

	    if (Input.GetKeyDown(KeyCode.A))
	    {
	        entry= AssetDatabase.LoadAssetAtPath<Entry_ComponentsEntryTemplate>("Assets/saveTemp.asset");
	        entry.GetNewWindow().ShowUtility();
            // EditorUtility.FocusProjectWindow();
            // Selection.activeObject = entry;


        }



    }

    private int i = 0;
    private UnityEngine.Object temp;

    void initializeEntry()
    {
        entry = EntryBase.CreateInstance<Entry_ComponentsEntryTemplate>();

    }
}

