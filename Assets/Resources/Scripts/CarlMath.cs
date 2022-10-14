using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarlMath : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Range(0, s.Length)]).ToArray());
    }
}
