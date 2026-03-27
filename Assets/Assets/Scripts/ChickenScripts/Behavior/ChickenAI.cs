using UnityEngine;

/// <summary>
/// Arbol de prioridades de comportamiento de la gallina.
/// Lee el genoma para modificar umbrales y probabilidades,
/// haciendo que cada gallina se comporte de forma unica.
///
/// Jerarquia de prioridades:
///   1. Sobrevivir (huir de amenazas)
///   2. Necesidades (comer, dormir)
///   3. Ocio (pasear, socializar, idle)
///
/// Modificadores geneticos:
///   - Crowd (Seguidora/Independiente/Rebelde): afecta socializacion y obediencia a horarios
///   - Awareness (Low/Medium/High): afecta deteccion de amenazas y sensibilidad al miedo
///   - Speed (Low/Medium/High): afecta velocidad de movimiento
/// </summary>
public class ChickenAI : MonoBehaviour
{
    [Header("Estado actual")]
    public MainGoal currentGoal;

    [Header("Referencias")]
    public ChickenNeeds needs;

    [Header("Umbrales base")]
    [Tooltip("Hambre por encima de este valor activa busqueda de comida")]
    public float hungerThreshold = 70f;

    [Tooltip("Energia por debajo de este valor activa descanso")]
    public float energyThreshold = 20f;

    [Tooltip("Miedo por encima de este valor activa huida")]
    public float fearThreshold = 50f;

    [Header("Velocidad de movimiento")]
    public float baseMoveSpeed = 3f;

    // Modificadores geneticos (se configuran en Initialize)
    private float hungerThresholdModifier;
    private float energyThresholdModifier;
    private float fearDetectionRange;
    private float moveSpeedMultiplier;
    private float socialChance;
    private float rebellionDelay;
    private bool isRebel;

    // Pequeno random anadido para que no sean robots
    private float personalRandomOffset;

    /// <summary>
    /// Inicializa la AI con los datos del genoma.
    /// </summary>
    public void Initialize(Genome genome)
    {
        if (needs == null)
        {
            needs = GetComponent<ChickenNeeds>();
        }

        // Cada gallina tiene un pequeno offset aleatorio personal
        personalRandomOffset = Random.Range(-5f, 5f);

        if (genome == null) return;

        ApplyGeneticModifiers(genome);
    }

    /// <summary>
    /// Aplica los modificadores geneticos al comportamiento.
    /// </summary>
    private void ApplyGeneticModifiers(Genome genome)
    {
        // === CROWD (Seguidora / Independiente / Rebelde) ===
        string crowd = genome.GetExpressedAllele("Crowd");
        switch (crowd)
        {
            case "Seguidora":
                socialChance = 0.4f;    // Le gusta estar con otras gallinas
                rebellionDelay = 0f;    // Obedece horarios
                isRebel = false;
                break;
            case "Independiente":
                socialChance = 0.15f;   // Le da igual
                rebellionDelay = 0f;
                isRebel = false;
                break;
            case "Rebelde":
                socialChance = 0.1f;    // Pasa de socializar
                rebellionDelay = Random.Range(5f, 15f); // Tarda X segundos extra en ir a dormir
                isRebel = true;
                break;
            default:
                socialChance = 0.2f;
                rebellionDelay = 0f;
                isRebel = false;
                break;
        }

        // === AWARENESS (Low / Medium / High) ===
        float awareness = genome.GetTraitValue("Awareness");
        fearDetectionRange = 3f + (awareness * 8f); // Rango: 3-11 unidades
        // Awareness alta = detecta amenazas antes pero tambien se asusta mas facil
        fearThreshold = 60f - (awareness * 30f); // Rango: 60 (Low) a 37.5 (High)

        // === SPEED (Low / Medium / High) ===
        float speed = genome.GetTraitValue("Speed");
        moveSpeedMultiplier = 0.6f + (speed * 0.8f); // Rango: 0.6x a 1.4x

        // Aplicar el offset personal a los umbrales
        hungerThresholdModifier = personalRandomOffset;
        energyThresholdModifier = personalRandomOffset * 0.5f;
    }

    void Update()
    {
        if (needs == null) return;

        DecideGoal();
        ExecuteCurrentGoal();
    }

