using UnityEngine;

public class SafePointsManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject[] safePoints;
    void Start()
    {
        CloseSafePoints();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ActivateSafePoints()
    {
        foreach(GameObject sp in safePoints)
        {
            sp.SetActive(true);
        }
    }
    public void CloseSafePoints()
    {
        foreach(GameObject sp in safePoints)
        {
            sp.SetActive(false);
        }
    }
}
