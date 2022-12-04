using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRoller : MonoBehaviour {

    public static Dictionary<Vector3, GameObject> trays = new Dictionary<Vector3, GameObject>();

    /*[System.Serializable]
    public struct Di
    {
        public int n;
        public GameObject model;
    }*/

    public Di[] dice;
    public GameObject DiceTray;

    public TextMesh diceRolled;

    public int rolled = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void roll(int sides)
    {
        Di di = new Di();
        di.n = -1;
        for (int i = 0; i < dice.Length; i++)
        {
            if (dice[i] == null)
                continue;
            if (dice[i].n == sides)
            {
                di = dice[i];
                break;
            }
        }
        if (di.n == -1)
        {
            for (int i = 0; i < dice.Length; i++)
            {
                if (dice[i] == null)
                    continue;
                if (dice[i].n > sides)
                {
                    di = dice[i];
                    break;
                }
            }
        }
        if (di.n == -1)
        {
            int max_n = 0;
            foreach (Di d in dice)
            {
                if (d.n > di.n) {
                    di = d;
                }
            }
        }

        StartCoroutine(RollIt(di, sides));

    }

    IEnumerator RollIt(Di di, int n)
    {
        bool new_tray = true;
        foreach (TextMesh text in di.text)
        {
            text.text = "";
        }
        Vector3 tray_pos = Camera.main.transform.position + 5 * Camera.main.transform.forward + 10 * Vector3.down;
        tray_pos = new Vector3((int)tray_pos.x, (int)tray_pos.y, (int)tray_pos.z);
        if (trays.ContainsKey(tray_pos))
        {
            if (!trays[tray_pos] || trays[tray_pos] == null)
            {
                trays.Remove(tray_pos);
            } else
            {
                new_tray = false;
                trays[tray_pos].GetComponent<DeleteInTime>().time = 2.6f;
            }
        }
        if (new_tray)
        {
            GameObject tray_obj = GameObject.Instantiate<GameObject>(DiceTray) as GameObject;
            tray_obj.transform.position = tray_pos;
            trays.Add(tray_pos, tray_obj);
            tray_obj.GetComponent<DeleteInTime>().time = 2.6f;
        }
        yield return new WaitForEndOfFrame();
        GameObject di_obj = GameObject.Instantiate<GameObject>(di.gameObject) as GameObject;
        di = di_obj.GetComponent<Di>();
        di_obj.transform.position = Camera.main.transform.position + 4 * Camera.main.transform.forward;
        di_obj.GetComponent<Rigidbody>().velocity += 10*Camera.main.transform.forward*Random.Range(0.8f, 1.2f) + 10*Camera.main.transform.right * Random.Range(-0.1f, 0.1f);
        di_obj.GetComponent<Rigidbody>().angularVelocity += new Vector3(Random.Range(-15, 15), Random.Range(-15, 15), Random.Range(-15, 15));

        yield return new WaitForSeconds(2.5f);

        //for (int i = 0; i < 30; i++)
        //{
            //yield return new WaitForSeconds(0.1f);
            //di_obj.transform.eulerAngles += 0.1f * (Vector3.up - di_obj.GetComponent<Di>().transform.up).normalized;
            //di_obj.GetComponent<Rigidbody>().rotation = Quaternion.Euler(di_obj.GetComponent<Rigidbody>().rotation.eulerAngles + 0.1f * (Vector3.up - di_obj.GetComponent<Di>().transform.up).normalized);
            //Debug.Log((Vector3.up - di_obj.GetComponent<Di>().transform.up).normalized);
        //}

        di_obj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        di_obj.GetComponent<Rigidbody>().velocity = Vector3.zero;

        //GameObject.Destroy(tray_obj);
        Destroy(di_obj.GetComponent<Rigidbody>());
        di_obj.GetComponent<Collider>().isTrigger = true;

        int num = (int)Random.Range(1, n + 1);
        rolled += num;
        diceRolled.text = "Roll dice: " + rolled;
        Debug.Log("" + num + (num.ToString().Contains("6") || num.ToString().Contains("9") ? "." : ""));
        foreach (TextMesh text in di.text)
        {
            text.text = "" + num + (num.ToString().Contains("6") || num.ToString().Contains("9") ? "." : "");
        }

        //di_obj.transform.up = Vector3.up - 2 * (di_obj.transform.up + di.top_face.transform.up);

        di_obj.transform.eulerAngles =  Vector3.zero - di.top_face.transform.localEulerAngles;
        //di_obj.transform.up = Vector3.up - di.top_face.transform.up;
        //di_obj.transform.up = Vector3.up - di.top_face.transform.up;

    }
}
