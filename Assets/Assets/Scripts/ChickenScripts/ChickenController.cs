using UnityEngine;

/// <summary>
/// Script principal de cada gallina. Conecta todos los subsistemas:
/// - Genome (genetica)
/// - Appearance (aspecto visual)
/// - Growth (etapas de vida)
/// - AI (comportamiento)
/// - Needs (necesidades)
///
/// Uso:
///   1. Anadir este componente a un prefab de gallina junto con los demas componentes.
///   2. Asignar el GeneCatalog en el Inspector.
///   3. Llamar a InitializeRandom() para gallinas de primera generacion,
///      o Initialize(padre, madre) para gallinas nacidas de padres.
/// </summary>
public class ChickenController : MonoBehaviour
{
    [Header("Configuracion")]
    [Tooltip("Catalogo con todas las definiciones de genes del juego")]
    public GeneCatalog catalog;

    [Header("Genoma (se genera al inicializar)")]
    public Genome genome;

    // Referencias a los otros componentes (se buscan automaticamente)
    private ChickenAppearance appearance;
    private ChickenGrowth growth;
    private ChickenAI ai;
    private ChickenNeeds needs;

    void Awake()
    {
        // Buscar componentes en este GameObject
        appearance = GetComponent<ChickenAppearance>();
        growth = GetComponent<ChickenGrowth>();
        ai = GetComponent<ChickenAI>();
        needs = GetComponent<ChickenNeeds>();
    }

    /// <summary>
    /// Inicializa una gallina con un genoma completamente aleatorio.
    /// Usar para la primera generacion de gallinas.
    /// </summary>
    public void InitializeRandom(GrowthStage startStage = GrowthStage.Adult)
    {
        if (catalog == null)
        {
            Debug.LogError("ChickenController: No se ha asignado un GeneCatalog!");
            return;
        }

        genome = GeneHelper.CreateRandomGenome(catalog);
        ApplyGenomeToAllSystems(startStage);
    }

    /// <summary>
    /// Inicializa una gallina por herencia de dos padres.
    /// Usar cuando nace una gallina de un huevo.
    /// </summary>
    public void Initialize(Genome father, Genome mother)
    {
        if (catalog == null)
        {
            Debug.LogError("ChickenController: No se ha asignado un GeneCatalog!");
            return;
        }

        genome = GeneHelper.CreateFromParents(catalog, father, mother);
        ApplyGenomeToAllSystems(GrowthStage.Egg);
    }

    /// <summary>
    /// Inicializa una gallina con un genoma ya creado.
    /// Usar cuando se quiere control total sobre el genoma.
    /// </summary>
    public void Initialize(Genome prebuiltGenome, GrowthStage startStage = GrowthStage.Egg)
    {
        genome = prebuiltGenome;
        ApplyGenomeToAllSystems(startStage);
    }

    /// <summary>
    /// Aplica el genoma a todos los subsistemas.
    /// </summary>
    private void ApplyGenomeToAllSystems(GrowthStage startStage)
    {
        // Crecimiento
        if (growth != null)
        {
            growth.Initialize(startStage);
            growth.OnStageChanged += OnGrowthStageChanged;
        }

        // Apariencia
        if (appearance != null)
        {
            appearance.ApplyGenome(genome, startStage);
        }

        // Necesidades
        if (needs != null)
        {
            needs.ApplyGenome(genome);
        }

        // AI / Comportamiento
        if (ai != null)
        {
            ai.Initialize(genome);
        }
    }

    /// <summary>
    /// Callback cuando la gallina cambia de etapa de crecimiento.
    /// Actualiza la apariencia para reflejar la nueva etapa.
    /// </summary>
    private void OnGrowthStageChanged(GrowthStage newStage)
    {
        if (appearance != null)
        {
            appearance.ApplyGenome(genome, newStage);
        }

        // Al llegar a adulto, la AI empieza a funcionar completamente
        if (newStage == GrowthStage.Adult && ai != null)
        {
            ai.Initialize(genome);
        }
    }

    /// <summary>
    /// Devuelve un resumen legible del genoma de esta gallina (para debug/UI).
    /// </summary>
    public string GetGenomeSummary()
    {
        if (genome == null) return "Sin genoma";

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine($"Sexo: {genome.sex}");

        foreach (Gene gene in genome.genes)
        {
            string expressed = gene.GetExpressedAllele();
            if (gene.IsHemizygous())
            {
                sb.AppendLine($"  {gene.definition.geneName}: {gene.allele1} (hemizigoto) -> Expresa: {expressed}");
            }
            else
            {
                sb.AppendLine($"  {gene.definition.geneName}: {gene.allele1}/{gene.allele2} -> Expresa: {expressed}");
            }
        }

        return sb.ToString();
    }

    void OnDestroy()
    {
        if (growth != null)
        {
            growth.OnStageChanged -= OnGrowthStageChanged;
        }
    }
}
