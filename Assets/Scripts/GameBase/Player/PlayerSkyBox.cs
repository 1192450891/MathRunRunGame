using UnityEngine;

public class PlayerSkyBox
{
    private GameObject skyGameObject;
    private GameObject cloudGameObject;
        
    private const string skyGameObjectName="SM_SimpleSky_Dome_01";
    private const string cloudGameObjectName="SM_Generic_CloudRing_01";

    private const float cloudRotateSpeed = -0.7f;
        
    public PlayerSkyBox(Transform transform)
    {
        skyGameObject = TransformUtil.Find(transform,skyGameObjectName).gameObject;
        cloudGameObject = TransformUtil.Find(transform, cloudGameObjectName).gameObject;
    }


    public void FixUpdate()
    {
        cloudGameObject.transform.Rotate(0, cloudRotateSpeed * Time.deltaTime, 0);
    }
}