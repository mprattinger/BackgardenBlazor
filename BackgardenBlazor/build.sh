#!/bin/sh

rm -rf /mnt/d/Backgarden/Blazor
dotnet publish -r linux-arm -c Release -o /mnt/d/Backgarden/Blazor
pushd /mnt/d/Backgarden/Blazor
scp -rp ./* pi@backgarden:/var/www/backgarden
popd