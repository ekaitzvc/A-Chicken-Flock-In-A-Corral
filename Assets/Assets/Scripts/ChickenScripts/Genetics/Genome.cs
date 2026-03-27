using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Genoma completo de una gallina.
/// Contiene su sexo y la lista de todos sus genes.
/// </summary>
[System.Serializable]
public class Genome
{
    public ChickenSex sex;
    public List<Gene> genes = new List<Gene>();

    /// <summary>
    /// Busca un gen por nombre.
    /// </summary>
    public Gene GetGene(string geneName)
    {
        for (int i = 0; i < genes.Count; i++)
        {
            if (genes[i].definition != null && genes[i].definition.geneName == geneName)
            {
                return genes[i];
            }
        }
        Debug.LogWarning($"Gen '{geneName}' no encontrado en el genoma.");
        return null;
    }

    /// <summary>
    /// Shortcut: devuelve el alelo expresado de un gen por nombre.
    /// </summary>
    public string GetExpressedAllele(string geneName)
    {
        Gene gene = GetGene(geneName);
        if (gene != null)
        {
            return gene.GetExpressedAllele();
        }
        return "";
    }

    /// <summary>
    /// Shortcut: comprueba si la gallina tiene un alelo especifico en un gen.
    /// </summary>
    public bool HasAllele(string geneName, string alleleValue)
    {
        Gene gene = GetGene(geneName);
        if (gene != null)
        {
            return gene.HasAllele(alleleValue);
        }
        return false;
    }

    /// <summary>
    /// Devuelve el valor numerico de un gen de comportamiento.
    /// Mapea los alelos tipicos "Low"/"Medium"/"High" a valores float.
    /// Se puede personalizar los mapeos pasando un diccionario.
    /// </summary>
    public float GetTraitValue(string geneName, Dictionary<string, float> alleleToValue = null)
    {
        string expressed = GetExpressedAllele(geneName);
        if (string.IsNullOrEmpty(expressed))
        {
            return 0.5f; // Valor por defecto si no se encuentra
        }

        if (alleleToValue != null && alleleToValue.ContainsKey(expressed))
        {
            return alleleToValue[expressed];
        }

        // Mapeo por defecto para alelos comunes
        switch (expressed)
        {
            case "Low": return 0.25f;
            case "Medium": return 0.5f;
            case "High": return 0.75f;
            case "VeryHigh": return 1.0f;
            case "VeryLow": return 0.1f;
            case "Small": return 0.3f;
            case "Big": return 0.7f;
            case "Mini": return 0.15f;
            case "SuperLarge": return 1.0f;
            default: return 0.5f;
        }
    }
}
