using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Category
{

    [SerializeField] private string name;
 

    public Category(string catigoryName)
    {
        this.Name = catigoryName;
    }

    public string Name
    {
        get { return name; }
        set { name = value; }
    }


}


public class ConversationCategory : Category
{
    public ConversationCategory(string catigoryName) : base(catigoryName)
    {
    }
}

public class DatabaseCategory : Category
{
    public DatabaseCategory(string catigoryName) : base(catigoryName)
    {
    }
}