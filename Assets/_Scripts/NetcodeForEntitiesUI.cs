using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine.SceneManagement;
using Unity.Networking.Transport;

public class NetcodeForEntitiesUI : MonoBehaviour
{

    [SerializeField] private Button startServerButton;
    [SerializeField] private Button joinGameButton;

    private void Awake()
    {
        startServerButton.onClick.AddListener(StartServer);
        joinGameButton.onClick.AddListener(JoinGame);
    }

    private void StartServer()
    {
        World serverWorld = ClientServerBootstrap.CreateServerWorld("ServerWorld");
        World clientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");

        foreach (World world in World.All)
        {
            if (world.Flags == WorldFlags.Game)
            {
                world.Dispose();
                break;
            }
        }

        if (World.DefaultGameObjectInjectionWorld == null)
        {
            World.DefaultGameObjectInjectionWorld = serverWorld;
        }

        SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Single);

        ushort port = 7979;

        RefRW<NetworkStreamDriver> networkStreamDriver =
            serverWorld.EntityManager.CreateEntityQuery(typeof(NetworkStreamDriver)).GetSingletonRW<NetworkStreamDriver>();
        networkStreamDriver.ValueRW.Listen(NetworkEndpoint.AnyIpv4.WithPort(port));

        NetworkEndpoint connectNetworkEndpoint = NetworkEndpoint.LoopbackIpv4.WithPort(port);
        networkStreamDriver =
            clientWorld.EntityManager.CreateEntityQuery(typeof(NetworkStreamDriver)).GetSingletonRW<NetworkStreamDriver>();
        networkStreamDriver.ValueRW.Connect(clientWorld.EntityManager, connectNetworkEndpoint);
    }

    private void JoinGame()
    {
        World clientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");

        foreach (World world in World.All)
        {
            if (world.Flags == WorldFlags.Game)
            {
                world.Dispose();
                break;
            }
        }

        if (World.DefaultGameObjectInjectionWorld == null)
        {
            World.DefaultGameObjectInjectionWorld = clientWorld;
        }

        SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Single);

        ushort port = 7979;
        string ip = "127.0.0.1";

        NetworkEndpoint connectNetworkEndpoint = NetworkEndpoint.Parse(ip, port);
        RefRW<NetworkStreamDriver> networkStreamDriver =
            clientWorld.EntityManager.CreateEntityQuery(typeof(NetworkStreamDriver)).GetSingletonRW<NetworkStreamDriver>();
        networkStreamDriver.ValueRW.Connect(clientWorld.EntityManager, connectNetworkEndpoint);
    }
}