using UnityEngine;

/// <summary>
/// Gestiona las necesidades de la gallina.
/// Los valores de las necesidades se ven modificados por el genoma.
/// </summary>
public class ChickenNeeds : MonoBehaviour
{
    [Header("Necesidades (0-100)")]
    public float hunger = 0f;
    public float energy = 100f;
    public float horniness = 0f;
    public float miedo = 0f;

    [Header("Tasas base (por segundo)")]
    [Tooltip("Velocidad a la que sube el hambre")]
    public float baseHungerRate = 0.5f;

    [Tooltip("Velocidad a la que baja la energia")]
    public float baseEnergyDrainRate = 0.2f;

    [Tooltip("Velocidad a la que sube el deseo de aparearse")]
    public float baseHorninessRate = 0.1f;

    [Header("Modificadores geneticos (se aplican automaticamente)")]
    [HideInInspector] public float hungerModifier = 1f;
    [HideInInspector] public float energyModifier = 1f;
    [HideInInspector] public float fearSensitivity = 1f;

    /// <summary>
    /// Configura los modificadores de necesidades basados en el genoma.
    /// </summary>
    public void ApplyGenome(Genome genome)
    {
        if (genome == null) return;

        // El tamano afecta al hambre: gallinas mas grandes tienen mas hambre
        float size = genome.GetTraitValue("SizeLocus");
        hungerModifier = 0.7f + (size * 0.6f); // Rango: 0.7 (Mini) a 1.3 (SuperLarge)

        // Awareness afecta la sensibilidad al miedo
        fearSensitivity = genome.GetTraitValue("Awareness");

        // Speed afecta el consumo de energia (mas rapidas gastan mas)
        float speed = genome.GetTraitValue("Speed");
        energyModifier = 0.8f + (speed * 0.4f); // Rango: 0.8 a 1.2
    }

    void Update()
    {
        hunger += Time.deltaTime * baseHungerRate * hungerModifier;
        hunger = Mathf.Clamp(hunger, 0f, 100f);

        energy -= Time.deltaTime * baseEnergyDrainRate * energyModifier;
        energy = Mathf.Clamp(energy, 0f, 100f);

        horniness += Time.deltaTime * baseHorninessRate;
        horniness = Mathf.Clamp(horniness, 0f, 100f);

        // El miedo decae con el tiempo si no hay amenazas
        miedo -= Time.deltaTime * 0.3f;
        miedo = Mathf.Clamp(miedo, 0f, 100f);
    }

    /// <summary>
    /// Incrementa el miedo. La cantidad se ve afectada por la sensibilidad genetica.
    /// </summary>
    public void AddFear(float amount)
    {
        miedo += amount * fearSensitivity;
        miedo = Mathf.Clamp(miedo, 0f, 100f);
    }

    /// <summary>
    /// Reduce el hambre al comer.
    /// </summary>
    public void Eat(float amount)
    {
        hunger -= amount;
        hunger = Mathf.Clamp(hunger, 0f, 100f);
    }

    /// <summary>
    /// Recupera energia al descansar/dormir.
    /// </summary>
    public void Rest(float amount)
    {
        energy += amount;
        energy = Mathf.Clamp(energy, 0f, 100f);
    }
}
