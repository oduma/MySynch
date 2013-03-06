
xcopy ..\MySynch.Publisher\Assemblies\*.exe.config ..\MySynch.Publisher\Config\*.* /Y
del ..\MySynch.Publisher\Assemblies\*.exe.config
xcopy ..\MySynch.Subscriber\Assemblies\*.exe.config ..\MySynch.Subscriber\Config\*.* /Y
del ..\MySynch.Subscriber\Assemblies\*.exe.config
xcopy ..\MySynch.Distributor\Assemblies\*.exe.config ..\MySynch.Distributor\Config\*.* /Y
del ..\MySynch.Distributor\Assemblies\*.exe.config

heat dir ..\MySynch.Publisher\Assemblies -srd -dr PUBLISHER -cg PublisherGroup -var var.PublisherSource -gg -out PublisherGroup.wxs
heat dir ..\MySynch.Subscriber\Assemblies -srd -dr SUBSCRIBER -cg SubscriberGroup -var var.SubscriberSource -gg -out SubscriberGroup.wxs
heat dir ..\MySynch.Distributor\Assemblies -srd -dr DISTRIBUTOR -cg DistributorGroup -var var.DistributorSource -gg -out DistributorGroup.wxs