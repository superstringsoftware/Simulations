using UnityEngine;
using Unity.Entities;

class SpawnerAuthoring : MonoBehaviour
{
    public GameObject Prefab;
    public float SpawnRate;
    public int TotalObjects;
    public int Scale;
    public float DeltaTime;
    public float StartingVel;
}

class SpawnerBaker : Baker<SpawnerAuthoring>
{
    public override void Bake(SpawnerAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new Spawner
        {
            // By default, each authoring GameObject turns into an Entity.
            // Given a GameObject (or authoring component), GetEntity looks up the resulting Entity.
            Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
            SpawnPosition = authoring.transform.position,
            NextSpawnTime = 0.0f,
            SpawnRate = authoring.SpawnRate,
            TotalObjects = authoring.TotalObjects,
            Scale = authoring.Scale,
            DeltaTime = authoring.DeltaTime,
            StartingVel = authoring.StartingVel
        });
        AddComponent(entity, new Body
        {
            Position = authoring.transform.position,
            Mass = 1.0f,
            Velocity = Vector3.zero
        });
    }
}