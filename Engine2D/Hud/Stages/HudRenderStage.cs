using Engine2D.Rendering.Pipeline;

namespace Engine2D.Hud.Stages
{

    // =========================
    // HUD render stage
    // =========================
    public sealed class HudRenderStage : IRenderer2D
    {
        private readonly Action<HudDrawer> _Draw;

        public HudRenderStage(Action<HudDrawer> draw)
        {
            _Draw = draw;
        }

        public void Render(in RenderContext2D context)
        {
            HudDrawer drawer = new(context);
            _Draw(drawer);
        }
    }
}


