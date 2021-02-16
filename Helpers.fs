module GraphColouring.Helpers

open GraphColouring.TypeProvider
open Newtonsoft.Json
open System
open System.IO

let printJson (a: 'a) =
    (* Generic function to serialize into JSON and write to console *)
    a
    |> JsonConvert.SerializeObject
    |> Console.WriteLine

let private _getItemAtDepth depth (a: string list) =
    match List.tryItem depth a with
    | None -> "          |"
    | Some item -> sprintf "    %s    |" item

let private _printBorder char columns =
    char |> String.replicate (columns * 11 + 1) |> Console.WriteLine

let private _printHeader columns =
    _printBorder "-" (Map.toList columns |> List.length)
    Console.Write "|"
    Map.iter
        (fun k _ -> sprintf " Slot: %02i |" (k+1) |> Console.Write)
        columns
    Console.WriteLine ""
    _printBorder "-" (Map.toList columns |> List.length)

let printTimetable (timetable: Timetable) =
    Console.WriteLine "---- Printing Timetable ----"
    _printHeader timetable.Timeslots
    let maxDepth = 
        (* Find number of exams in each timeslot *)
        Map.map (fun k v -> List.length v) timetable.Timeslots
        |> Map.toList
        (* Find timeslot with the most exams *)
        |> List.maxBy (fun (_, v) -> v)
        (* Return the length *)
        |> fun (_, v) -> v
    List.init
        maxDepth
        id
    |> List.iter
        (fun i ->
            Console.Write "|"
            Map.iter
                (fun _ v -> _getItemAtDepth i v |> Console.Write)
                timetable.Timeslots
            Console.WriteLine ""
            _printBorder "-" (Map.toList timetable.Timeslots |> List.length)
        )

let private _toSettings (set: SettingsBase) =
    {
        Settings.Algorithm = 
            match set.Algorithm with
            | Some s when String.Equals(s, "greedy") -> Algorithm.Greedy
            | None | _ -> Algorithm.Greedy
        Room = set.Room
    }

let loadSettings =
    File.ReadAllText "./settings.json"
    |> JsonConvert.DeserializeObject<SettingsBase>
    |> _toSettings