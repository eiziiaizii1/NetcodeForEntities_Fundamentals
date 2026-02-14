using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
partial struct ShootSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        NetworkTime networkTime = SystemAPI.GetSingleton<NetworkTime>();
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        foreach ((
            RefRO<NetcodePlayerInput> netcodePlayerInput,
            RefRO<LocalTransform> localTransform,
            RefRO<GhostOwner> ghostOwner)
            in SystemAPI.Query<
                RefRO<NetcodePlayerInput>,
                RefRO<LocalTransform>,
                RefRO<GhostOwner>>().WithAll<Simulate>())
        {

            if (networkTime.IsFirstTimeFullyPredictingTick)
            {
                if (netcodePlayerInput.ValueRO.shoot.IsSet)
                {
                    Debug.Log("Shoot true! " + state.World);

                    Entity bulletEntity = entityCommandBuffer.Instantiate(entitiesReferences.bulletPrefabEntity);
                    entityCommandBuffer.SetComponent(bulletEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));
                    entityCommandBuffer.SetComponent(bulletEntity, new GhostOwner { NetworkId = ghostOwner.ValueRO.NetworkId });
                }
            }
        }
        entityCommandBuffer.Playback(state.EntityManager);
    }
}
