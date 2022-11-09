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

    public TextMesh consoleText;

    public bool dm = false;

    string path = "Assets/Resources/Yeet.txt";
    string serverPath = "Yeet.txt";

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
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

        if (Input.GetButtonDown("Flashlight"))
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
                else if (hit.transform.gameObject.name.Equals("DM Text"))
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
                else if (hit.transform.gameObject.name.Equals("Hide"))
                {
                    Debug.Log("Toggle visibility");
                    if (selected != null)
                    {
                        selected.GetComponent<FancyObject>().hide = !selected.GetComponent<FancyObject>().hide;
                        selected.GetComponent<FancyObject>().UpdateDisplay();
                    }
                }
            }
        }
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Jump") != 0) {
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

    IEnumerator pullFromFile()
    {
        yield return new WaitForEndOfFrame();
        GameObject[] fancyObjects = GameObject.FindGameObjectsWithTag("FancyObject");
        Debug.Log("Reading from " + path);
        System.IO.StreamReader reader = new System.IO.StreamReader(path);

        string m = reader.ReadToEnd();
        reader.Close();

        deserialize(m);
    }

    IEnumerator pullFromServer()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("Reading from server");

        //deserialize(ssh.readFromFile(serverPath));
        ssh.pullFile = serverPath;
        while (ssh.pullContents == null && ssh.connection)
        {
            yield return new WaitForSeconds(0.5f);
            Debug.Log("Waiting for response");
        }
        if (ssh.pullContents != null)
        {
            Debug.Log("Found response");
            deserialize(ssh.pullContents);
        }
    }

    void deserialize(string m)
    {
        GameObject[] fancyObjects = GameObject.FindGameObjectsWithTag("FancyObject");
        int p = 0;
        while (p < m.Length && m.IndexOf("}", p) != -1 && m.IndexOf("{", p) != -1)
        {
            p = m.IndexOf("{", p) + 1;
            string id = m.Substring(m.IndexOf("id:", p) + 3, m.IndexOf(";", p) - (m.IndexOf("id:", p) + 3));
            foreach (GameObject o in fancyObjects)
            {
                if (o.GetComponent<FancyObject>().id.Equals(id))
                {
                    o.GetComponent<FancyObject>().Deserialize(m.Substring(p, m.IndexOf("}", p) - p));
                    break;
                }
            }
        }
    }

    IEnumerator pushToFile()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("Writing to " + path);
       
        System.IO.File.WriteAllText(path, serialize());
    }

    IEnumerator pushToServer()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("Writing to server");

        //ssh.writeToFile(serverPath, serialize());
        ssh.pushFile = serverPath;
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
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
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

    IEnumerator ClickAnim(TextMesh text)
    {
        Color ogColor = text.color;
        text.color = new Color(1 - ogColor.r, 1 - ogColor.g, 1 - ogColor.b);
        yield return new WaitForSeconds(0.2f);
        text.color = ogColor;
    }
}
