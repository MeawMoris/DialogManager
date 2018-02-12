using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class test : MonoBehaviour
{


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
	        entry.GetVisableWindow().Conversaton.Componets.ForEach(x=>x.IsInEditMode = !x.IsInEditMode);
    }

    void initializeEntry()
    {
        entry = ScriptableObject.CreateInstance<Entry>();
        entry.Componets.Add(EntryComponent.CreateInstance<EntryComponent>());
        entry.Componets.Add(EntryComponent.CreateInstance<EntryComponent>());
        entry.Componets.Add(EntryComponent.CreateInstance<EntryComponent>());
        entry.Componets.Add(EntryComponent.CreateInstance<EntryComponent>());

    }
}

