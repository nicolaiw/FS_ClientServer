!/bin/bash
if test "$OS" = "Windows_NT"
then
  # use .Net
  packages/FAKE/tools/FAKE.exe build.fsx 
else
  # use mono
  mono packages/FAKE/tools/FAKE.exe build.fsx 
fi