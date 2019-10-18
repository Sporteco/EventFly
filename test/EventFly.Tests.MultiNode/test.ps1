rmdir .\EventFly.Tests.MultiNode.AggregateClusterTests\ -r
rmdir .\EventFly.Tests.MultiNode.AggregateSagaClusterTests\ -r
rmdir .\bin\ -r
rmdir .\obj\ -r


dotnet build "EventFly.Tests.MultiNode.csproj" --configuration Release

Copy-Item .\bin\Release\netcoreapp2.2\Newtonsoft.Json.dll .\tools\win7-x64\Newtonsoft.Json.dll -force

dotnet ".\tools\win7-x64\Akka.MultiNodeTestRunner.dll" ".\bin\Release\netcoreapp2.2\EventFly.Tests.MultiNode.dll" "-Dmultinode.platform=netcore"