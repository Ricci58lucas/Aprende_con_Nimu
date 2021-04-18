using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public static MoveCamera instance = null;
    public float movingSpeed = 5f;

    void Start() => instance = this;

    void Update() => transform.position += Vector3.right * Time.deltaTime * movingSpeed;
}
