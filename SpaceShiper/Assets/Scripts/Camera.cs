using UnityEngine;

public class Camera : MonoBehaviour
{
    public GameObject target;
    public float speed;

    void FixedUpdate()
    {
        // что бы скорость была стабильной в любом случае
        // и учитывая что мы вызываем из FixedUpdate мы умножаем на fixedDeltaTimе
        transform.Translate(
            ((target.transform.position - this.transform.position) - Vector3.forward * 11) * speed * Time.fixedDeltaTime);
    }
}
