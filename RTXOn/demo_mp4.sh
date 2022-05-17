#!/bin/bash
# 1. Create ProgressBar function
# 1.1 Input is currentState($1) and totalState($2)
function ProgressBar {
# Process data
    let _progress=(${1}*100/${2}*100)/100
    let _done=(${_progress}*4)/10
    let _left=40-$_done
# Build progressbar string lengths
    _fill=$(printf "%${_done}s")
    _empty=$(printf "%${_left}s")

# 1.2 Build progressbar strings and print the ProgressBar line
# 1.2.1 Output example:                           
# 1.2.1.1 Completed [########################################] 100%
printf "\rCompleted [${_fill// /#}${_empty// /-}] ${_progress}%%"

}

orthogonal_flag=""

if [ -z $1 ]
then
  camera="perspective"
else
  camera="$1"
  if [ "$camera" = "orthogonal" ]
  then
    orthogonal_flag="--orthogonal"
  fi
fi

if [ ! -d $camera ]
then
  mkdir $camera
fi

for angle in $(seq 0 359); do
    # Angle with three digits, e.g. angle="1" → angleNNN="001"
    angleNNN=$(printf "%03d" $angle)
    ./bin/Debug/net6.0/RTXOn demo --width=640 --height=480 $orthogonal_flag --angle-deg $angle --output=$camera/img$angleNNN.png
    ProgressBar $angle 359
done

printf '\n'

# -r 25: Number of frames per second
ffmpeg -r 25 -f image2 -s 640x480 -i $camera/img%03d.png \
    -vcodec libx264 -pix_fmt yuv420p \
    spheres-$camera.mp4