using UnityEngine;

/// <summary>
/// Lee el genoma de la gallina y decide que capas visuales aplicar.
/// Usa los loci tipicos de aves (LocusC, LocusE, LocusBL, LocusS, etc.)
/// para determinar color de plumaje, cresta, tamano, etc.
///
/// Ejemplo de uso:
///   - LocusC: White/NonWhite (White es dominante, suprime color)
///   - LocusE: FullBlack/NoNeck/Perdiz (controla extension de melanina)
///   - LocusBL: NoBL/Grey (dilusion azul)
///   - LocusS: Silver/Gold (color base, sex-linked)
///   - CrestLocus: Small/Medium/Big/SuperLarge
///   - SizeLocus: Mini/Small/Medium/Big/SuperLarge
/// </summary>
public class ChickenAppearance : MonoBehaviour
{
    [Header("Referencias de sprites/capas")]
    [Tooltip("SpriteRenderer principal del cuerpo")]
    public SpriteRenderer bodyRenderer;

    [Tooltip("SpriteRenderer de la cresta")]
    public SpriteRenderer crestRenderer;

    [Tooltip("SpriteRenderer para patron de cuello")]
    public SpriteRenderer neckPatternRenderer;

    [Header("Configuracion de escala por tamano")]
    public float miniScale = 0.5f;
    public float smallScale = 0.75f;
    public float mediumScale = 1.0f;
    public float bigScale = 1.25f;
    public float superLargeScale = 1.6f;

    // Color resultante calculado del genoma
    [HideInInspector] public Color baseBodyColor;
    [HideInInspector] public Color neckColor;
    [HideInInspector] public float chickenScale;

    /// <summary>
    /// Aplica la apariencia visual basada en el genoma.
    /// Llamar despues de asignar el genoma a la gallina.
    /// </summary>
    public void ApplyGenome(Genome genome, GrowthStage stage)
    {
        if (genome == null) return;

        CalculateColors(genome);
        CalculateSize(genome);

        if (stage == GrowthStage.Chick)
        {
            ApplyChickAppearance();
        }
        else if (stage == GrowthStage.Adult)
        {
            ApplyAdultAppearance(genome);
        }
        // En stage Egg no se aplica apariencia de gallina
    }

    /// <summary>
    /// Calcula los colores del plumaje basandose en los loci de color.
    /// </summary>
    private void CalculateColors(Genome genome)
    {
        string locusC = genome.GetExpressedAllele("LocusC");
        string locusE = genome.GetExpressedAllele("LocusE");
        string locusBL = genome.GetExpressedAllele("LocusBL");
        string locusS = genome.GetExpressedAllele("LocusS");

        // LocusC: White es dominante y suprime todo color
        if (locusC == "White")
        {
            baseBodyColor = Color.white;
            neckColor = Color.white;
            return;
        }

        // Color base segun LocusS (sex-linked en aves reales)
        Color colorBase = (locusS == "Gold") ? new Color(0.8f, 0.55f, 0.2f) : new Color(0.85f, 0.85f, 0.85f);

        // LocusE: extension de melanina
        switch (locusE)
        {
            case "Full":
                baseBodyColor = Color.black;
                neckColor = Color.black;
                break;

            case "Birchen":
                // Cuello desnudo / sin plumas de color en cuello
                baseBodyColor = colorBase;
                neckColor = new Color(0.9f, 0.7f, 0.6f); // Piel rosada
                break;

            case "Perdiz":
            default:
                // Patron perdiz: cuerpo del color base, cuello mas oscuro
                baseBodyColor = colorBase;
                neckColor = colorBase * 0.6f;
                neckColor.a = 1f;
                break;
        }

        // LocusBL: dilusion azul/gris
        if (locusBL == "Grey")
        {
            baseBodyColor = Color.Lerp(baseBodyColor, Color.grey, 0.5f);
            neckColor = Color.Lerp(neckColor, Color.grey, 0.4f);
        }
    }

    /// <summary>
    /// Calcula la escala basada en el gen de tamano.
    /// </summary>
    private void CalculateSize(Genome genome)
    {
        string size = genome.GetExpressedAllele("SizeLocus");

        switch (size)
        {
            case "Mini":       chickenScale = miniScale; break;
            case "Small":      chickenScale = smallScale; break;
            case "Big":        chickenScale = bigScale; break;
            case "SuperLarge": chickenScale = superLargeScale; break;
            case "Medium":
            default:           chickenScale = mediumScale; break;
        }
    }

    /// <summary>
    /// Aplica la apariencia de pollito (version simplificada, mas amarillenta).
    /// </summary>
    private void ApplyChickAppearance()
    {
        // Los pollitos son versiones mas claras/amarillentas del color adulto
        Color chickColor = Color.Lerp(baseBodyColor, new Color(1f, 0.95f, 0.4f), 0.6f);
        float chickScale = chickenScale * 0.4f;

        if (bodyRenderer != null)
        {
            bodyRenderer.color = chickColor;
        }

        if (neckPatternRenderer != null)
        {
            neckPatternRenderer.enabled = false; // Pollitos no muestran patron de cuello
        }

        if (crestRenderer != null)
        {
            crestRenderer.enabled = false; // Pollitos no tienen cresta visible
        }

        transform.localScale = Vector3.one * chickScale;
    }

    /// <summary>
    /// Aplica la apariencia adulta con todos los detalles geneticos.
    /// </summary>
    private void ApplyAdultAppearance(Genome genome)
    {
        if (bodyRenderer != null)
        {
            bodyRenderer.color = baseBodyColor;
        }

        if (neckPatternRenderer != null)
        {
            neckPatternRenderer.enabled = true;
            neckPatternRenderer.color = neckColor;
        }

        // Cresta
        if (crestRenderer != null)
        {
            crestRenderer.enabled = true;
            string crest = genome.GetExpressedAllele("CrestLocus");
            float crestScale = 1.0f;
            switch (crest)
            {
                case "Small":      crestScale = 0.6f; break;
                case "Medium":     crestScale = 1.0f; break;
                case "Big":        crestScale = 1.4f; break;
                case "SuperLarge": crestScale = 1.8f; break;
            }
            crestRenderer.transform.localScale = Vector3.one * crestScale;
        }

        transform.localScale = Vector3.one * chickenScale;
    }
}
