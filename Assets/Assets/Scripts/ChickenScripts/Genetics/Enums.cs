/// Enums del sistema de genetica

public enum ChickenSex
{
    Male,
    Female
}

public enum MutationBias
{
    Random,        // Muta a cualquier alelo al azar
    TendToNormal,  // Tiende a mutar al primer alelo de possibleAlleles (el "normal")
    TendToSpecific // Tiende a mutar a un alelo especifico definido en biasTargetAllele
}

/// Enums generales del juego

public enum MainGoal
{
    Sobrevivir,
    Needs,
    Ocio
}

public enum GrowthStage
{
    Egg,
    Chick,
    Adult
}
