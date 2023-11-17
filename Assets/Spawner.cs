using Unity.Entities;
using Unity.Mathematics;

public struct Spawner : IComponentData
{
    public Entity Prefab;
    public float3 SpawnPosition;
    public float NextSpawnTime;
    public float SpawnRate;
    public int TotalObjects;
    public int Scale;
    public float DeltaTime;
    public float StartingVel;
}

public struct Body : IComponentData
{
    public float3 Position;
    public float Mass;
    public float3 Velocity;
}