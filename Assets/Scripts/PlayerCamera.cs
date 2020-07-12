using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    public float mouseSensitivity = 100f;
    private Transform player;
    private float xRotation = 0f, tilt, mouseX, mouseY;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
        tilt = 0;
    }

    // Update is called once per frame
    void Update()   
    {
        Look();
    }
    void Look()
    {
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation,0,0);
        player.transform.Rotate(Vector3.up * mouseX);
    }

    public void Tilt(float tilt)
    {
        this.tilt = tilt;
    }

    public void ClimbTilt(){
        this.tilt = 15;
    }
}