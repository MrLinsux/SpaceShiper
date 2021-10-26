using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public GameObject target;
    public float smooth = 40;
    public float speed = 8.5f;
    public bool old;
    public float time = 0;

    void FixedUpdate()
    {
        // что бы скорость была стабильной в любом случае
        // и учитывая что мы вызываем из FixedUpdate мы умножаем на fixedDeltaTimе
        //if (Vector2.Distance(this.transform.position, target.transform.position) < 0.1f)
        //    time = 0;
        //else
        //    time += Time.fixedDeltaTime;

        if (!old)
            this.transform.position = (Vector3)Vector2.Lerp(this.transform.position, target.transform.position, Time.fixedDeltaTime/(smooth * Time.fixedDeltaTime)) - Vector3.forward * 9;
        else
            transform.Translate(((target.transform.position - this.transform.position) - Vector3.forward * 11) * speed * Time.fixedDeltaTime);
    }
}
