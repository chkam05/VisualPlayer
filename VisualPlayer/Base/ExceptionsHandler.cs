using chkam05.InternalMessages;
using chkam05.VisualPlayer.Base.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Base
{
    public class ExceptionsHandler
    {

        //  EVENTS

        public event EventHandler<HandleExceptionEventArgs> OnExceptionHandle;


        //  VARIABLES

        private static ExceptionsHandler _instance;

        private string _applicationName = "";
        private Stack<Exception> _exceptions;
        private InternalMessagesContainer _internalMessages;


        #region GETTERS & SETTERS

        public int ExceptionsInQueue
        {
            get => _exceptions.Count;
        }

        public bool IsInitialized
        {
            get => _internalMessages != null;
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> PlayerCore class constructor. </summary>
        /// <param name="applicationName"> Name of application. </param>
        private ExceptionsHandler(string applicationName)
        {
            _applicationName = applicationName;
            _exceptions = new Stack<Exception>();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create instance of PlayerCore class. </summary>
        /// <param name="applicationName"> Name of application. </param>
        public static ExceptionsHandler Create(string applicationName)
        {
            _instance = new ExceptionsHandler(applicationName);
            return _instance;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get or create instance of PlayerCore class. </summary>
        public static ExceptionsHandler Instance
        {
            get
            {
                return _instance ?? null;
            }
        }

        #endregion CLASS METHODS

        #region EXCEPTIONS MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Handle exception by passing it to internal message and external method. </summary>
        /// <param name="exception"> Raised exception. </param>
        private void HandleException(Exception exception)
        {
            var internalMessage = _internalMessages.ShowError(_applicationName, exception.Message, false);
            var eventArgs = new HandleExceptionEventArgs(exception, internalMessage);
            OnExceptionHandle?.Invoke(this, eventArgs);
        }

        #endregion EXCEPTIONS MANAGEMENT METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get next exception from queue and show it. </summary>
        public void HandleNextException()
        {
            if (IsInitialized && ExceptionsInQueue > 0)
                HandleException(_exceptions.Pop());
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Show all exceptions from queue. </summary>
        public void HandleStack()
        {
            if (IsInitialized)
                while (ExceptionsInQueue > 0)
                    HandleException(_exceptions.Pop());
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Report new exception to handle. </summary>
        /// <param name="exception"> Newly raised exception. </param>
        public void ReportException(Exception exception)
        {
            if (!IsInitialized)
                _exceptions.Push(exception);
            else
                HandleException(exception);
        }

        #endregion INTERACTION METHODS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Initialize interface integration. </summary>
        /// <param name="internalMessages"> Internal messages container. </param>
        public void InitializeInterface(InternalMessagesContainer internalMessages)
        {
            _internalMessages = internalMessages;
        }

        #endregion SETUP METHODS

        

    }
}
