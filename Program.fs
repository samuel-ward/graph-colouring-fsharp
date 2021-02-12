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
    //greedyColoured |> printGraph
    //greedyColoured |> printGraphColouring
    greedyColoured |> Graph.printTimetable

    let randomColoured = RandomColour.colourGraph settings constraintGraph
    //randomColoured |> printJson
    //randomColoured |> printGraph
    //randomColoured |> printGraphColouring
    randomColoured |> Graph.printTimetable

    ()

let rec private _run (settings: Settings) =
    let startTime = System.DateTime.UtcNow
    match settings.Algorithm with
    | Greedy -> createExamConstraintGraph settings (getExamData ()) |> Greedy.colourGraph settings
    | Random -> createExamConstraintGraph settings (getExamData ()) |> RandomColour.colourGraph settings
    |> fun g ->
        g |> Graph.printTimetable
        let ellapsed = (System.DateTime.UtcNow)-startTime
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