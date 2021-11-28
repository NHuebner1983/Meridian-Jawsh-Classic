echo "Performing local operations first"

echo "Copying BOF files to loadkod..."
find /mnt/c/M59420/Classic/kod/ -name '*.bof' -print0 | xargs -0 cp -t /mnt/c/M59420/Classic/run/server/loadkod/

echo "Copying RSC files to rsc..."
find /mnt/c/M59420/Classic/kod/ -name '*.rsc' -print0 | xargs -0 cp -t /mnt/c/M59420/Classic/run/server/rsc/

echo "Synchronizing loadkod on public game server..."
sshpass -f /secure-server.us.pass rsync --size-only /mnt/c/M59420/Classic/run/server/loadkod/* root@secure-server.us:/meridian/run/server/loadkod/

echo "Synchronizing rsc on public game server..."
sshpass -f /secure-server.us.pass rsync --size-only /mnt/c/M59420/Classic/run/server/rsc/* root@secure-server.us:/meridian/run/server/rsc/

echo "Synchronizing KODBase"
sshpass -f /secure-server.us.pass rsync -v /mnt/c/M59420/Classic/kod/* root@secure-server.us:/meridian/kod/
