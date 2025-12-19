namespace Engine2D.Tiles
{

    public interface ITileSource { }
    public sealed class HttpTileSource : ITileSource { }
    public sealed class DiskTileCache : ITileSource { }
    public sealed class MemoryTileCache : ITileSource { }
    public sealed class ProceduralTileSource : ITileSource { }

    public sealed class TileCoverageProvider { }

    public sealed class TileStitcher { }
    public sealed class TileScaler { }
    public sealed class ComposedTileCache { }

    //TileRenderItem has too much stuff

}
