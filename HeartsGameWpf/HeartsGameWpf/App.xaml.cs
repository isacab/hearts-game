using HeartsGameEngine;
using HeartsGameEngine.DataObjects;
using HeartsGameWpf.View;
using HeartsGameWpf.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace HeartsGameWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Game game = new Game();
            Rules rules = new Rules(game);
            GameManager gameManager = new GameManager(game, rules);
            MainWindow mw = new MainWindow();
            mw.MainViewModel = new MainViewModel(gameManager);
            mw.Show();
        }
    }
}
