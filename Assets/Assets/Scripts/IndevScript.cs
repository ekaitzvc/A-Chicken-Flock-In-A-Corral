using UnityEngine;

public class IndevScript : MonoBehaviour
{
    public GameObject chickenPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameObject pollito = Instantiate(chickenPrefab, transform.position, Quaternion.identity);
            pollito.GetComponent<ChickenController>().InitializeRandom();
        }
    }
}