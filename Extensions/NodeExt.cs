using Godot;

namespace EIV_Common.Extensions;

public static class NodeExt
{
    public static void SetMP(this Node node, int NetId)
    {
        if (node == null)
            return;
        node.SetMultiplayerAuthority(NetId);
        if (node.HasNode("MSync"))
            node.GetNode<MultiplayerSynchronizer>("MSync").SetMultiplayerAuthority(NetId);
    }
}
