module GraphColouring.Program

open GraphColouring.Algorithms
open GraphColouring.CsvHelper
open GraphColouring.Graph
open GraphColouring.Helpers
open GraphColouring.TypeProvider
open Newtonsoft.Json
open System
open System.Collections.Generic

let private _setJsonSettings =
    (* Set JSON serializer with F# type converters *)
    JsonConvert.DefaultSettings <-
        fun () ->
            let settings = JsonSerializerSettings()
            settings.Formatting <- Formatting.Indented
            settings.ContractResolver <- Serialization.CamelCasePropertyNamesContractResolver()
            settings.Converters <- [
                FifteenBelow.Json.OptionConverter () :> JsonConverter
                FifteenBelow.Json.UnionConverter () :> JsonConverter
                FifteenBelow.Json.MapConverter () :> JsonConverter
            ] |> List.toArray :> IList<JsonConverter>
            settings.NullValueHandling <- NullValueHandling.Ignore
            settings

let private _test settings =
    (* Test functions *)
    let students = getStudentData ()
    students |> printJson

    let exams = getExamData ()
    exams |> printJson

    let constraintGraph = createExamConstraintGraph settings (getExamData ())
    //constraintGraph |> printJson
    constraintGraph |> printGraph

    let greedyColoured = Greedy.colourGraph settings constraintGraph
    //greedyColoured |> printJson
    greedyColoured |> printGraph
    //greedyColoured |> printGraphColouring
    //greedyColoured |> Greedy.printTimetable

    ()

let rec private _run (settings: Settings) =
    match settings.Algorithm with
    | Greedy ->
        let startTime = System.DateTime.UtcNow
        let graph = createExamConstraintGraph settings (getExamData ()) |> Greedy.colourGraph settings
        graph |> Greedy.printTimetable
        let endTime = System.DateTime.UtcNow
        let ellapsed = endTime-startTime
        Console.WriteLine "---- Ellapsed Time ----"
        sprintf "%fms" ellapsed.TotalMilliseconds
        |> Console.WriteLine
        Console.WriteLine "---- - ----"
        (* Uncomment the following line to make the application run continuously *)
        //_run settings

[<EntryPoint>]
let main _ =
    _setJsonSettings
    let settings = loadSettings
    (* Uncomment the following line to run the test functions *)
    //_test settings
    _run settings
    0