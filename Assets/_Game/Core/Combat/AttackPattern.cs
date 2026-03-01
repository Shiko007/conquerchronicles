namespace ConquerChronicles.Core.Combat
{
    public enum AttackPattern
    {
        MeleeSwing,       // Arc in front of character
        RangedSingle,     // Single projectile to nearest
        RangedPiercing,   // Projectile passes through enemies
        AoECircle,        // Damage in radius around player
        AoECone,          // Cone-shaped area in direction
        MultiProjectile,  // N projectiles in spread
        Orbiting,         // Orbs circling player
        Chain,            // Jumps between N enemies
        Nova,             // Expanding ring from player
        SummonZone        // Persistent damage zone on ground
    }
}
