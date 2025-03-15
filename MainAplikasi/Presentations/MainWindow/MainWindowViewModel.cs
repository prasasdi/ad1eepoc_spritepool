using MainAplikasi.Enums;
using MainAplikasi.Helpers;
using MainAplikasi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainAplikasi.Presentations.MainWindow
{
    public class MainWindowViewModel
    {
        private readonly IServiceProvider _serviceProvider;
        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            LoadingSprite = new(E_FrameRate.FPS60);
            RunSpriteRight = new();
            RunSpriteLeft = new(direction: E_RunDirection.RunLeft);
        }

        public LoadingSprite LoadingSprite { get; set; }
        public RunSprite RunSpriteRight { get; set; }
        public RunSprite RunSpriteLeft { get; set; }
    }
}
