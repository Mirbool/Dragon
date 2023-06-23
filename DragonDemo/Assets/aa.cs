using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aa : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        player.transform.Translate(Vector3.forward * y * Time.deltaTime * 10);
        player.transform.Rotate(Vector3.up * x * Time.deltaTime * 100);
    }
}
