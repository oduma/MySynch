using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace MySynch.Monitor.MVVM.ViewModels
{
    internal class AsynchViewModelBase:ViewModelBase
    {

        private bool _uiAvailable = true;
        public bool UIAvailable
        {
            get
            {
                return this._uiAvailable;
            }

            set
            {
                if (value != _uiAvailable)
                {
                    _uiAvailable = value;
                    RaisePropertyChanged("UIAvailable");
                }
            }
        }


        private Visibility _messageVisible = Visibility.Hidden;
        public Visibility MessageVisible
        {
            get
            {
                return this._messageVisible;
            }

            set
            {
                if (value != _messageVisible)
                {
                    _messageVisible = value;
                    RaisePropertyChanged("MessageVisible");
                }
            }
        }


        protected void UnblockTheUI()
        {
            UIAvailable = true;
            MessageVisible = Visibility.Hidden;
        }

        protected void BlockTheUI(EventHandler<ProgressChangedEventArgs> doSaveWorkProgressChanged)
        {
            doSaveWorkProgressChanged(this, new ProgressChangedEventArgs(0, "Stopping the UI."));
            UIAvailable = false;
            MessageVisible = Visibility.Visible;
        }


        private string _workingMessage;
        public string WorkingMessage
        {
            get
            {
                return this._workingMessage;
            }

            set
            {
                if (value != _workingMessage)
                {
                    _workingMessage = value;
                    RaisePropertyChanged("WorkingMessage");
                }
            }
        }



    }
}
