using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Di : MonoBehaviour {

    public int n;
    public GameObject top_face;
    public TextMesh[] text;


	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    [SerializeField]
    private float _timeScale = 4;
    bool first = true;
    Rigidbody rb;
    public float timeScale
    {
        get { return _timeScale; }
        set
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }
            if (!first)
            {
                rb.mass *= timeScale;
                rb.velocity /= timeScale;
                rb.angularVelocity /= timeScale;
            }
            first = false;

            _timeScale = Mathf.Abs(value);

            rb.mass /= timeScale;
            rb.velocity *= timeScale;
            rb.angularVelocity *= timeScale;
        }
    }

    void Awake()
    {
        timeScale = _timeScale;
    }

    void FixedUpdate()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        if (rb != null)
        {
            float dt = Time.fixedDeltaTime * timeScale;
            rb.velocity += Physics.gravity * dt * 4;
        }
    }

}
