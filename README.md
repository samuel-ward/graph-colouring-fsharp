Graph-Clouring In F#
=========================

## Overview

This project has been conducted in accordance with Massey University directive while undergoing course 159.333 Programming Project.

This project is designed to explore different graph-colouring algorithms and their implementation using F# on the .NET stack.
It is a .NET Core 3.1 application.

## Requirements

- [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1).
- [VSCode](https://code.visualstudio.com/download)
- [Ionide extension for VSCode](https://marketplace.visualstudio.com/items?itemName=Ionide.Ionide-fsharp)

## The Problem

The Graph Colouring problem is what is known as an _NP Complete_ problem.

## Algorithms

### Random Colouring Algorithm

The Random Colouring Algorithm shuffles all indices, using the Fisher-Yates shuffle algorithm in this case, to randomise the order in which the vertices are processed.

#### Psuedo Code

```
1. Randomly select first vertex and colour it with the lowest chromatic index
2. For each remaining vertex (V-1) do the following:
    a. Choose a vertex at random
    b. Consider the lowest available chromatic index that is not present
        on an adjacent vertex
    c. If all adjacent vertices assign a new chromatic index
```

### Greedy Algorithm

The Greedy Colouring Algorithm doesn't guarantee to use a the minimum number of colours, but it does gaurantee an upper bound on the number of colours - being `d+1` where `d` is the maximum degree of a vertex.

#### Psuedo Code

```
1. Colour the first veretex with the lowest chromatic index
2. For each remaining vertex (V-1) do the following:
    a. Consider current vertex and assign the lowest chromatic index that hasn't been used on a previously coloured vertex adjacent to it.
    b. If all used colours appear on adjacent vertices, assign a new chromatic index
```

## Running The Application

To run the application normally:

```
dotnet clean
dotnet build
dotnet run
```

To run the application in the debugger:

- Open VSCode
- Set breakpoints
- Open Debug view
- Select __Run > Start Debugging__ (or pressing <kbd>F5</kbd>)

### Data

The data is set to be taken from `./Data/anonymised.csv` at build time.

There is a `TODO:` in code to implement this as a setting rather than hardcoded.

The data is currently required to follow the proceeding format from the CSV file:

```
id, exam1, exam2, exam3, exam4,
```

Any empty exam slots should be indicated by a following comma and don't need to be sequential e.g. `00, AA,, AB,,`

>_NOTE:_ Whitespaces are trimmed during the csv deserialsation.

>_NOTE:_ For formatting of the timetable output at the end it is recommended to stick to 2 character representations of the exams

### Settings

The settings are available in `./settings.json` and are deserialised when the application runs.

They consist of the following:

```json
{
    "algorithm": "all | greedy | random",
    "rooms": 10
}
```

>_NOTE:_ For formatting of the timetable output at the end it is recommended to stick to a room limit lower than `100`.

> _NOTE:_ The settings get deserialised as option types, this means that if they are undefined in the JSON file the application will use defaults instead

> _NOTE:_ The only algorithms available are `"greedy", "random"`, more were planned to be added.

#### Settings To Be Implemented

The following are settings that have not yet been implemented because of time constraints:

- "data": data location is hardcoded
- "timeslots": there is currently no restriction on timeslots i.e. number of columns in output timetable

Desired settings:

```json
{
    "algorithm": "all | random | greedy | k-start | lazy-dfs",
    "data": "/location/to/data/file.csv",
    "rooms": 10,
    "timeslots": 10
}
```