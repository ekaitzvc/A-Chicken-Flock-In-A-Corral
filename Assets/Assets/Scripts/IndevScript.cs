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
            Vector3 randomPosition = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
            GameObject pollito = Instantiate(chickenPrefab, transform.position + randomPosition, Quaternion.identity);
            pollito.GetComponent<ChickenController>().InitializeRandom();
            pollito.GetComponent<ChickenGrowth>().Initialize(GrowthStage.Egg);
        }
    }
}