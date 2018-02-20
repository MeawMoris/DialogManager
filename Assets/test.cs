using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class test : MonoBehaviour
{
    private bool editMode;
    [SerializeField] private Entry_ComponentsEntryTemplate entry;


	void Start () {
	    initializeEntry();
    }
	
	// Update is called once per frame
	void Update ()
	{

	    if (Input.GetKeyDown(KeyCode.X))
	        entry.GetVisableWindow().Show();
	    if (Input.GetKeyDown(KeyCode.C))
	    {
	        entry.ShowAddButton = entry.ShowEditModeOption = editMode = !editMode;

	    }
        if(Input.GetKeyDown(KeyCode.A))
            System.GC.Collect();

    }

    void initializeEntry()
    {
        entry = ScriptableObject.CreateInstance<Entry_ComponentsEntryTemplate>();

    }
}

