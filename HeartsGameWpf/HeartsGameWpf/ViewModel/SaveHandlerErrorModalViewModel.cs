using HeartsGameEngine;
using HeartsGameWpf.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HeartsGameWpf.ViewModel
{
    public class SaveHandlerErrorModalViewModel : BaseViewModel
    {
        public SaveHandlerErrorModalViewModel()
        {
        }

        private string title = "";
        public string Title
        {
            get { return title; }
            set { SetValue(ref title, value); }
        }

        private string message = "";
        public string Message
        {
            get { return message; }
            set { SetValue(ref message, value); }
        }
    }
}
