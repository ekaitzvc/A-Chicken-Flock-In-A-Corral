using UnityEngine;

/// <summary>
/// Gestiona las etapas de crecimiento de la gallina:
/// Huevo -> Pollito -> Adulto.
/// Notifica a otros componentes cuando cambia de etapa.
/// </summary>
public class ChickenGrowth : MonoBehaviour
{
    [Header("Configuracion de tiempos (segundos de juego)")]
    [Tooltip("Tiempo que tarda en eclosionar el huevo")]
    public float eggDuration = 30f;

    [Tooltip("Tiempo que dura la fase de pollito antes de ser adulto")]
    public float chickDuration = 60f;

    [Header("Estado actual")]
    public GrowthStage currentStage = GrowthStage.Egg;

    [Tooltip("Tiempo acumulado en la etapa actual")]
    public float stageTimer;

    [Header("Referencias opcionales")]
    [Tooltip("GameObject del modelo de huevo (se desactiva al eclosionar)")]
    public GameObject eggModel;

    [Tooltip("GameObject del modelo de gallina (se activa al eclosionar)")]
    public GameObject chickenModel;

    // Evento que se dispara al cambiar de etapa
    public event System.Action<GrowthStage> OnStageChanged;

    /// <summary>
    /// Inicializa el crecimiento. Llamar al crear la gallina.
    /// startStage permite comenzar desde cualquier etapa (ej: Adult para gallinas iniciales).
    /// </summary>
    public void Initialize(GrowthStage startStage = GrowthStage.Egg)
    {
        currentStage = startStage;
        stageTimer = 0f;
        ApplyStageVisuals();
        gameObject.GetComponent<ChickenAppearance>().SetBodyPartObjetcs();
    }

    void Update()
    {
        if (currentStage == GrowthStage.Adult)
        {
            return; // Ya crecida, no hacer nada
        }

        stageTimer += Time.deltaTime;

        if (currentStage == GrowthStage.Egg && stageTimer >= eggDuration)
        {
            TransitionTo(GrowthStage.Chick);
        }
        else if (currentStage == GrowthStage.Chick && stageTimer >= chickDuration)
        {
            TransitionTo(GrowthStage.Adult);
        }

        /*if (currentStage == GrowthStage.Chick)
        {
            
        }*/
    }

    /// <summary>
    /// Transiciona a una nueva etapa de crecimiento.
    /// </summary>
    private void TransitionTo(GrowthStage newStage)
    {
        currentStage = newStage;
        stageTimer = 0f;
        ApplyStageVisuals();
        OnStageChanged?.Invoke(newStage);
    }

    /// <summary>
    /// Activa/desactiva modelos segun la etapa.
    /// </summary>
    private void ApplyStageVisuals()
    {
        switch (currentStage)
        {
            case GrowthStage.Egg:
                if (eggModel != null) eggModel.SetActive(true);
                if (chickenModel != null) chickenModel.SetActive(false);
                Debug.Log("EGG");
                break;

            case GrowthStage.Chick:
                if (eggModel != null) eggModel.SetActive(false);
                if (chickenModel != null) chickenModel.SetActive(true);
                Debug.Log("CHICK");
                break;

            case GrowthStage.Adult:
                if (eggModel != null) eggModel.SetActive(false);
                if (chickenModel != null) chickenModel.SetActive(true);
                Debug.Log("ADULT");
                break;
        }
    }

    /// <summary>
    /// Devuelve el progreso de la etapa actual (0 a 1).
    /// </summary>
    public float GetStageProgress()
    {
        switch (currentStage)
        {
            case GrowthStage.Egg:
                return Mathf.Clamp01(stageTimer / eggDuration);
            case GrowthStage.Chick:
                return Mathf.Clamp01(stageTimer / chickDuration);
            default:
                return 1f;
        }
    }
}
