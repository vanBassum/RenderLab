namespace Engine2D.Rendering.Pipeline
{
    public sealed class RenderPipeline2D
    {
        private readonly List<IRenderer2D> _stages = new();

        public void AddStage(IRenderer2D stage)
        {
            _stages.Add(stage);
        }

        public void Render(in RenderContext2D context)
        {
            foreach (var stage in _stages)
            {
                stage.Render(context);
            }
        }
    }
}
