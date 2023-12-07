//Shady
using UnityEngine;

[ExecuteInEditMode]
public class Reveal : MonoBehaviour
{
    [SerializeField] Material Mat;
    [SerializeField] Light SpotLight;
	
	void Update ()
    {
        if (SpotLight.intensity != 0 && SpotLight.gameObject.activeInHierarchy)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            Mat.SetVector("MyLightPosition", SpotLight.transform.position);
            Mat.SetVector("MyLightDirection", -SpotLight.transform.forward);
            Mat.SetFloat("MyLightAngle", SpotLight.spotAngle);
        }
        else
        {
            Mat.SetVector("MyLightPosition", Vector4.zero);
            Mat.SetVector("MyLightDirection", Vector4.zero);
            Mat.SetFloat("MyLightAngle", 0);
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }//Update() end
}//class end