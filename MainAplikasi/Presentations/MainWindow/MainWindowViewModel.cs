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
            LoadingSprite = new();
        }

        public LoadingSprite LoadingSprite { get; set; }
    }
}
