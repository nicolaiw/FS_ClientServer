#!/bin/bash

if test "$OS" = "Windows_NT"
then
  # use .Net

  if [ ! -f packages/FAKE/tools/FAKE.exe ]; then
    .nuget/NuGet.exe install FAKE -OutputDirectory packages -ExcludeVersion
  fi

  packages/FAKE/tools/FAKE.exe build.fsx 
else
  # use mono

  if [ ! -f packages/FAKE/tools/FAKE.exe ]; then
    mono .nuget/NuGet.exe install FAKE -OutputDirectory packages -ExcludeVersion
  fi

  mono packages/FAKE/tools/FAKE.exe build.fsx 
fi