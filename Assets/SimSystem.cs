using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Mathematics;
using System.Diagnostics;

[BurstCompile]
public partial struct SimSystem : ISystem
{
    public int counter;
    public void OnCreate(ref SystemState state) {
        counter = 0;
    }

    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //UnityEngine.Debug.Log("Scheduling SimulationJob " + counter++);
        EntityQuery query = new EntityQueryBuilder(Allocator.Temp).WithAllRW<Body>().WithAllRW<LocalTransform>().Build(ref state);
        // Creates a new instance of the job, assigns the necessary data, and schedules the job in parallel.
        new SimulationJob
        {
            deltaTime = 0.01f,
            rnd = new Unity.Mathematics.Random(1),
            bodies = query.ToComponentDataArray<Body>(Allocator.TempJob)
        }.ScheduleParallel();
    }

    
}

[BurstCompile]
public partial struct SimulationJob : IJobEntity
{
    public float deltaTime;
    public Unity.Mathematics.Random rnd;
    //[NativeDisableParallelForRestriction]
    [ReadOnly]
    public NativeArray<Body> bodies;

    // IJobEntity generates a component data query based on the parameters of its `Execute` method.
    // This example queries for all Spawner components and uses `ref` to specify that the operation
    // requires read and write access. Unity processes `Execute` for each entity that matches the
    // component data query.
    [BurstCompile]
    private void Execute(ref Body body, ref LocalTransform transform)
    {
        float3 acc = 0.0f;
        for (int i = 0; i < bodies.Length; i++)
        {
            var r = bodies[i].Position - body.Position;
            var unit = math.normalize(r);
            var r2 = r.x * r.x + r.y * r.y + r.z * r.z;
            if ( (r2 < 100f) )
                continue;
            
            var accScalar = bodies[i].Mass / r2;
            acc += accScalar * unit;
        }
        //UnityEngine.Debug.Log("Acc = " + acc);
        body.Velocity += acc * deltaTime;
        body.Position += body.Velocity * deltaTime;
        transform.Position = body.Position;
    }
}