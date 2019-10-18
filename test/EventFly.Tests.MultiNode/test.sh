#!/bin/sh

#Clean
rm -rf ./EventFly.Tests.MultiNode.AggregateClusterTests/
rm -rf ./EventFly.Tests.MultiNode.AggregateSagaClusterTests/
rm -rf ./bin/
rm -rf ./obj/

dotnet build EventFly.Tests.MultiNode.csproj --configuration Release
dotnet build ../EventFly.NodeTestRunner/EventFly.NodeTestRunner.csproj --configuration Release --runtime osx-x64
dotnet build ../EventFly.MultiNodeTestRunner/EventFly.MultiNodeTestRunner.csproj --configuration Release --runtime osx-x64
/bin/cp -rf ./bin/Release/netcoreapp2.2/Newtonsoft.Json.dll ../EventFly.MultiNodeTestRunner/bin/Release/netcoreapp2.2/osx-x64/Newtonsoft.Json.dll
#dotnet ../EventFly.MultiNodeTestRunner/bin/Release/netcoreapp2.2/osx-x64/Akka.MultiNodeTestRunner.dll ./bin/Release/netcoreapp2.2/EventFly.Tests.MultiNode.dll -Dmultinode.platform=netcore
coverlet '../EventFly.MultiNodeTestRunner/bin/Release/netcoreapp2.2/osx-x64/Akka.MultiNodeTestRunner.dll' --target 'dotnet' --targetargs '../EventFly.MultiNodeTestRunner/bin/Release/netcoreapp2.2/osx-x64/Akka.MultiNodeTestRunner.dll ./bin/Release/netcoreapp2.2/EventFly.Tests.MultiNode.dll -Dmultinode.platform=netcore -Dmultinode.output-directory=/Users/lutandongqakaza/Workspace/EventFly/EventFly/multinodelogs' --format 'opencover' --include "[EventFly]" --include "[EventFly.Clustering]" --exclude "[xunit*]*" --exclude "[Akka.NodeTestRunner*]*" --exclude "[EventFly.NodeTestRunner*]*" --verbosity detailed --output '/Users/lutandongqakaza/Workspace/EventFly/EventFly/coverageresults/multinode.opencover.xml'




/Users/lutandongqakaza/Workspace/EventFly/EventFly/build/tools/coverlet '/Users/lutandongqakaza/Workspace/EventFly/EventFly/test/EventFly.MultiNodeTestRunner/bin/Release/netcoreapp2.2/osx-x64/Akka.MultiNodeTestRunner.dll' --target 'dotnet' --targetargs '/Users/lutandongqakaza/Workspace/EventFly/EventFly/test/EventFly.MultiNodeTestRunner/bin/Release/netcoreapp2.2/osx-x64/Akka.MultiNodeTestRunner.dll /Users/lutandongqakaza/Workspace/EventFly/EventFly/test/EventFly.Tests.MultiNode/bin/Release/netcoreapp2.2/EventFly.Tests.MultiNode.dll -Dmultinode.platform=netcore -Dmultinode.output-directory=/Users/lutandongqakaza/Workspace/EventFly/EventFly/multinodelogs' --format 'opencover' --include "[EventFly]" --include "[EventFly.Clustering]" --exclude "[xunit*]*" --exclude "[Akka.NodeTestRunner*]*" --exclude "[EventFly.NodeTestRunner*]*" --verbosity detailed --output '/Users/lutandongqakaza/Workspace/EventFly/EventFly/coverageresults/multinode.opencover.xml'