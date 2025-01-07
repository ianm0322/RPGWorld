using System.Collections.Generic;
using UnityEngine;

public interface IHitboxCaster
{
    public HitboxSO Hitbox { get; set; }
    public IReadOnlyList<Collider2D> Overlap();
    public IReadOnlyList<Collider2D> Overlap(HitboxSO hitbox);
}
