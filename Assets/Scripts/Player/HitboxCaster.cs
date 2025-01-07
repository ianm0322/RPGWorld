using System.Collections.Generic;
using UnityEngine;

public class HitboxCaster : MonoBehaviour, IHitboxCaster
{
    [SerializeField]
    private HitboxSO _hitbox;
    [SerializeField]
    private LayerMask _hitLayer;


    public HitboxSO Hitbox
    {
        get => _hitbox;
        set => _hitbox = value;
    }


    private IPlayerController _playerController;
    private Collider2D _collider;

    private ContactFilter2D _hitFilter;
    private List<Collider2D> _hitResults = new List<Collider2D>();

    private void Start()
    {
        _playerController = GetComponent<IPlayerController>();
        _collider = GetComponent<Collider2D>();

        _hitFilter.layerMask = _hitLayer;
    }


    public IReadOnlyList<Collider2D> Overlap()
    {
        _collider.enabled = false;

        Vector2 offset = _hitbox.Point;
        offset.x *= _playerController.Direction == Direction1D.Left ? -1 : 1;
        float angle = 0;

        _hitResults.Clear();
        Physics2D.OverlapBox(_playerController.Position + offset, _hitbox.Size, angle, _hitFilter, _hitResults);

        _collider.enabled = true;

#if UNITY_EDITOR
        _debug_hitbox_enabled = true;
#endif

        return _hitResults;
    }

    public IReadOnlyList<Collider2D> Overlap(HitboxSO hitbox)
    {
        _collider.enabled = false;

        Vector2 offset = hitbox.Point;
        offset.x *= _playerController.Direction == Direction1D.Left ? -1 : 1;
        float angle = 0;

        _hitResults.Clear();
        Physics2D.OverlapBox(_playerController.Position + offset, hitbox.Size, angle, _hitFilter, _hitResults);

        _collider.enabled = true;

#if UNITY_EDITOR
        DebugUtility.DrawBox(_playerController.Position + offset, hitbox.Size, Color.green, 1.5f);
#endif


        return _hitResults;
    }


#if UNITY_EDITOR
    bool _debug_hitbox_enabled;

    private void OnDrawGizmos()
    {
        if (_hitbox != null)
        {
            Gizmos.color = _debug_hitbox_enabled ? Color.green : Color.red;
            _debug_hitbox_enabled = false;

            Vector2 position = this.transform.position;
            Vector2 offset = _hitbox.Point;
            if (_playerController != null)
            {
                position = _playerController.Position;
                offset.x *= _playerController.Direction == Direction1D.Left ? -1 : 1;
            }
            Gizmos.DrawWireCube(this.transform.position + (Vector3)offset, _hitbox.Size);
        }
    }
#endif
}
