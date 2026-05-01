using UnityEngine;
using System.Collections;

public class Neonflicker : MonoBehaviour
{
    public Color neonColor = new Color(1f, 0.5f, 0f);
    public float minInten = 0f;
    public float maxInten = 0f;
    private Material mat;
    private Light flicekrLight;

    private void Start()
    {
        mat = GetComponent<Renderer>().material;
        flicekrLight = GetComponentInChildren<Light>();
        
    }

    IEnumerator Flicker ()
    {
        float intensity = Random.Range(2f, maxInten);
        mat.SetColor("_EmissionColor", neonColor * intensity);
        if (flicekrLight) flicekrLight.intensity = intensity;

        yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));

        mat.SetColor("_EmissionColor", neonColor * minInten);
        if (flicekrLight) flicekrLight.intensity = minInten;

        yield return new WaitForSeconds(Random.Range(0.02f, 0.08f));

        intensity = Random.Range(2f, maxInten);
        mat.SetColor("_EmissionColor", neonColor * intensity);
        if (flicekrLight) flicekrLight.intensity = intensity;

        yield return new WaitForSeconds(Random.Range(0.5f, 3f));
    }
}
