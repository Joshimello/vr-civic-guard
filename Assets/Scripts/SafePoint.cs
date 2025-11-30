using UnityEngine;

public class SafePoint : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Canvas coverCanvas;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // when player enter the triiger
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            coverCanvas.enabled = true;
            Invoke("DisableCanvas", 5f);
        }
    }
    void DisableCanvas()
    {
        coverCanvas.enabled = false;
    }
}
