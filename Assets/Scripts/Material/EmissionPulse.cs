using UnityEngine;

public class EmissionPulse : MonoBehaviour
{
    public Color emissionColor = Color.red;
    public float minIntensity = 0.5f;
    public float maxIntensity = 3f;
    public float pulseSpeed = 1f;

    private Material mat;
    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        float intensity = Mathf.Lerp(minIntensity, maxIntensity,(Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);

        mat.SetColor("_Emissionccolor", emissionColor * intensity);
    }
}
