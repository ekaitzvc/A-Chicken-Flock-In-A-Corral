using UnityEngine;

/// <summary>
/// Utilidades estaticas para crear genomas y gestionar herencia.
/// </summary>
public static class GeneHelper
{
    /// <summary>
    /// Crea un genoma completamente aleatorio (para la primera generacion).
    /// </summary>
    public static Genome CreateRandomGenome(GeneCatalog catalog)
    {
        Genome genome = new Genome();
        genome.sex = Random.value > 0.5f ? ChickenSex.Male : ChickenSex.Female;

        foreach (GeneDefinition geneDef in catalog.allGenes)
        {
            Gene gene = CreateRandomGene(geneDef, genome.sex);
            genome.genes.Add(gene);
        }

        return genome;
    }

    /// <summary>
    /// Crea un genoma por herencia de dos padres.
    /// Cada gen del hijo recibe un alelo de cada padre con posibilidad de mutacion.
    /// </summary>
    public static Genome CreateFromParents(GeneCatalog catalog, Genome father, Genome mother)
    {
        Genome child = new Genome();
        child.sex = Random.value > 0.5f ? ChickenSex.Male : ChickenSex.Female;

        foreach (GeneDefinition geneDef in catalog.allGenes)
        {
            Gene fatherGene = father.GetGene(geneDef.geneName);
            Gene motherGene = mother.GetGene(geneDef.geneName);

            Gene childGene = CreateInheritedGene(geneDef, fatherGene, motherGene, child.sex);
            child.genes.Add(childGene);
        }

        return child;
    }

    /// <summary>
    /// Crea un gen aleatorio para una gallina (primera generacion).
    /// </summary>
    private static Gene CreateRandomGene(GeneDefinition geneDef, ChickenSex sex)
    {
        string allele1 = geneDef.GetRandomAllele();

        // Si es sex-linked y la gallina es hembra (ZW), solo un alelo del padre
        if (geneDef.isSexLinked && sex == ChickenSex.Female)
        {
            return new Gene(geneDef, allele1);
        }

        string allele2 = geneDef.GetRandomAllele();
        return new Gene(geneDef, allele1, allele2);
    }

    /// <summary>
    /// Crea un gen heredado de dos padres.
    /// En aves: machos ZZ tienen dos alelos, hembras ZW solo uno del padre en genes sex-linked.
    /// </summary>
    private static Gene CreateInheritedGene(
        GeneDefinition geneDef,
        Gene fatherGene,
        Gene motherGene,
        ChickenSex childSex)
    {
        // Obtener alelo del padre (con posible mutacion)
        string fromFather;
        if (fatherGene != null)
        {
            fromFather = fatherGene.GetAlleleForOffspring();
        }
        else
        {
            fromFather = geneDef.GetRandomAllele();
        }

        // Si es sex-linked y el hijo es hembra, solo recibe del padre
        // (la mutacion ya se aplico en GetAlleleForOffspring arriba)
        if (geneDef.isSexLinked && childSex == ChickenSex.Female)
        {
            return new Gene(geneDef, fromFather);
        }

        // Obtener alelo de la madre (con posible mutacion)
        string fromMother;
        if (motherGene != null)
        {
            fromMother = motherGene.GetAlleleForOffspring();
        }
        else
        {
            fromMother = geneDef.GetRandomAllele();
        }

        return new Gene(geneDef, fromFather, fromMother);
    }
}
