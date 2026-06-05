using UnityEngine;

public class VeinInfectionColour : MonoBehaviour
{
    public BoidSpawner spawner;

    [Header("Colours")]
    public Color healthyColor = Color.red;
    public Color infectedColor = Color.black;

    [Header("Infection")]
    public int infectionTarget = 100;

    [Header("Shader Graph")]
    public string colorProperty = "_BaseColor";

    private MaterialPropertyBlock propertyBlock;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();

        if (spawner == null)
        {
            spawner = FindFirstObjectByType<BoidSpawner>();
        }

        propertyBlock = new MaterialPropertyBlock();
    }

    void Update()
    {
        if (spawner == null) return;

        float infectionPercent =
            Mathf.Clamp01(
                (float)spawner.infectedCells / infectionTarget
            );

        Color currentColor =
            Color.Lerp(
                healthyColor,
                infectedColor,
                infectionPercent
            );

        //rend.GetPropertyBlock(propertyBlock);

        propertyBlock.SetColor(
            colorProperty,
            currentColor
        );

        rend.SetPropertyBlock(propertyBlock);
    }
}