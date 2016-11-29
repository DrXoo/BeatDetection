# Beat Detection module for Unity 3D
Okay first things first:

1. I'm not an English speaker so forgive me if i write something wrong.
2. Read [**this**](http://archive.gamedev.net/archive/reference/programming/features/beatdetection/index.html) first 

There is _TWO_ modules in here:
* Simple
* With Subbands

I'll explain to you what it does in both cases

### Simple Beat Detection module
Scripts:
* _SimpleBeatDetection_
* _ExampleUse_

Simple Beat Detection computes the instant energy of the song each frame. Then it calculates the average instant energy and a variance energy which later we can ask if there is a beat at that time or not. 

Easy to say but quite hard to understand at first. The code is commented and it is not very long.

### Subbands (kind of frequency selected) Beat Detection module
Scripts:
* _SubbandBeatDetection_
* _CubesManager_
* _CubeSound_

In the Simple Beat Detection we compute the instant energy of all the samples array. Instead of that, we are going to divide that array in small subbands. Each subband does what the simple beat detection do.

The number of subbands is up to us. 64 Subbands is a nice number because it provides us with a good precision.

CubesManager creates several cubeSounds. Each on of them will subscribe to several subbands.
