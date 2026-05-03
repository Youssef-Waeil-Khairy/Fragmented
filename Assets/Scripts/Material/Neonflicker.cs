using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;

public class Neonflicker : MonoBehaviour
{
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    public Color neonColor = new Color(1f, 0.5f, 0f);
    public float minInten = 0f;
    public float maxInten = 0f;
    [SerializeField] private List<MeshRenderer> meshRenderers;
    [SerializeField] private List<Material> mats;
    [SerializeField] private List<Light> flicekrLights;

    public bool IsFlickering = true;
    public bool CanToggleFlicker = true;
    public float FlickerSpeed = 0.5f;
    public float FlickerTime = 0f;
    public bool MaxRangeToggle = true;
    public float Intensity = 0f;
    [MinMaxSlider(0.2f, 1f)]public Vector2 TimeRange = new Vector2(1f, 1f);

    private void Start()
    {
        //mat = GetComponent<Renderer>().material;
        //flicekrLight = GetComponentInChildren<Light>();


    }

    private void Update()
    {
        if (IsFlickering)
        {
            FlickerTime += Time.deltaTime;

            if (FlickerTime >= FlickerSpeed)
            {
                FlickerTime = 0f;

                ToggleIntensity();
                foreach (Material mat in mats)
                {
                    mat.SetColor(EmissionColor, neonColor * Intensity);
                }
                foreach (Light flicekrLight in flicekrLights)
                {
                    if (flicekrLight) flicekrLight.intensity = Intensity;
                }
            }
        }
    }

    [Button]
    public void SetUp()
    {
        if (meshRenderers == null || meshRenderers.Count == 0)
        {
            return;
        }

        foreach (MeshRenderer mr in meshRenderers)
        {
            mats.Add(mr.material);
        }
    }

    [Button]
    public void ToggleIntensity()
    {
        if (MaxRangeToggle)
        {
            MaxRangeToggle = false;
            Intensity = maxInten;
            FlickerSpeed = Random.Range(TimeRange.x, TimeRange.y);
        }
        else
        {
            MaxRangeToggle = true;
            Intensity = minInten;
            FlickerSpeed = Random.Range(TimeRange.x, TimeRange.y);
        }
    }

    public void ToggleFlicker(bool isFlickering)
    {
        if (!CanToggleFlicker)
        {
            return;
        }

        IsFlickering = isFlickering;

        if (!IsFlickering)
        {
            Intensity = maxInten;

            foreach (Material mat in mats)
            {
                mat.SetColor(EmissionColor, neonColor * Intensity);
            }
            foreach (Light flicekrLight in flicekrLights)
            {
                if (flicekrLight) flicekrLight.intensity = Intensity;
            }
        }
    }
    public void ToggleCanFlickerToggle(bool canToggle)
    {
        CanToggleFlicker = canToggle;
    }

    /*
    IEnumerator Flicker ()
    {
        float intensity = Random.Range(2f, maxInten);
        mat.SetColor(EmissionColor, neonColor * intensity);
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
    */
}