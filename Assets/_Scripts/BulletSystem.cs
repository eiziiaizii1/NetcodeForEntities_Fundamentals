using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
partial struct BulletSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach ((
            RefRW<LocalTransform> localTransform,
            RefRW<Bullet> bullet,
            Entity entity)
            in SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRW<Bullet>>().WithEntityAccess().WithAll<Simulate>())
        {

            float moveSpeed = 10f;
            localTransform.ValueRW.Position += new Unity.Mathematics.float3(0, 0, 1) * moveSpeed * SystemAPI.Time.DeltaTime;

            if (state.World.IsServer())
            {
                bullet.ValueRW.timer -= SystemAPI.Time.DeltaTime;
                if (bullet.ValueRW.timer <= 0f)
                {
                    entityCommandBuffer.DestroyEntity(entity);
                }
            }
        }
        entityCommandBuffer.Playback(state.EntityManager);
    }
}
