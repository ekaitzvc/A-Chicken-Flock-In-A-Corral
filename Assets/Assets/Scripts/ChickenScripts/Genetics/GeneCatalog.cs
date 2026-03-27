using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Catalogo de todos los genes disponibles en el juego.
/// Se crea como asset: clic derecho > Create > Genes > GeneCatalog.
/// </summary>
[CreateAssetMenu(fileName = "GeneCatalog", menuName = "Genes/GeneCatalog")]
public class GeneCatalog : ScriptableObject
{
    [Tooltip("Lista de todas las definiciones de genes del juego")]
    public List<GeneDefinition> allGenes = new List<GeneDefinition>();
}
