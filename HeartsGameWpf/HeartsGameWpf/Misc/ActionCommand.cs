using HeartsGameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HeartsGameWpf.Misc
{
    public class ActionCommand<T> : ICommand
    {
        private readonly Action<T> action;
        private readonly Func<T, bool> validation;

        public ActionCommand(Action<T> action, Func<T, bool> validation = null)
        {
            this.action = action;
            this.validation = validation;
        }
        
        public bool CanExecute(object parameter)
        {
            if(validation == null)
                return true;
            
            T t = GetParameter(parameter);

            return validation(t);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            T t = GetParameter(parameter);
            
            action(t);
        }

        private T GetParameter(object parameter)
        {
            T t = default(T);

            if (parameter != null)
            {
                Type type = parameter.GetType();

                if (!(parameter is T))
                    throw new ArgumentException("parameter is of type " + type.ToString() + ". It must be of type " + typeof(T).ToString());

                t = (T)parameter;
            }

            return t;
        }
    }
}
