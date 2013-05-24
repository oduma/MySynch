xcopy ..\MySynch.Publisher\Assemblies\*.exe ..\MySynch.Publisher\Exe\*.* /Y
del ..\MySynch.Publisher\Assemblies\*.exe
xcopy ..\MySynch.Subscriber\Assemblies\*.exe ..\MySynch.Subscriber\Exe\*.* /Y
del ..\MySynch.Subscriber\Assemblies\*.exe
xcopy ..\MySynch.Broker\Assemblies\*.exe ..\MySynch.Broker\Exe\*.* /Y
del ..\MySynch.Broker\Assemblies\*.exe

xcopy ..\MySynch.Publisher\Assemblies\*.exe.config ..\MySynch.Publisher\Config\*.* /Y
del ..\MySynch.Publisher\Assemblies\*.exe.config
xcopy ..\MySynch.Subscriber\Assemblies\*.exe.config ..\MySynch.Subscriber\Config\*.* /Y
del ..\MySynch.Subscriber\Assemblies\*.exe.config
xcopy ..\MySynch.Broker\Assemblies\*.exe.config ..\MySynch.Broker\Config\*.* /Y
del ..\MySynch.Broker\Assemblies\*.exe.config

heat dir ..\MySynch.Publisher\Assemblies -srd -dr PUBLISHER -cg PublisherGroup -var var.PublisherSource -gg -out PublisherGroup.wxs
heat dir ..\MySynch.Subscriber\Assemblies -srd -dr SUBSCRIBER -cg SubscriberGroup -var var.SubscriberSource -gg -out SubscriberGroup.wxs
heat dir ..\MySynch.Broker\Assemblies -srd -dr BROKER -cg BrokerGroup -var var.BrokerSource -gg -out BrokerGroup.wxs
