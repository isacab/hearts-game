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
        public SaveHandler(string fileName, Game game)
        {
            if (fileName == null || game == null)
                throw new ArgumentNullException();

            FileName = fileName;
            Game = game;
        }

        public string FileName { get; set; }

        public Game Game { get; set; }

        public void Save()
        {
            try
            {
                XDocument doc = new XDocument();
                doc.Add(Game.GenerateXElement());
                doc.Save(FileName);
            }
            catch(Exception e)
            {
                if (e is IOException)
                    throw;
                else
                    throw new SaveHandlerException("Game is invalid and could not be saved.", e);
            }
        }

        public void Load()
        {
            try
            {
                XDocument doc = XDocument.Load(FileName);
                XElement gameEl = doc.Element("Game");
                Game.Load(gameEl);
            }
            catch (Exception e)
            {
                if(e is IOException)
                    throw;
                else
                    throw new SaveHandlerException("Could not load the game. Is the savefile in an invalid format?", e);
            }
        }
    }
}
