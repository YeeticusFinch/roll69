using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FancyCam : MonoBehaviour {

    float rotX = 0;
    float rotY = 0;
    public float mouseSensitivity = 3f;
    bool trackSelected = false;

    GameObject selected;
    public LayerMask mask;

    public TextMesh trackSelectedButton;
    public TextMesh selectedText;
    public TextMesh dmText;
    public TextMesh hideText;
    public Light flashlight;
    public SshConnection ssh;
    public TextMesh mapSelect;

    private Map map;

    public TextMesh consoleText;

    public bool dm = false;

    string localPath = "Assets/Resources/";
    //string serverPath = "Yeet.txt";
    string fileExt = ".r69";

    public List<GameObject> deleteOnClear = new List<GameObject>();

    // Use this for initialization
    void Start () {
        map = this.gameObject.AddComponent<Map>();
	}

    string prevKey = "";
    public bool typeBox = false;

	// Update is called once per frame
	void Update () {
        if (typeBox && mapSelecting && Input.anyKey)
        {
            //if (prevKey != null && prevKey.Length > 0 && !Input.GetKey())
            //    prevKey = "";
            foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(kcode))
                {
                    //Debug.Log("KeyCode down: " + kcode);
                    if ("ABCDEFGHIJKLMNOPQRSTUVWXYZ".Contains(kcode.ToString()))
                        {
                        //if (!kcode.ToString().Equals(prevKey))
                        //{
                            //prevKey = kcode.ToString();
                            if (Input.GetKey(KeyCode.LeftShift))
                                mapSelect.text += kcode.ToString();
                            else
                                mapSelect.text += kcode.ToString().ToLower();
                            return;
                        //}
                    } else if (kcode.ToString().Equals("Backspace"))
                    {
                        mapSelect.text = mapSelect.text.Remove(mapSelect.text.Length - 1);
                    }
                    break;
                }
            }
            //mapSelect.text += Event.current.keyCode.ToString();
        }
        if (Input.GetButton("Fire2"))
        {
            rotX -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
            rotY += Input.GetAxisRaw("Mouse X") * mouseSensitivity;
            //Quaternion xQuaternion = Quaternion.AngleAxis(rotX, Vector3.up);
            //Quaternion yQuaternion = Quaternion.AngleAxis(rotY, -Vector3.right);
            //transform.localRotation = originalRotation * xQuaternion * yQuaternion;
            //transform.eulerAngles = new Vector3(rotX, rotY, 0);
            transform.eulerAngles = new Vector3(rotX, rotY, 0);
            //GameMaster.instance.board.canMove = false;
        }

        if (!dm || selected == null)
        {
            hideText.text = "";
        } else
        {
            hideText.text = "Hide = " + (selected.GetComponent<FancyObject>().hide ? "true" : "false");
        }

        if (Input.GetButtonDown("Flashlight") && !typeBox)
        {
            if (flashlight.intensity == 0)
                flashlight.intensity = 1;
            else
                flashlight.intensity = 0;
        }
        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000, mask))
            {
                //Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.GetComponent<TextMesh>() != null)
                {
                    StartCoroutine(ClickAnim(hit.transform.GetComponent<TextMesh>()));
                }
                if (hit.transform.GetComponent<FancyObject>() != null && (dm || !hit.transform.GetComponent<FancyObject>().dm_only))
                {
                    Debug.Log("Selection change");
                    selected = hit.transform.gameObject;
                    selectedText.text = "Selected = " + hit.transform.gameObject.GetComponent<FancyObject>().title;
                }
                else if (hit.transform.gameObject.name.Equals("Change Sprite"))
                {
                    Debug.Log("Changing sprite of selected object");
                    if (selected != null)
                    {
                        selected.GetComponent<FancyObject>().NextSprite();
                    }
                }
                else if (hit.transform.gameObject.name.Equals("First Person"))
                {
                    Debug.Log("Camera to Selected");
                    if (selected != null)
                    {
                        this.transform.SetPositionAndRotation(selected.transform.position, Quaternion.Euler(selected.GetComponent<FancyObject>().modelEulers));
                    }
                }
                else if (hit.transform.gameObject.name.Equals("Selected to Camera"))
                {
                    Debug.Log("Selected to Camera");
                    if (selected != null)
                    {
                        selected.transform.SetPositionAndRotation(this.transform.position, Quaternion.Euler(selected.GetComponent<FancyObject>().modelEulers));
                    }
                }
                else if (hit.transform.gameObject.name.Equals("Track Selected"))
                {
                    Debug.Log("Toggle track selected object");
                    if (selected != null)
                    {
                        trackSelected = !trackSelected;
                        trackSelectedButton.text = "Track Selected = " + (trackSelected ? "true" : "false");
                    }
                }
                else if (hit.transform.gameObject.name.Equals("DM Text") && !typeBox)
                {
                    Debug.Log("DM login attempt");
                    if (ssh.tryLogin())
                    {
                        dm = !dm;
                        dmText.text = "DM = " + (dm ? "true" : "false");
                        GameObject[] fancyObjects = GameObject.FindGameObjectsWithTag("FancyObject");
                        foreach (GameObject o in fancyObjects)
                        {
                            o.GetComponent<FancyObject>().UpdateDisplay();
                        }
                    }
                }
                else if (hit.transform.gameObject.name.Equals("Pull"))
                {
                    StartCoroutine(pullFromServer());
                }
                else if (hit.transform.gameObject.name.Equals("Push"))
                {
                    StartCoroutine(pushToServer());
                }
                else if (hit.transform.gameObject.name.Equals("Hide") && dm)
                {
                    Debug.Log("Toggle visibility");
                    if (selected != null)
                    {
                        selected.GetComponent<FancyObject>().hide = !selected.GetComponent<FancyObject>().hide;
                        selected.GetComponent<FancyObject>().UpdateDisplay();
                    }
                }
                else if (hit.transform.gameObject.name.Equals("Map Select") && dm)
                {
                    if (!mapSelecting)
                    {
                        mapSelect.text = "Map Name: ";
                        mapSelecting = true;
                        typeBox = true;
                        Debug.Log("Map Selector");
                        for (int i = 0; i < map.maps.Count; i++)
                        {
                            Debug.Log("Creating " + "MapName=" + map.maps[i]);
                            GameObject newObject = GameObject.Instantiate(this.mapSelect.gameObject, this.transform);
                            newObject.transform.position += transform.right * (i + 1.35f) * 0.6f;
                            newObject.name = "MapName";
                            newObject.GetComponent<TextMesh>().text = map.maps[i];
                            newObject.GetComponent<TextMesh>().color = Color.cyan;
                            newObject.tag = "DeleteOnClear";
                            deleteOnClear.Add(newObject);
                        }
                    } else
                    {
                        typeBox = false;
                        mapSelecting = false;
                        for (int i = deleteOnClear.Count-1; i >=1; i++)
                        {
                            GameObject.Destroy(deleteOnClear[i]);
                            //deleteOnClear.Remove(i);
                        }
                        foreach (GameObject o in GameObject.FindGameObjectsWithTag("DeleteOnClear"))
                        {
                            GameObject.Destroy(o);
                        }
                        map.file = mapSelect.text.Substring(mapSelect.text.IndexOf(": ") + 2);
                        Debug.Log("Setting map to " + map.file);
                        map.AddMap(map.file);
                    }
                } else if (hit.transform.gameObject.name.Equals("MapName"))
                {
                    mapSelect.text = "Map Name: " + hit.transform.gameObject.GetComponent<TextMesh>().text;
                    typeBox = false;
                    mapSelecting = false;
                    foreach (GameObject o in deleteOnClear)
                        GameObject.Destroy(o);
                    foreach (GameObject o in GameObject.FindGameObjectsWithTag("DeleteOnClear"))
                    {
                        GameObject.Destroy(o);
                    }
                    /*for (int i = deleteOnClear.Count - 1; i >= 1; i++)
                    {
                        GameObject.Destroy(deleteOnClear[i]);
                        //deleteOnClear.Remove(i);
                    }*/
                    map.file = mapSelect.text.Substring(mapSelect.text.IndexOf(": ") + 2);
                    Debug.Log("Setting map to " + map.file);
                }
            }
        }
        if (!typeBox && (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Jump") != 0)) {
            if (Input.GetButton("Move Selected"))
            {
                // move the selected piece
                if (selected != null)
                {
                    selected.transform.position += (Input.GetAxisRaw("Vertical") * horizontalForward() + Input.GetAxisRaw("Horizontal") * horizontalRight() + Input.GetAxisRaw("Jump") * Vector3.up) * (0.6f + 0.5f*Input.GetAxisRaw("Sprint"));
                    if (trackSelected)
                        transform.position += (Input.GetAxisRaw("Vertical") * horizontalForward() + Input.GetAxisRaw("Horizontal") * horizontalRight() + Input.GetAxisRaw("Jump") * Vector3.up) * (0.6f + 0.5f * Input.GetAxisRaw("Sprint"));
                }
            } else
            {
                //move the camera
                transform.position += (Input.GetAxisRaw("Vertical") * transform.forward + Input.GetAxisRaw("Horizontal") * transform.right + Input.GetAxisRaw("Jump") * transform.up) * (0.6f + 0.5f*Input.GetAxisRaw("Sprint"));
            }
        }
    }

    public bool mapSelecting = false;

    IEnumerator pullFromFile()
    {
        yield return new WaitForEndOfFrame();
        GameObject[] fancyObjects = GameObject.FindGameObjectsWithTag("FancyObject");
        Debug.Log("Reading from " + localPath + map.file + fileExt);
        System.IO.StreamReader reader = new System.IO.StreamReader(localPath + map.file + fileExt);

        string m = reader.ReadToEnd();
        reader.Close();

        deserialize(m);
    }

    IEnumerator pullFromServer()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("Reading from server");

        ssh.pullFile = "r69.main";

        while (ssh.pullContents == null && ssh.connection)
        {
            yield return new WaitForSeconds(0.5f);
            Debug.Log("Waiting for response");
        }
        if (ssh.pullContents != null)
        {
            Debug.Log("Found response");
            map.Deserialize(ssh.pullContents);
            ssh.pullContents = null;
        }

        mapSelect.text = map.file;

        while (ssh.pullFile != null && ssh.pullFile.Length > 0)
        {
            yield return new WaitForSeconds(0.4f);
        }

        //deserialize(ssh.readFromFile(serverPath));
        ssh.pullFile = map.file + fileExt;
        while (ssh.pullContents == null && ssh.connection)
        {
            yield return new WaitForSeconds(0.5f);
            Debug.Log("Waiting for response");
        }
        if (ssh.pullContents != null)
        {
            Debug.Log("Found response");
            deserialize(ssh.pullContents);
            ssh.pullContents = null;
        }


    }

    void deserialize(string m)
    {
        GameObject[] fancyObjects = GameObject.FindGameObjectsWithTag("FancyObject");
        bool[] fancyList = new bool[fancyObjects.Length];
        int p = 0;
        while (p < m.Length && m.IndexOf("}", p) != -1 && m.IndexOf("{", p) != -1)
        {
            p = m.IndexOf("{", p) + 1;
            string id = m.Substring(m.IndexOf("id:", p) + 3, m.IndexOf(";", p) - (m.IndexOf("id:", p) + 3));
            bool foundIt = false;
            for (int i = 0; i < fancyObjects.Length; i++)
            {
                GameObject o = fancyObjects[i];
                if (o.GetComponent<FancyObject>().id.Equals(id))
                {
                    fancyList[i] = true;
                    foundIt = true;
                    o.GetComponent<FancyObject>().Deserialize(m.Substring(p, m.IndexOf("}", p) - p));
                    break;
                }
            }
            if (!foundIt)
            {
                GameObject newObject = Resources.Load("Display/FancyObjectTemplate") as GameObject;
                newObject.GetComponent<FancyObject>().Deserialize(m.Substring(p, m.IndexOf("}", p) - p));
                GameObject.Instantiate(newObject);
                foundIt = true;
            }
        }
        if (fancyList.Length > 0)
        {
            for (int i = fancyList.Length - 1; i >= 0; i++)
            {
                if (i > 0 && i < fancyList.Length && !fancyList[i])
                {
                    Debug.Log("Destroying " + fancyObjects[i].GetComponent<FancyObject>().title);
                    GameObject.Destroy(fancyObjects[i]);
                }
            }
        }
    }

    IEnumerator pushToFile()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("Writing to " + localPath + map.file + fileExt);
       
        System.IO.File.WriteAllText(localPath + map.file + fileExt, serialize());
    }

    IEnumerator pushToServer()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("Writing to server");


        ssh.pushFile = "r69.main";
        ssh.pushContents = map.Serialize();


        while (ssh.pushFile != null && ssh.pushFile.Length > 0)
            yield return new WaitForSeconds(0.3f);

        //ssh.writeToFile(serverPath, serialize());
        ssh.pushFile = map.file + fileExt;
        ssh.pushContents = serialize();
    }

    string serialize()
    {
        string fancyText = "";
        GameObject[] fancyObjects = GameObject.FindGameObjectsWithTag("FancyObject");
        foreach (GameObject o in fancyObjects)
        {
            fancyText += "{" + o.GetComponent<FancyObject>().Serialize() + "}";
        }
        if (System.IO.File.Exists(localPath + map.file + fileExt))
        {
            System.IO.File.Delete(localPath + map.file + fileExt);
        }
        return fancyText;
    }

    Vector3 horizontalForward()
    {
        return new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
    }

    Vector3 horizontalRight()
    {
        return Vector3.Cross(horizontalForward(), -Vector3.up);
        //return new Vector3(transform.right.x, 0, transform.right.y).normalized;
    }

    /* // dropbox fallback
    IEnumerator upload()
    {
        Dictionary<string, string> postHeader = new Dictionary<string, string>();
        postHeader.Add("Authorization", "Bearer <code>");
        postHeader.Add("Dropbox-API-Arg", "{\"path\": \"/provaupload.json\"}");
        postHeader.Add("Content-Type", "");
        using (WWW www = new WWW("https://content.dropboxapi.com/2/files/download", null, postHeader))
        {
            yield return www;
            if (www.error != null)
            {
                Debug.Log(www.error + " " + www.text);
            }
            else
            {
                Debug.Log("Success! " + www.text);
            }
        }
    }*/

    bool clickAniming = false;

    IEnumerator ClickAnim(TextMesh text)
    {
        if (clickAniming == false)
        {
            clickAniming = true;
            Color ogColor = text.color;
            text.color = new Color(1 - ogColor.r, 1 - ogColor.g, 1 - ogColor.b);
            yield return new WaitForSeconds(0.2f);
            if (text && text != null)
            {
                text.color = ogColor;
                clickAniming = false;
            }
        }
    }
}
