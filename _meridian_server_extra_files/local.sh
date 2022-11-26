echo "Performing local operations first"

echo "Copying server for git..."
cp -Rf /mnt/c/M59420/Classic/run/server/* /mnt/c/M59420/Blakserv/run/server/

#echo "Copying BOF files to loadkod..."
#find /mnt/c/M59420/Classic/kod/ -name '*.bof' -print0 | xargs -0 cp -t /mnt/c/M59420/Classic/run/server/loadkod/

#echo "Copying RSC files to rsc..."
#find /mnt/c/M59420/Classic/kod/ -name '*.rsc' -print0 | xargs -0 cp -t /mnt/c/M59420/Classic/run/server/rsc/

echo "Finished local update..."

