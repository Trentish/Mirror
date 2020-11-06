using System;
using Mono.Cecil;
using UnityEngine;

namespace Mirror.Weaver
{
    public static class WeaverTypes
    {
        public static MethodReference ScriptableObjectCreateInstanceMethod;

        public static MethodReference NetworkBehaviourDirtyBitsReference;

        public static MethodReference BehaviorConnectionToServerReference;

        public static MethodReference NetworkServerGetActive;
        public static MethodReference NetworkServerGetLocalClientActive;
        public static MethodReference NetworkClientGetActive;

        public static MethodReference NetworkBehaviourGetIdentity;
        public static MethodReference NetworkIdentityGetServer;
        public static MethodReference NetworkIdentityGetClient;

        public static MethodReference NetworkBehaviourIsServer;
        public static MethodReference NetworkBehaviourIsClient;
        public static MethodReference NetworkBehaviourIsLocalClient;
        public static MethodReference NetworkBehaviourHasAuthority;
        public static MethodReference NetworkBehaviourIsLocalPlayer;

        public static MethodReference MethodInvocationExceptionConstructor;

        // custom attribute types
        public static MethodReference InitSyncObjectReference;

        public static MethodReference syncVarEqualReference;
        public static MethodReference syncVarNetworkIdentityEqualReference;
        public static MethodReference setSyncVarHookGuard;
        public static MethodReference getSyncVarHookGuard;
        public static MethodReference setSyncVarNetworkIdentityReference;
        public static MethodReference getSyncVarNetworkIdentityReference;
        public static MethodReference registerServerRpcDelegateReference;
        public static MethodReference registerRpcDelegateReference;
        public static MethodReference getTypeReference;
        public static MethodReference getTypeFromHandleReference;
        public static MethodReference logErrorReference;
        public static MethodReference logWarningReference;
        public static MethodReference sendServerRpcInternal;

        private static AssemblyDefinition currentAssembly;

        public static TypeReference Import<T>() => Import(typeof(T));

        public static TypeReference Import(Type t) => currentAssembly.MainModule.ImportReference(t);

        public static void SetupTargetTypes(AssemblyDefinition currentAssembly)
        {
            // system types
            WeaverTypes.currentAssembly = currentAssembly;

            TypeReference NetworkServerType = Import<NetworkServer>();
            NetworkServerGetActive = Resolvers.ResolveMethod(NetworkServerType, currentAssembly, "get_Active");
            NetworkServerGetLocalClientActive = Resolvers.ResolveMethod(NetworkServerType, currentAssembly, "get_LocalClientActive");
            TypeReference NetworkClientType = Import<NetworkClient>();
            NetworkClientGetActive = Resolvers.ResolveMethod(NetworkClientType, currentAssembly, "get_Active");

            TypeReference NetworkBehaviourType = Import<NetworkBehaviour>();
            TypeReference RemoteCallHelperType = Import(typeof(RemoteCalls.RemoteCallHelper));

            NetworkBehaviourGetIdentity = Resolvers.ResolveMethod(NetworkBehaviourType, currentAssembly, "get_NetIdentity");
            TypeReference NetworkIdentityType = Import<NetworkIdentity>();
            NetworkIdentityGetServer = Resolvers.ResolveMethod(NetworkIdentityType, currentAssembly, "get_Server");
            NetworkIdentityGetClient = Resolvers.ResolveMethod(NetworkIdentityType, currentAssembly, "get_Client");

            NetworkBehaviourIsServer = Resolvers.ResolveProperty(NetworkBehaviourType, currentAssembly, "IsServer");
            NetworkBehaviourIsClient = Resolvers.ResolveProperty(NetworkBehaviourType, currentAssembly, "IsClient");
            NetworkBehaviourIsLocalClient = Resolvers.ResolveProperty(NetworkBehaviourType, currentAssembly, "IsLocalClient");
            NetworkBehaviourHasAuthority = Resolvers.ResolveProperty(NetworkBehaviourType, currentAssembly, "HasAuthority");
            NetworkBehaviourIsLocalPlayer = Resolvers.ResolveProperty(NetworkBehaviourType, currentAssembly, "IsLocalPlayer");

            TypeReference ScriptableObjectType = Import<ScriptableObject>();
            ScriptableObjectCreateInstanceMethod = Resolvers.ResolveMethod(
                ScriptableObjectType, currentAssembly,
                md => md.Name == "CreateInstance" && md.HasGenericParameters);

            TypeReference MethodInvocationExceptionType = Import<MethodInvocationException>();
            MethodInvocationExceptionConstructor = Resolvers.ResolveMethodWithArg(MethodInvocationExceptionType, currentAssembly, ".ctor", "System.String");

            NetworkBehaviourDirtyBitsReference = Resolvers.ResolveProperty(NetworkBehaviourType, currentAssembly, "SyncVarDirtyBits");

            BehaviorConnectionToServerReference = Resolvers.ResolveMethod(NetworkBehaviourType, currentAssembly, "get_ConnectionToServer");

            syncVarEqualReference = Resolvers.ResolveMethod(NetworkBehaviourType, currentAssembly, "SyncVarEqual");
            syncVarNetworkIdentityEqualReference = Resolvers.ResolveMethod(NetworkBehaviourType, currentAssembly, "SyncVarNetworkIdentityEqual");
            setSyncVarHookGuard = Resolvers.ResolveMethod(NetworkBehaviourType, currentAssembly, "SetSyncVarHookGuard");
            getSyncVarHookGuard = Resolvers.ResolveMethod(NetworkBehaviourType, currentAssembly, "GetSyncVarHookGuard");

            setSyncVarNetworkIdentityReference = Resolvers.ResolveMethod(NetworkBehaviourType, currentAssembly, "SetSyncVarNetworkIdentity");
            getSyncVarNetworkIdentityReference = Resolvers.ResolveMethod(NetworkBehaviourType, currentAssembly, "GetSyncVarNetworkIdentity");
            registerServerRpcDelegateReference = Resolvers.ResolveMethod(RemoteCallHelperType, currentAssembly, "RegisterServerRpcDelegate");
            registerRpcDelegateReference = Resolvers.ResolveMethod(RemoteCallHelperType, currentAssembly, "RegisterRpcDelegate");
            TypeReference unityDebug = Import(typeof(Debug));
            logErrorReference = Resolvers.ResolveMethod(unityDebug, currentAssembly, "LogError");
            logWarningReference = Resolvers.ResolveMethod(unityDebug, currentAssembly, "LogWarning");

            TypeReference typeType = Import(typeof(Type));
            getTypeFromHandleReference = Resolvers.ResolveMethod(typeType, currentAssembly, "GetTypeFromHandle");
            sendServerRpcInternal = Resolvers.ResolveMethod(NetworkBehaviourType, currentAssembly, "SendServerRpcInternal");

            InitSyncObjectReference = Resolvers.ResolveMethod(NetworkBehaviourType, currentAssembly, "InitSyncObject");

        }
    }
}
