using UnityEngine;

public class Camera : MonoBehaviour
{
    public GameObject target;
    public float speed;
    public float offset = 7.0f;
    Rigidbody2D rb2D;

    // Start is called before the first frame update
    void Start()
    {
        rb2D = target.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.transform.position = Vector3.Lerp(
            this.transform.position, 
            (Vector2)target.transform.position + rb2D.velocity.normalized * offset, 
            speed * Time.deltaTime
            ) - new Vector3(0, 0, 11);
    }
}
