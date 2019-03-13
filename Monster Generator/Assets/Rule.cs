using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rule : MonoBehaviour
{
    public char input;
    string result;

    public Rule() { }

	public Rule(char input, string result)
    {
        this.input = input;
        this.result = result;
    }
	
	
    virtual public string GiveResult()
    {
        return result;
    }
}
