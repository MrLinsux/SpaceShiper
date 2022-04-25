using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Laser : MonoBehaviour
{
    public GameObject beam;
    public Tilemap map;
    public float activeTime = 1f;
    public float passiveTime = 1f;
    private List<GameObject> beams = new List<GameObject>();

    private void Start()
    {
        map = GameObject.Find("Map").GetComponent<Tilemap>();
        Vector3Int direction = Vector3Int.zero;
        if (this.transform.rotation.eulerAngles == Vector3.zero)
            direction = Vector3Int.right;
        else if (this.transform.rotation.eulerAngles == Vector3.forward * 90)
            direction = Vector3Int.up;
        else if (this.transform.rotation.eulerAngles == Vector3.forward * 180) 
            direction = Vector3Int.left;
        else if (this.transform.rotation.eulerAngles == Vector3.forward * -90)
            direction = Vector3Int.down;

        int i = 1;
        do
        {
            beams.Add(Instantiate(beam, this.transform.position + direction * i + Vector3Int.back*51, Quaternion.identity, this.transform));
            beams[i - 1].SetActive(false);
        }
        while (map.GetTile(map.WorldToCell(this.transform.position + direction * ++i)) != null);
        StartCoroutine(LaserCycle());
    }

    private IEnumerator LaserCycle()
    {
        while (true)
        {
            for(int i = 0; i < beams.Count; i++)
                beams[i].SetActive(true);
            yield return new WaitForSeconds(activeTime);

            for (int i = 0; i < beams.Count; i++)
                beams[i].SetActive(false);
            yield return new WaitForSeconds(passiveTime);
        }
    }
}
