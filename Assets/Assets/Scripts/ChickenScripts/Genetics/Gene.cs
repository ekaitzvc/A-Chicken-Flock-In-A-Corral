using UnityEngine;

/// <summary>
/// Instancia de un gen en una gallina concreta.
/// Contiene la referencia a su definicion y los dos alelos heredados.
/// </summary>
[System.Serializable]
public class Gene
{
    [Tooltip("Definicion del gen (ScriptableObject)")]
    public GeneDefinition definition;

    [Tooltip("Alelo heredado del padre")]
    public string allele1;

    [Tooltip("Alelo heredado de la madre (vacio si es sex-linked en hembra)")]
    public string allele2;

    /// <summary>
    /// Crea un gen con dos alelos especificos.
    /// </summary>
    public Gene(GeneDefinition def, string a1, string a2)
    {
        definition = def;
        allele1 = a1;
        allele2 = a2;
    }

    /// <summary>
    /// Crea un gen con un solo alelo (para genes sex-linked en hembras).
    /// </summary>
    public Gene(GeneDefinition def, string a1)
    {
        definition = def;
        allele1 = a1;
        allele2 = null;
    }

    /// <summary>
    /// Devuelve true si este gen solo tiene un alelo (sex-linked en hembra).
    /// </summary>
    public bool IsHemizygous()
    {
        return string.IsNullOrEmpty(allele2);
    }

    /// <summary>
    /// Devuelve el alelo que se expresa fenotipicamente.
    /// Si es hemizigoto (un solo alelo), se expresa ese directamente.
    /// Si tiene dos alelos, usa el orden de dominancia del GeneDefinition.
    /// </summary>
    public string GetExpressedAllele()
    {
        if (IsHemizygous())
        {
            return allele1;
        }

        if (allele1 == allele2)
        {
            return allele1;
        }

        // Buscar cual es mas dominante segun dominanceOrder
        if (definition.dominanceOrder != null && definition.dominanceOrder.Length > 0)
        {
            foreach (string dominant in definition.dominanceOrder)
            {
                if (dominant == allele1) return allele1;
                if (dominant == allele2) return allele2;
            }
        }

        // Si no hay orden definido, devolver el primero
        return allele1;
    }

    /// <summary>
    /// Elige un alelo al azar de los dos para pasar al hijo,
    /// aplicando la posibilidad de mutacion.
    /// </summary>
    public string GetAlleleForOffspring()
    {
        string chosen;

        if (IsHemizygous())
        {
            chosen = allele1;
        }
        else
        {
            chosen = Random.value > 0.5f ? allele1 : allele2;
        }

        return definition.PossiblyMutate(chosen);
    }

    /// <summary>
    /// Devuelve true si la gallina tiene al menos un alelo con el valor dado.
    /// Util para comprobar portadores de alelos recesivos.
    /// </summary>
    public bool HasAllele(string alleleValue)
    {
        return allele1 == alleleValue || allele2 == alleleValue;
    }

    /// <summary>
    /// Devuelve true si ambos alelos son iguales (homocigoto).
    /// </summary>
    public bool IsHomozygous()
    {
        return !IsHemizygous() && allele1 == allele2;
    }
}
