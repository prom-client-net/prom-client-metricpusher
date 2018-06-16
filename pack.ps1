dotnet pack $env:APPVEYOR_BUILD_FOLDER\src\Prometheus.Client.HttpRequestDurations -c Release --include-symbols --no-build -o artifacts\nuget
