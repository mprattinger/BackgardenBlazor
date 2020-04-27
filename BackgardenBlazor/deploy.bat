dotnet publish -r linux-arm -o D:\Backgarden\Blazor
pushd D:\Backgarden\Blazor
pscp -pw Fl61291420  -v -r .\* pi@backgarden:/var/www/backgarden
popd