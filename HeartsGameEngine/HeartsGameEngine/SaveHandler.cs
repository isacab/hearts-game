using HeartsGameEngine.DataObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace HeartsGameEngine
{
    public class SaveHandlerException : Exception
    {
        public SaveHandlerException() {}

        public SaveHandlerException(string message)
            : base(message) {}

        public SaveHandlerException(string message, Exception inner)
            : base(message, inner) {}
    }

    class SaveHandler
    {
        public const string FileName = "save.xml";
        private FileSystemWatcher watcher = new FileSystemWatcher();

        public SaveHandler()
        {
            watcher.Path = Directory.GetCurrentDirectory();
            watcher.Filter = FileName;
            watcher.Changed += Watcher_Changed; 
        }

        public event EventHandler<EventArgs> AutoLoaded;

        private Game game;
        public Game Game
        {
            get { return game; }
            set { game = value; }
        }

        private bool enableWatcher = false;
        public bool EnableWatcher
        {
            get 
            {
                return enableWatcher; 
            }
            set 
            {
                enableWatcher = value;
                watcher.EnableRaisingEvents = value; 
            }
        }

        public bool SaveFileExists()
        {
            return File.Exists(FileName);
        }

        public void Save()
        {
            if (game == null)
                return;

            watcher.EnableRaisingEvents = false;

            try
            {
                XDocument doc = new XDocument();
                doc.Add(game.GenerateXElement());
                doc.Save(FileName);
            }
            catch(Exception e)
            {
                if (e is IOException)
                    throw;
                else
                    throw new SaveHandlerException("Game is invalid and could not be saved.", e);
            }
            finally
            {
                watcher.EnableRaisingEvents = enableWatcher;
            }
        }

        public void Load()
        {
            if (game == null)
                return;

            watcher.EnableRaisingEvents = false;

            try
            {
                XDocument doc = XDocument.Load(FileName);
                XElement gameEl = doc.Element("Game");
                game.Load(gameEl);
            }
            catch (Exception e)
            {
                if(e is IOException)
                    throw;
                else
                    throw new SaveHandlerException("Could not load the game. Is the savefile in an invalid format?", e);
            }
            finally
            {
                watcher.EnableRaisingEvents = enableWatcher;
            }
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                System.Threading.Thread.Sleep(1000);

                Load();

                if (AutoLoaded != null)
                    AutoLoaded(this, EventArgs.Empty);
            }
        }
    }
}
