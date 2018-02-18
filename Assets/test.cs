using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class test : MonoBehaviour
{
    private bool editMode;

    [SerializeField] private Entry entry;
    //[SerializeField] private EntryComponent t3;
    //[SerializeField] private EntryComponent_StringField t2;
    // Use this for initialization
	void Start () {
	  
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (Input.GetKeyDown(KeyCode.Z))
	        initializeEntry();
	    if (Input.GetKeyDown(KeyCode.X))
	        entry.GetVisableWindow().Show();
	    if (Input.GetKeyDown(KeyCode.C))
	    {
	        editMode = !editMode;
            entry.GetVisableWindow().Conversaton.Componets.ForEach(x=>x.IsInEditMode = editMode);
	    }
        if(Input.GetKeyDown(KeyCode.A))
            System.GC.Collect();

    }

    void initializeEntry()
    {
        entry = ScriptableObject.CreateInstance<Entry>();

    }
}

