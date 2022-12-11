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

    public int modifier = 0;

    public Dictionary<int, int> diceCounts = new Dictionary<int, int>();

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
        Vector3 tray_pos = Camera.main.transform.position + 15 * Camera.main.transform.forward + 5 * Vector3.down;
        tray_pos = new Vector3((int)tray_pos.x, (int)tray_pos.y, (int)tray_pos.z);
        if (trays.ContainsKey(tray_pos))
        {
            if (!trays[tray_pos] || trays[tray_pos] == null)
            {
                trays.Remove(tray_pos);
            } else
            {
                new_tray = false;
                trays[tray_pos].GetComponent<DeleteInTime>().time = 2.2f;
            }
        }
        if (new_tray)
        {
            GameObject tray_obj = GameObject.Instantiate<GameObject>(DiceTray) as GameObject;
            tray_obj.transform.position = tray_pos;
            trays.Add(tray_pos, tray_obj);
            tray_obj.GetComponent<DeleteInTime>().time = 2.1f;
        }
        yield return new WaitForEndOfFrame();
        GameObject di_obj = GameObject.Instantiate<GameObject>(di.gameObject) as GameObject;
        di = di_obj.GetComponent<Di>();
        di_obj.transform.position = Camera.main.transform.position + 13 * Camera.main.transform.forward + 5 * Vector3.up;
        di_obj.transform.eulerAngles = new Vector3(Random.Range(-90, 90), Random.Range(-90, 90), Random.Range(-90, 90));
        di_obj.GetComponent<Rigidbody>().velocity += 10*Camera.main.transform.forward*Random.Range(0.8f, 1.2f) + 10*Camera.main.transform.right * Random.Range(-0.1f, 0.1f);
        di_obj.GetComponent<Rigidbody>().angularVelocity += new Vector3(Random.Range(-30, 30), Random.Range(-30, 30), Random.Range(-30, 30));

        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.21f);
            if (di_obj.GetComponent<Rigidbody>().velocity.magnitude < 1f || di_obj.GetComponent<Rigidbody>().angularVelocity.magnitude < 1f)
                break;
        }

        if (di_obj != null)
        {
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

            if (diceCounts.ContainsKey(n))
                diceCounts[n]++;
            else
                diceCounts.Add(n, 1);

            int num = (int)Random.Range(1, n + 1);
            rolled += num;
            UpdateRolledText();
            //Debug.Log("" + num + (num.ToString().Contains("6") || num.ToString().Contains("9") ? "." : ""));
            foreach (TextMesh text in di.text)
            {
                text.text = "" + num + (num.ToString().Contains("6") || num.ToString().Contains("9") ? "." : "");
            }

            //di_obj.transform.up = Vector3.up - 2 * (di_obj.transform.up + di.top_face.transform.up);

            di_obj.transform.eulerAngles = Vector3.zero - di.top_face.transform.localEulerAngles;
            //di_obj.transform.up = Vector3.up - di.top_face.transform.up;
            //di_obj.transform.up = Vector3.up - di.top_face.transform.up;
        }

    }

    public void UpdateRolledText()
    {
        diceRolled.text = "Dice Rolled:\n";
        foreach (int k in diceCounts.Keys)
        {
            diceRolled.text += diceCounts[k] + "d" + k + " + ";
        }
        diceRolled.text += modifier + " = " + rolled + " + " + modifier + " = " + (rolled + modifier);
    }
}
