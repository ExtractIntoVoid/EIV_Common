using Godot;

namespace EIV_Common.Extensions;

public static class NodeExt
{
    public static void SetMP(this Node node, int netId)
    {
        node.SetMultiplayerAuthority(netId);
        if (node.HasNode("MSync"))
            node.GetNode<MultiplayerSynchronizer>("MSync").SetMultiplayerAuthority(netId);
    }
}
