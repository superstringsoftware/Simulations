using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;
using System;


public partial struct SpawnerSystem : ISystem
{
    private bool _initialized;
    public void OnCreate(ref SystemState state) {
        // Queries for all Spawner components. Uses RefRW because this system wants
        // to read from and write to the component. If the system only needed read-only
        // access, it would use RefRO instead.
        Debug.Log("SpawnerSystem.OnCreate");
       _initialized = false;
    }

    public void OnDestroy(ref SystemState state) { }

    
    public void OnUpdate(ref SystemState state)
    {
        if (_initialized) return;
        Debug.Log("SpawnerSystem.OnUpdate");
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
        foreach (RefRO<Spawner> spawner in SystemAPI.Query<RefRO<Spawner>>())
        {
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            uint secondsSinceEpoch = (uint)t.TotalSeconds;
            var scale = spawner.ValueRO.Scale;
            var rnd = new Unity.Mathematics.Random(secondsSinceEpoch);
            for (int i = 0; i < spawner.ValueRO.TotalObjects; i++)
            {

                // Spawns a new entity and positions it at the spawner.
                Entity newEntity = state.EntityManager.Instantiate(spawner.ValueRO.Prefab);
                // LocalPosition.FromPosition returns a Transform initialized with the given position.
                float3 pos = 0.0f; //new Unity.Mathematics.float3(rnd.NextFloat() * scale, rnd.NextFloat() * scale, rnd.NextFloat() * scale);
                float polar = rnd.NextFloat() * 2 * math.PI;
                float alpha = rnd.NextFloat() * 2 * math.PI;
                float3 velUnit = math.normalize(new float3(math.sin(polar)*math.cos(alpha), math.sin(polar)*math.sin(alpha), math.cos(polar) ));                       //new Unity.Mathematics.float3(rnd.NextFloat() - 0.5f, rnd.NextFloat() - 0.5f, rnd.NextFloat()-0.5f));
                var vel = velUnit * (rnd.NextFloat()/2 + 1)* spawner.ValueRO.StartingVel;
                ecb.AddComponent<Body>(newEntity, new Body
                {
                    Mass = rnd.NextFloat() * 10.0f + 5f,
                    Velocity = vel,
                    Position = pos
                });
                state.EntityManager.SetComponentData(newEntity, LocalTransform.FromPosition(pos));
            }
        }
        //Dependency.Complete();

        // Now that the job is completed, you can enact the changes.
        // Note that Playback can only be called on the main thread.
        ecb.Playback(state.EntityManager);

        // You are responsible for disposing of any ECB you create.
        ecb.Dispose();
        _initialized = true;
    }

    
}