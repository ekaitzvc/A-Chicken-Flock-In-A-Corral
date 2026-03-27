using UnityEngine;

/// <summary>
/// ScriptableObject que define un tipo de gen.
/// Se crea como asset en Unity: clic derecho > Create > Genes > GeneDefinition.
/// Para anadir un gen nuevo al juego, solo hay que crear un asset nuevo y anadirlo al catalogo.
/// </summary>
[CreateAssetMenu(fileName = "NewGene", menuName = "Genes/GeneDefinition")]
public class GeneDefinition : ScriptableObject
{
    [Header("Identidad")]
    [Tooltip("Nombre unico del gen (ej: LocusC, Speed, Crowd)")]
    public string geneName;

    [Tooltip("Todos los alelos posibles para este gen")]
    public string[] possibleAlleles;

    [Tooltip("Orden de dominancia: el primero es el mas dominante. " +
             "Para codominancia, pon los alelos en el mismo nivel (se expresara el primero encontrado).")]
    public string[] dominanceOrder;

    [Header("Mutacion")]
    [Tooltip("Probabilidad de que un alelo mute al heredarse (0 = nunca, 1 = siempre)")]
    [Range(0f, 1f)]
    public float mutationRate = 0.01f;

    [Tooltip("Tipo de sesgo en la mutacion")]
    public MutationBias mutationBias = MutationBias.Random;

    [Tooltip("Alelo al que tiende la mutacion cuando mutationBias es TendToSpecific. " +
             "Si es TendToNormal, se usa el primer alelo de possibleAlleles automaticamente.")]
    public string biasTargetAllele;

    [Tooltip("Fuerza del sesgo (0 = sin sesgo, 1 = siempre muta al target). " +
             "Solo aplica cuando mutationBias != Random.")]
    [Range(0f, 1f)]
    public float biasStrength = 0.8f;

    [Header("Herencia sexual")]
    [Tooltip("Si es true, este gen esta ligado al cromosoma sexual. " +
             "Las hembras (ZW) solo tendran un alelo del padre. " +
             "Los machos (ZZ) tendran dos alelos normalmente.")]
    public bool isSexLinked;

    /// <summary>
    /// Devuelve un alelo aleatorio de los posibles (para primera generacion).
    /// </summary>
    public string GetRandomAllele()
    {
        if (possibleAlleles == null || possibleAlleles.Length == 0)
        {
            Debug.LogWarning($"GeneDefinition '{geneName}' no tiene alelos definidos.");
            return "";
        }
        return possibleAlleles[Random.Range(0, possibleAlleles.Length)];
    }

    /// <summary>
    /// Dado un alelo original, devuelve el alelo (posiblemente mutado).
    /// Aplica la tasa de mutacion y el sesgo configurado.
    /// </summary>
    public string PossiblyMutate(string originalAllele)
    {
        if (possibleAlleles == null || possibleAlleles.Length <= 1)
        {
            return originalAllele;
        }

        if (Random.value >= mutationRate)
        {
            return originalAllele;
        }

        // Hay mutacion! Decidir a que alelo muta
        return GetMutatedAllele(originalAllele);
    }

    private string GetMutatedAllele(string originalAllele)
    {
        switch (mutationBias)
        {
            case MutationBias.TendToNormal:
                return MutateWithBias(originalAllele, possibleAlleles[0]);

            case MutationBias.TendToSpecific:
                if (!string.IsNullOrEmpty(biasTargetAllele))
                {
                    return MutateWithBias(originalAllele, biasTargetAllele);
                }
                return MutateRandom(originalAllele);

            case MutationBias.Random:
            default:
                return MutateRandom(originalAllele);
        }
    }

    private string MutateWithBias(string originalAllele, string targetAllele)
    {
        if (Random.value < biasStrength)
        {
            // Mutacion sesgada: muta al target
            return targetAllele;
        }
        // Mutacion aleatoria (dentro del porcentaje no sesgado)
        return MutateRandom(originalAllele);
    }

    private string MutateRandom(string originalAllele)
    {
        // Elegir un alelo diferente al original
        string[] candidates = System.Array.FindAll(possibleAlleles, a => a != originalAllele);
        if (candidates.Length == 0)
        {
            return originalAllele;
        }
        return candidates[Random.Range(0, candidates.Length)];
    }
}
