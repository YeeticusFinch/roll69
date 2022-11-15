using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

    public string file = "Yeet";
    public Color background = Color.black;

    public List<string> maps = new List<string>();

	// Use this for initialization
	void Start () {
        background = GetComponent<Camera>().backgroundColor;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddMap(string newMap)
    {
        foreach (string m in maps)
        {
            if (m.Equals(newMap))
                return;
        }
        maps.Add(newMap);
    }

    public void Deserialize(string input)
    {
        int i = 0;
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
            i = input.IndexOf(';', i) + 1;
            
        }
    }

    public void DeserializeSetting(string input)
    {
        int i = 0;
        while (i < input.Length && input.IndexOf(':', i) != -1)
        {
            string sel = input.Substring(i, input.IndexOf(':', i) - i);
            if (sel[0] == ';') sel = sel.Substring(1);
            if (sel[sel.Length - 1] == ':') sel = sel.Substring(0, sel.Length - 1);
            string val = input.Substring(input.IndexOf(':', i) + 1, input.IndexOf(';', i) - (input.IndexOf(':', i) + 1));
            i = input.IndexOf(';', i) + 1;

            if (sel.Substring(0, 4).Equals("maps"))
            {
                maps.Add(val);
            }
            else if (sel.Equals("file"))
            {
                file = val;
            }
            else if (sel.Equals("bg-r"))
            {
                background.r = float.Parse(val);
            }
            else if (sel.Equals("bg-g"))
            {
                background.g = float.Parse(val);
            }
            else if (sel.Equals("bg-b") || sel.Equals("be.b"))
            {
                background.b = float.Parse(val);
            }
        }

        GetComponent<FancyCam>().BackgroundColorPicker.SetColor(background);
        GetComponent<Camera>().backgroundColor = background;
    }

    public string Serialize()
    {
        string mapStr = "";
        for (int i = 0; i < maps.Count; i++)
        {
            mapStr += "maps" + i + ":" + maps[i] + ";";
        }
        return "file:" + file + ";"
            + mapStr
            ;
    }

    public string SerializeSetting()
    {
        return
        "bg-r:" + background.r + ";"
        + "bg-g:" + background.g + ";"
        + "bg-b:" + background.b + ";"
        ;
    }
}
