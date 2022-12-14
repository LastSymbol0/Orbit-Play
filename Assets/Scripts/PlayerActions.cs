using System.Linq;
using Mirror;
using UnityEngine;

/// <summary>
/// Correspond for Player-specific actions: particles consuming, TODO: movement, TODO:shooting
/// </summary>
public class PlayerActions : NetworkBehaviour
{
    public float SpeedScaleMax = 0.2f;
    public float ParticlesConsumingTime = 1f;
    public Color FinalColor = Color.red;
    public float ParticlesConsumingProgress => ParticlesConsumingTimeProgress / ParticlesConsumingTime;
    public float ParticlesConsumingTimeProgress { get; private set; }
    public bool OpenToConsume => ParticlesConsumingProgress > 0.4f;

    private float _startSpeed;
    private float _startOrbitShift;
    private Color _startColor;
    private float _startColliderRadius;

    private float _finalSpeed;
    private float _finalOrbitShift;
    private  Color _finalColor;
    private float _finalColliderRadius;
    
    private Attractor _attractor;
    private SpriteRenderer _spriteRenderer;
    private CircleCollider2D _innerCollider;
    private CircleCollider2D _outerCollider;

    void Start()
    {
        _attractor = GetComponent<Attractor>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _innerCollider = GetComponents<CircleCollider2D>().Single(x => !x.isTrigger);
        
        _startSpeed = _attractor.SpeedScale;
        _startOrbitShift = _attractor.OrbitDistanceShift;
        _startColor = _spriteRenderer.color;
        _startColliderRadius = _innerCollider.radius;
        
        _finalSpeed = _attractor.SpeedScale * SpeedScaleMax;
        _finalColor = FinalColor;
        _finalColliderRadius = 0f + 0.01f; // 0.01f - threshold 
        _finalOrbitShift = _startOrbitShift - 1f;
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        // TODO: add mobile control
        if (Input.GetKey(KeyCode.Space))
        {
            if (ParticlesConsumingTimeProgress < ParticlesConsumingTime)
            {
                ParticlesConsumingTimeProgress += Time.deltaTime;

                PlayAnimation();
            }
        }
        else if (ParticlesConsumingTimeProgress > 0.1f)
        {
            ParticlesConsumingTimeProgress -= Time.deltaTime * 5;

            PlayAnimation();
        }
    }

    private void PlayAnimation()
    {
        // TODO: add networking sync
        
        _attractor.SpeedScale = Mathf.Lerp(_startSpeed, _finalSpeed, ParticlesConsumingProgress);
        _attractor.OrbitDistanceShift = Mathf.Lerp(_startOrbitShift, _finalOrbitShift, ParticlesConsumingProgress);

        _spriteRenderer.color = Color.Lerp(_startColor, _finalColor, ParticlesConsumingProgress);

        _innerCollider.radius = Mathf.Lerp(_startColliderRadius, _finalColliderRadius, ParticlesConsumingProgress);
    }
}
