using Godot;
using System.Diagnostics;

public partial class TestScript : Node {

    public override void _Ready() {
        // Check masks
        Debug.Assert(MaskForLayer(1) == LayerNames.Render2D.ImLayer1Mask);
        Debug.Assert(MaskForLayer(2) == LayerNames.Render2D.ImLayer2Mask);
        Debug.Assert(MaskForLayer(4) == LayerNames.Render2D.SkipTo4Mask);
        Debug.Assert(MaskForLayer(10) == LayerNames.Render2D.SkipAllTheWayTo10Mask);

        Debug.Assert(MaskForLayer(2) == LayerNames.Render3D.Render2Mask);

        Debug.Assert(MaskForLayer(1) == LayerNames.Physics2D.PhyLayer1Mask);
        Debug.Assert(MaskForLayer(2) == LayerNames.Physics2D.PhyLayer2Mask);
        Debug.Assert(MaskForLayer(4) == LayerNames.Physics2D.PhySkipTo4Mask);
        Debug.Assert(MaskForLayer(5) == LayerNames.Physics2D.Layer5WithSpaceMask);
        Debug.Assert(MaskForLayer(6) == LayerNames.Physics2D.Lowercase6Mask);
        Debug.Assert(MaskForLayer(7) == LayerNames.Physics2D._7IsTheLayerMask);

        Debug.Assert(MaskForLayer(1) == LayerNames.Navigation2D.Nav1Mask);
        Debug.Assert(MaskForLayer(2) == LayerNames.Navigation2D.Nav2Mask);

        Debug.Assert(MaskForLayer(1) == LayerNames.Physics3D.Phy3D1Mask);
        Debug.Assert(MaskForLayer(3) == LayerNames.Physics3D.Phy3D3Mask);

        Debug.Assert(MaskForLayer(1) == LayerNames.Navigation3D.Nav3DLayer1Mask);
        Debug.Assert(MaskForLayer(2) == LayerNames.Navigation3D.Nav3DLayer2Mask);
        Debug.Assert(MaskForLayer(3) == LayerNames.Navigation3D.Nav3DLayer3Mask);

        Debug.Assert(MaskForLayer(1) == LayerNames.Avoidance.Avoid1Mask);
        Debug.Assert(MaskForLayer(2) == LayerNames.Avoidance.Avoid2Mask);

        // Check indexes. Yes they're off by 1. It's very confusing.
        // But it makes it work with functions like CanvasLayer.SetVisibilityLayerBit()
        GD.Print(LayerNames.Render2D.ImLayer1Index);
        Debug.Assert(0 == LayerNames.Render2D.ImLayer1Index);
        Debug.Assert(1 == LayerNames.Render2D.ImLayer2Index);
    }

    private uint MaskForLayer(int index) {
        return 1u << (index-1);
    }
}
