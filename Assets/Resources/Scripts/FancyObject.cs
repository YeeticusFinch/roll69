using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FancyObject : NetworkBehaviour
{

    public string name;
    public bool flat = true;
    public string owner;
    public int hp;
    public int ac;
    public string id;
    public long timestamp;
    public string display;

    GameObject model;

    // Start is called before the first frame update
    void Start()
    {

    }

    void InitNew()
    {
        id = CarlMath.RandomString(Random.Range(6, 10));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Deserialize(string input)
    {
        int i = 0;
        while (i < input.Length)
        {
            string sel = input.Substring(i, input.IndexOf(':') - i);
            if (sel[0] == ';') sel = sel.Substring(1);
            if (sel[sel.Length - 1] == ':') sel = sel.Substring(0, sel.Length - 1);
            string val = input.Substring(input.IndexOf(':') + 1, input.IndexOf(';', i + 2) - (input.IndexOf(':') + 1));
            if (val[0] == ':') sel = val.Substring(1);
            if (val[sel.Length - 1] == ';') val = sel.Substring(0, sel.Length - 1);
            i = input.IndexOf(';', i + 2) + 1;
            switch (sel)
            {
                case "id":
                    id = val;
                    break;
                case "name":
                    name = val;
                    break;
                case "flat":
                    if (System.String.Equals(val, "1"))
                        flat = true;
                    else
                        flat = false;
                    break;
                case "owner":
                    owner = val;
                    break;
                case "hp":
                    hp = int.Parse(val);
                    break;
                case "ac":
                    ac = int.Parse(val);
                    break;
                case "xpos":
                    transform.position = new Vector3(float.Parse(val), transform.position.y, transform.position.z);
                    break;
                case "ypos":
                    transform.position = new Vector3(transform.position.x, float.Parse(val), transform.position.z);
                    break;
                case "zpos":
                    transform.position = new Vector3(transform.position.x, transform.position.y, float.Parse(val));
                    break;
                case "xsiz":
                    transform.localScale = new Vector3(float.Parse(val), transform.localScale.y, transform.localScale.z);
                    break;
                case "ysiz":
                    transform.localScale = new Vector3(transform.localScale.x, float.Parse(val), transform.localScale.z);
                    break;
                case "zsiz":
                    transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, float.Parse(val));
                    break;
                case "xeul":
                    transform.eulerAngles = new Vector3(float.Parse(val), transform.eulerAngles.y, transform.eulerAngles.z);
                    break;
                case "yeul":
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, float.Parse(val), transform.eulerAngles.z);
                    break;
                case "zeul":
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, float.Parse(val));
                    break;
                case "timestamp":
                    timestamp = long.Parse(val);
                    break;
                case "display":
                    display = val;
                    break;
            }
        }
    }

    string Serialize()
    {
        return "id:" + id + ";name:" + name + ";flat:" + (flat ? 1 : 0) + ";owner:" + owner + ";hp:" + hp + ";ac:" + ac + ";xpos:" + transform.position.x + ";ypos:" + transform.position.y + ";zpos:" + transform.position.z + ";xsiz:" + transform.localScale.x + ";ysiz:" + transform.localScale.y + ";zsiz:" + transform.localScale.z + ";xeul:" + transform.eulerAngles.x + ";yeul:" + transform.eulerAngles.y + ";zeul:" + transform.eulerAngles.z + ";timestamp:" + System.DateTimeOffset.Now.ToUnixTimeMilliseconds() + ";display:" + display + ";";
    }

}
