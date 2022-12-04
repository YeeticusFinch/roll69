using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteInTime : MonoBehaviour {

    public float time = 1000;

	// Use this for initialization
	void Start () {
		
	}
	
	void FixedUpdate () {
        time -= Time.fixedDeltaTime;
        if (time <= 0)
        {
            if (DiceRoller.trays.ContainsKey(transform.position))
                DiceRoller.trays.Remove(transform.position);
            GameObject.Destroy(this.gameObject);
        }
	}
}
