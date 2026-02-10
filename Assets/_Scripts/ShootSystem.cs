using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
partial struct ShootSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        NetworkTime networkTime = SystemAPI.GetSingleton<NetworkTime>();

        foreach(RefRO<NetcodePlayerInput> netcodePlayerInput in SystemAPI.Query<RefRO<NetcodePlayerInput>>().WithAll<Simulate>()){
            if (networkTime.IsFirstTimeFullyPredictingTick)
            {
                if (netcodePlayerInput.ValueRO.shoot.IsSet)
                {
                    Debug.Log("shoot TRUE" + state.World);
                }
            }
        }
    }
}
