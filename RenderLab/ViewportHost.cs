using Engine2D.Input;
using RenderLab.Targets.WinForms;

namespace RenderLab
{
    public class ViewportHost
    {
        public required PictureBoxRenderHost RenderHost;
        public required InputQueue Input;
        public required CameraPanZoomHandler CameraInput;
    }



}