#!/bin/bash

if [ "$1" == "" ]; then
    echo "Usage: $(basename $0) ANGLE"
    exit 1
fi

readonly angle="$1"
readonly angleNNN=$(printf "%03d" $angle)
readonly pfmfile=image$angleNNN.pfm
readonly pngfile=image$angleNNN.png

time ./bin/Debug/net6.0/RTXon demo --renderer flat --angle-deg $angle \
    --width 640 --height 480 --pfm-output perspective-color/$pfmfile \
    && ./bin/Debug/net6.0/RTXon pfm2png --luminosity 0.5 perspective-color/$pfmfile perspective-color/$pngfile