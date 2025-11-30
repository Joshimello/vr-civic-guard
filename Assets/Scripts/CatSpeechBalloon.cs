using UnityEngine;

public class CatSpeechBalloon : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform CatTransform;
    public Transform PlayerTransform;
    void Start()
    {
        transform.position = new Vector3(CatTransform.position.x, CatTransform.position.y + 1.0f, CatTransform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        SetSpeechBalloonPosition();
    }
    void SetSpeechBalloonPosition()
    {
        transform.position = new Vector3(CatTransform.position.x, CatTransform.position.y + 1.0f, CatTransform.position.z);
        transform.LookAt(PlayerTransform);
        transform.Rotate(0, 180, 0);
    }
    
}
