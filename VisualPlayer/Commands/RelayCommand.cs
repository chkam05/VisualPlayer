using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VisualPlayer.Commands
{
    public class RelayCommand : ICommand
    {

        //  EVENTS

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }


        //  VARIABLES

        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> RelayCommand class constructor. </summary>
        /// <param name="execute"> Action to execute. </param>
        /// <param name="canExecute"> Can execute delegate. </param>
        /// <exception cref="ArgumentNullException"></exception>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        #endregion CLASS METHODS

        #region EXECUTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Check if action can be executed. </summary>
        /// <param name="parameter"> Object to evaluate by delegate. </param>
        /// <returns> True - can be executed; False - otherwise. </returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Execute action. </summary>
        /// <param name="parameter"> Execution parameter. </param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        #endregion EXECUTION METHODS

    }
}
