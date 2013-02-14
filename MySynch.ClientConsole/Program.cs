using System;
using MySynch.Contracts.Messages;
using MySynch.Core;
using MySynch.Core.DataTypes;
using MySynch.Core.Interfaces;
using MySynch.Proxies;

namespace MySynch.ClientConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //IPublisherProxy publisherProxy = new PublisherClient();
            //publisherProxy.InitiateUsingEndpoint("PublisherSciendoLaptop");
            //publisherProxy.PublishPackage();

            ISourceOfDataProxy sourceOfDataProxy = new SourceOfDataClient();
            sourceOfDataProxy.InitiateUsingEndpoint("SourceOfDataSciendoLaptop");
            //var data = sourceOfDataProxy.GetData(new RemoteRequest {FileName = @"C:\MySynch.Source.Test.Root\img001.jpg"});

            ISubscriberProxy subscriberProxy = new SubscriberClient();
            subscriberProxy.InitiateUsingEndpoint("SubscriberSciendoLaptop");
            CopyStrategy copyStrategy= new CopyStrategy();
            copyStrategy.Initialize(sourceOfDataProxy);
            subscriberProxy.ApplyChangePackage(new ChangePushPackage(), copyStrategy.Copy);
        }
    }
}
