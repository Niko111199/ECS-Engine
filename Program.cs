using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Graphics
{
    class Program
    {
        static void Main()
        {
            using (Window window = new Window(800, 600, "ECS Grafik projekt"))
            {
                window.Run();
            }
        }
    }
}
