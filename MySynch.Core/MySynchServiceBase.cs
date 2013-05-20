﻿using System;
using System.Timers;
using MySynch.Common;
using MySynch.Common.Logging;
using MySynch.Contracts.Messages;
using MySynch.Core.DataTypes;
using MySynch.Core.Interfaces;
using MySynch.Proxies.Autogenerated.Implementations;

namespace MySynch.Core
{
    public abstract class MySynchServiceBase:WcfHostServiceBase
    {
        protected Timer Timer;
        protected BrokerClient BrokerClient;
        protected AttachRequest CurrentAttachRequest;
        protected string HostUrl;
        protected LocalComponentConfig LocalComponentConfig;
        protected ILocalComponent LocalComponent;

        public MySynchServiceBase()
        {
            LocalComponentConfig = ConfigurationHelper.ReadLocalComponentConfiguration();
        }

        public abstract bool InitializeLocalComponent();


        protected virtual void TimerElapseMethod(object sender, ElapsedEventArgs e)
        {
            LoggingManager.Debug("Timer kicked in again.");
            Timer.Enabled = false;
            if (TryMakeComponentKnown(LocalComponentConfig.BrokerName))
            {
                Timer.Interval = 120000;
                Timer.Enabled = true;
                return;
            }
            Timer.Interval = 60000;
            Timer.Enabled = true;
            LoggingManager.Debug("Starting timer again.");

        }

        protected bool TryMakeComponentKnown(string brokerName)
        {
            try
            {
                if (TryToOpenBroker(brokerName))
                {
                    LoggingManager.Debug("Trying to attach to the broker: " + brokerName);
                    BrokerClient.Attach(CurrentAttachRequest);
                    LoggingManager.Debug("Attached to broker: " + brokerName);
                    LocalComponent.Initialize(BrokerClient,LocalComponentConfig, HostUrl);
                    return true;
                }
                LoggingManager.Debug("Not attached to broker: " + brokerName);
                return false;
            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
                LoggingManager.Debug("Not attached to broker: " + brokerName);
                return false;
            }
        }

        protected void DetachFromBroker()
        {
            try
            {
                DetachRequest request = new DetachRequest { ServiceUrl = HostUrl };
                if (BrokerClient.Detach(request).Status)
                {
                    LoggingManager.Debug("Component detached from the broker");
                    return;
                }
                LoggingManager.Debug("Component will stop but still attached to the broker");

            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
            }
        }

        protected internal bool TryToOpenBroker(string brokerName)
        {
            try
            {
                LoggingManager.Debug("Trying to open broker: " + brokerName);
                if (BrokerClient == null)
                {
                    BrokerClient = new BrokerClient();
                    BrokerClient.InitiateUsingServerAddress(string.Format("http://{0}/broker", brokerName));
                }
                return BrokerClient.GetHeartbeat().Status;

            }
            catch (Exception ex)
            {
                BrokerClient = null;
                LoggingManager.LogMySynchSystemError(ex);
                return false;
            }
        }



        protected void StartTimer(double timerInterval, ElapsedEventHandler timerElapsedMethod)
        {
            Timer = new Timer();
            Timer.Interval = timerInterval;
            Timer.Elapsed += timerElapsedMethod;
            Timer.Enabled = true;
            Timer.Start();

        }
    }
}
