using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FancyObject : NetworkBehaviour
{

    public string title;
    public bool flat = false;
    public string owner;
    public int hp;
    public int ac;
    public string id;
    public long timestamp;
    public string sprite0;
    public string sprite1;
    public string sprite2;
    public string sprite3;
    public string sprite4;
    public string sprite5;
    public string model0;
    public string model1;
    public string model2;
    public string model3;
    public Vector3 modelEulers;
    public Sprite[] sprites = new Sprite[6];
    public GameObject[] models = new GameObject[4];

    public GameObject display;
    public int currentDisplay;
    public bool dm_only = true;
    public bool hide = false;
    public float feetMoved = 0;

    public Vector3 ojPos;

    public List<string> tags = new List<string>();

    public List<Vector3> pastPositions = new List<Vector3>();

    public List<string> sounds = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        pastPositions = new List<Vector3>();
        ojPos = transform.position;
        while (GetSprite(currentDisplay) == null && GetModel(currentDisplay - sprites.Length) == null)
        {
            currentDisplay++;
            currentDisplay %= sprites.Length + models.Length;
        }
        UpdateDisplay();
        modelEulers = Vector3.zero;
    }

    private int lastFeetMoved = -1;

    public float Displace(Vector3 displacement)
    {
        transform.position += displacement;
        feetMoved += displacement.magnitude * 0.91954f;
        if ((int)feetMoved % 5 == 0 && (int)feetMoved > lastFeetMoved)
        {
            pastPositions.Add(transform.position);
            lastFeetMoved = (int)feetMoved;
        }
        return feetMoved;
    }

    public Vector3 FallBack()
    {
        int newDistance = pastPositions.Count*5;
        if (pastPositions.Count == 0)
        {
            Vector3 disp = transform.position - ojPos;
            transform.position = ojPos;
            feetMoved = 0;
            lastFeetMoved = -1;
            return disp;
        }
        else
        {
            Vector3 disp = transform.position - pastPositions[pastPositions.Count - 1];
            transform.position = pastPositions[pastPositions.Count - 1];
            pastPositions.Remove(transform.position);
            feetMoved = pastPositions.Count * 5;
            lastFeetMoved -= 5;
            return disp;
        }
    }

    public void UpdateDisplay()
    {
        if (hide)
        {
            if (GameObject.FindGameObjectWithTag("MainCamera").gameObject.GetComponent<FancyCam>().dm)
            {
                GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f);
            }
            else
            {
                this.GetComponent<SpriteRenderer>().sprite = null;
                if (display != null)
                {
                    GameObject.Destroy(display);
                    display = null;
                }
                return;

            }
        } else
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        if (currentDisplay < sprites.Length)
        {
            if (display != null)
            {
                GameObject.Destroy(display);
                display = null;
            }
            this.GetComponent<SpriteRenderer>().sprite = GetSprite(currentDisplay);
            //Debug.Log(GetSprite(currentDisplay));
        } else
        {
            GetComponent<SpriteRenderer>().sprite = null;
            if (display != null)
            {
                GameObject.Destroy(display);
            }
            GameObject fancyYeetBoyo = GameObject.Instantiate<GameObject>(GetModel(currentDisplay - sprites.Length), transform.position, Quaternion.identity);
            try
            {
                //fancyYeetBoyo.transform.parent = (gameObject.transform);
            }
            catch (System.Exception e)
            {

            }
            display = fancyYeetBoyo;
        }
    }

    public void NextSprite()
    {
        currentDisplay++;
        while (GetSprite(currentDisplay) == null && GetModel(currentDisplay - sprites.Length) == null)
        {
            currentDisplay++;
            currentDisplay %= sprites.Length + models.Length;
        }
        Debug.Log("Switching to " + currentDisplay + " which is " + GetSprite(currentDisplay) + " and " + GetModel(currentDisplay));
        UpdateDisplay();
    }

    Sprite GetSprite(int n)
    {
        if (n >= sprites.Length)
            return null;
        if (sprites[n] == null)
        {
            sprites[n] = Resources.Load(GetDisplay(n), typeof(Sprite)) as Sprite;
        }
        
        return sprites[n];
    }

    GameObject GetModel(int n)
    {
        if (n >= models.Length || n < 0)
            return null;
        else if (models[n] == null)
        {
            //Debug.Log("Getting display " + GetDisplay(sprites.Length + n));
            models[n] = Resources.Load(GetDisplay(sprites.Length + n)) as GameObject;
        }
        return models[n];
    }

    string GetDisplay(int n)
    {
        string result = null;
        int c = 0;
        do
        {
            c++;
            switch (n)
            {
                case 0:
                    result = sprite0;
                    break;
                case 1:
                    result = sprite1;
                    break;
                case 2:
                    result = sprite2;
                    break;
                case 3:
                    result = sprite3;
                    break;
                case 4:
                    result = sprite4;
                    break;
                case 5:
                    result = sprite5;
                    break;
                case 6:
                    result = model0;
                    break;
                case 7:
                    result = model1;
                    break;
                case 8:
                    result = model2;
                    break;
                case 9:
                    result = model3;
                    break;
            }
            if (c > 10) break;
        } while (result == null || result.Equals(""));
        return result;
    }

    void InitNew()
    {
        id = CarlMath.RandomString(Random.Range(6, 10)).Replace(';', 'l').Replace(':','k').Replace('{','j').Replace('}','h');
    }

    // Update is called once per frame
    void Update()
    {
        if (!flat && currentDisplay < sprites.Length)
        {
            //transform.LookAt(GameObject.FindGameObjectWithTag("MainCamera").transform);
            transform.eulerAngles = GameObject.FindGameObjectWithTag("MainCamera").transform.eulerAngles;
        }
    }

    public void Deserialize(string input)
    {
        pastPositions = new List<Vector3>();
        sounds = new List<string>();
        int i = 0;
        feetMoved = 0;
        model0 = null;
        model1 = null;
        model2 = null;
        sprite0 = null;
        sprite1 = null;
        sprite2 = null;
        sprite3 = null;
        sprite4 = null;
        sprite5 = null;
        while (i < input.Length && input.IndexOf(':', i) != -1)
        {
            /*
            string sel = input.Substring(i, input.IndexOf(':', i) - i);
            if (sel[0] == ';') sel = sel.Substring(1);
            if (sel[sel.Length - 1] == ':') sel = sel.Substring(0, sel.Length - 1);
            string val = input.Substring(input.IndexOf(':', i) + 1, input.IndexOf(';', i + 1) - (input.IndexOf(':', i) + 1));
            if (val[0] == ':') sel = val.Substring(1);
            if (val[sel.Length - 1] == ';') val = sel.Substring(0, sel.Length - 1);
            i = input.IndexOf(';', i + 2) + 1;
            */
            string sel = input.Substring(i, input.IndexOf(':', i) - i);
            if (sel[0] == ';') sel = sel.Substring(1);
            if (sel[sel.Length - 1] == ':') sel = sel.Substring(0, sel.Length - 1);
            string val = input.Substring(input.IndexOf(':', i) + 1, input.IndexOf(';', i) - (input.IndexOf(':', i) + 1));
            i = input.IndexOf(';', i)+1;
            switch (sel)
            {
                case "tag":
                    tags.Add(val);
                    break;
                case "sound":
                    sounds.Add(val);
                    break;
                case "id":
                    id = val;
                    break;
                case "name":
                    name = val.Replace("(Clone)", "");
                    title = val.Replace("(Clone)", "");
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
                    //Debug.Log("HP = " + val);
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
                case "sprite0":
                    if (val == null || val.Equals(""))
                        break;
                    sprite0 = val;
                    sprites[0] = Resources.Load(val) as Sprite;
                    break;
                case "sprite1":
                    if (val == null || val.Equals(""))
                        break;
                    sprite1 = val;
                    sprites[1] = Resources.Load(val) as Sprite;
                    break;
                case "sprite2":
                    if (val == null || val.Equals(""))
                        break;
                    sprite2 = val;
                    sprites[2] = Resources.Load(val) as Sprite;
                    break;
                case "sprite3":
                    if (val == null || val.Equals(""))
                        break;
                    sprite3 = val;
                    sprites[3] = Resources.Load(val) as Sprite;
                    break;
                case "sprite4":
                    if (val == null || val.Equals(""))
                        break;
                    sprite4 = val;
                    sprites[4] = Resources.Load(val) as Sprite;
                    break;
                case "sprite5":
                    if (val == null || val.Equals(""))
                        break;
                    sprite5 = val;
                    sprites[5] = Resources.Load(val) as Sprite;
                    break;
                case "model0":
                    if (val == null || val.Equals(""))
                        break;
                    model0 = val;
                    models[0] = Resources.Load(val) as GameObject;
                    break;
                case "model1":
                    if (val == null || val.Equals(""))
                        break;
                    model1 = val;
                    models[1] = Resources.Load(val) as GameObject;
                    break;
                case "model2":
                    if (val == null || val.Equals(""))
                        break;
                    model2 = val;
                    models[2] = Resources.Load(val) as GameObject;
                    break;
                case "model3":
                    if (val == null || val.Equals(""))
                        break;
                    model3 = val;
                    models[3] = Resources.Load(val) as GameObject;
                    break;
                case "currentDisplay":
                    currentDisplay = int.Parse(val);
                    break;
                case "hide":
                    hide = bool.Parse(val);
                    break;
                case "dm_only":
                    dm_only = bool.Parse(val);
                    break;
            }
        }
        UpdateDisplay();
        ojPos = transform.position;
    }

    public string Serialize()
    {
        pastPositions = new List<Vector3>();
        feetMoved = 0;
        string serializedTags = "";
        foreach (string tag in tags)
        {
            serializedTags += ";tag:" + tag;
        }
        foreach (string sound in sounds)
        {
            serializedTags += ";sound:" + sound;
        }
        return "id:" + id
            + ";name:" + name.Replace("(Clone)", "")
            + ";flat:" + (flat ? 1 : 0)
            + ";owner:" + owner
            + ";hp:" + hp
            + ";ac:" + ac
            + ";xpos:" + transform.position.x
            + ";ypos:" + transform.position.y
            + ";zpos:" + transform.position.z
            + ";xsiz:" + transform.localScale.x
            + ";ysiz:" + transform.localScale.y
            + ";zsiz:" + transform.localScale.z
            + ";xeul:" + transform.eulerAngles.x
            + ";yeul:" + transform.eulerAngles.y
            + ";zeul:" + transform.eulerAngles.z
            + ";timestamp:" + System.DateTimeOffset.Now.ToUnixTimeMilliseconds()
            + (sprite0 != null && sprite0.Length > 0 ? ";sprite0:" + sprite0 : "")
            + (sprite1 != null && sprite1.Length > 0 ? ";sprite1:" + sprite1 : "")
            + (sprite2 != null && sprite2.Length > 0 ? ";sprite2:" + sprite2 : "")
            + (sprite3 != null && sprite3.Length > 0 ? ";sprite3:" + sprite3 : "")
            + (sprite4 != null && sprite4.Length > 0 ? ";sprite4:" + sprite4 : "")
            + (sprite5 != null && sprite5.Length > 0 ? ";sprite5:" + sprite5 : "")
            + (model0 != null && model0.Length > 0 ? ";model0:" + model0 : "")
            + (model1 != null && model1.Length > 0 ? ";model1:" + model1 : "")
            + (model2 != null && model2.Length > 0 ? ";model2:" + model2 : "")
            + (model3 != null && model3.Length > 0 ? ";model3:" + model3 : "")
            + ";currentDisplay:" + currentDisplay
            + ";hide:" + (hide ? "true" : "false")
            + ";dm_only:" + dm_only
            + serializedTags
            + ";";
    }

    /*
    [MenuItem("Fancy/Find Sprite")]
    public static void FindSprite()
    {
        var selected = Selection.activeGameObject;
        if (selected == null) return;
        var renderer = selected.GetComponent<SpriteRenderer>();
        if (renderer == null) return;
        Debug.Log(AssetDatabase.GetAssetPath(renderer.sprite));
    }

    [MenuItem("Fancy/Find Model")]
    public static void FindModel()
    {
        var selected = Selection.activeGameObject;
        if (selected == null) return;
        var renderer = selected.GetComponent<MeshRenderer>();
        if (renderer == null) return;
        Debug.Log(AssetDatabase.GetAssetPath(renderer.gameObject));
    }
    */
}
