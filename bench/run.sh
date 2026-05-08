#!/usr/bin/env bash
# Args: m n strategy(d|b) [todos: s|n]
M=$1; N=$2; S=$3; T=${4:-n}
if [ "$S" = "b" ]; then
  INPUT=$(printf "1\n%s\n%s\nb\n0\n" "$M" "$N")
else
  INPUT=$(printf "1\n%s\n%s\nd\n%s\n0\n" "$M" "$N" "$T")
fi
echo "$INPUT" | timeout "${TIMEOUT:-300}" dotnet /workspaces/dotnet/EpPentaminos/bin/Release/net10.0/EpPentaminos.dll
