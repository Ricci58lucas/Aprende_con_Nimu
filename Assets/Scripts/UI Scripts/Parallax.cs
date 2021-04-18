using UnityEngine;

public class Parallax : MonoBehaviour
{
    public GameObject cam;
    public float parallaxEffect;

    public Sprite nimuHouse = null;

    private float length;
    private float startPos;
    private bool bringIntroBG = false;

    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        float temp = cam.transform.position.x * (1 - parallaxEffect);
        float dist = cam.transform.position.x * parallaxEffect;
        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

        if (bringIntroBG)
        {
            if (temp > startPos + length)
                MoveCamera.instance.movingSpeed = 0f;

            return;
        };

        if (temp > startPos + length) 
            startPos += length;
        else if (temp < startPos - length) 
            startPos -= length;
    }

    public void BringIntroBG()
    {
        bringIntroBG = true;

        if (nimuHouse == null) return;

        GetComponentsInChildren<SpriteRenderer>()[2].sprite = nimuHouse;
    }
}