    /// <summary>
    /// Decide la meta principal basada en prioridades y estado actual.
    /// </summary>
    private void DecideGoal()
    {
        // Prioridad 1: Sobrevivir
        if (DetectThreat())
        {
            currentGoal = MainGoal.Sobrevivir;
            return;
        }

        // Prioridad 2: Necesidades
        float effectiveHungerThreshold = hungerThreshold + hungerThresholdModifier;
        float effectiveEnergyThreshold = energyThreshold - energyThresholdModifier;

        if (needs.hunger > effectiveHungerThreshold || needs.energy < effectiveEnergyThreshold)
        {
            currentGoal = MainGoal.Needs;
            return;
        }

        // Prioridad 3: Ocio
        currentGoal = MainGoal.Ocio;
    }

    /// <summary>
    /// Ejecuta la accion correspondiente a la meta actual.
    /// </summary>
    private void ExecuteCurrentGoal()
    {
        switch (currentGoal)
        {
            case MainGoal.Sobrevivir:
                RunAway();
                break;

            case MainGoal.Needs:
                float effectiveHungerThreshold = hungerThreshold + hungerThresholdModifier;
                if (needs.hunger > effectiveHungerThreshold)
                {
                    SearchForFood();
                }
                else
                {
                    GoToRest();
                }
                break;

            case MainGoal.Ocio:
                DoLeisure();
                break;
        }
    }

    /// <summary>
    /// Actividades de ocio con probabilidades influenciadas por genetica.
    /// </summary>
    private void DoLeisure()
    {
        float roll = Random.value;

        // La probabilidad de socializar depende del gen Crowd
        float wanderChance = 1f - socialChance - 0.15f; // El resto

        if (roll < socialChance)
        {
            StartSocialize();
        }
        else if (roll < socialChance + wanderChance)
        {
            StartWandering();
        }
        else
        {
            StartAnyOtherIdle();
        }
    }

    /// <summary>
    /// Detecta amenazas cercanas. El rango depende del gen Awareness.
    /// </summary>
    private bool DetectThreat()
    {
        // Si el miedo ya esta alto, considerarlo amenaza
        if (needs.miedo > fearThreshold)
        {
            return true;
        }

        // TODO: Implementar deteccion de depredadores en fearDetectionRange
        // Ejemplo:
        // Collider[] threats = Physics.OverlapSphere(transform.position, fearDetectionRange, threatLayer);
        // if (threats.Length > 0) { needs.AddFear(30f); return true; }

        return false;
    }

    /// <summary>
    /// Logica para ir a dormir. Las gallinas rebeldes tardan mas.
    /// Si una gallina rebelde tiene mucho miedo y no hay gallinas cerca,
    /// el miedo puede forzarla a ir a dormir de todos modos.
    /// </summary>
    public void TriggerGoToSleep()
    {
        if (isRebel && needs.miedo < fearThreshold)
        {
            // La rebelde se resiste: espera rebellionDelay segundos extra
            // TODO: Implementar con corrutina o timer
            // StartCoroutine(DelayedSleep(rebellionDelay));
            return;
        }

        GoToSleep();
    }

    /// <summary>
    /// Devuelve la velocidad actual de movimiento (base * multiplicador genetico).
    /// </summary>
    public float GetMoveSpeed()
    {
        return baseMoveSpeed * moveSpeedMultiplier;
    }

    /// <summary>
    /// Devuelve true si esta gallina es rebelde.
    /// </summary>
    public bool IsRebel()
    {
        return isRebel;
    }

    /// <summary>
    /// Devuelve el delay de rebeldia (segundos extra que tarda en obedecer).
    /// </summary>
    public float GetRebellionDelay()
    {
        return rebellionDelay;
    }

    // === Metodos de accion (a implementar con logica de movimiento) ===

    private void RunAway()
    {
        // TODO: Moverse en direccion opuesta a la amenaza a velocidad maxima
    }

    private void SearchForFood()
    {
        // TODO: Buscar comida cercana y moverse hacia ella
    }

    private void GoToRest()
    {
        // TODO: Buscar un sitio tranquilo y descansar (de dia)
    }

    private void GoToSleep()
    {
        // TODO: Ir al gallinero a dormir (de noche)
    }

    private void StartWandering()
    {
        // TODO: Moverse aleatoriamente por el corral
    }

    private void StartSocialize()
    {
        // TODO: Acercarse a otra gallina cercana
    }

    private void StartAnyOtherIdle()
    {
        // TODO: Animacion idle aleatoria (rascarse, picotear suelo, etc)
    }
}
