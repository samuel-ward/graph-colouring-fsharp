module GraphColouring.Program

open GraphColouring.Algorithms
open GraphColouring.CsvHelper
open GraphColouring.Graph
open GraphColouring.Helpers
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

let rec run i =
    Console.WriteLine (sprintf "%A" i)
    run i

let private _test settings =
    (* Test functions *)
    let students = getStudentData ()
    //students |> printJson

    let exams = getExamData ()
    //exams |> printJson

    let constraintGraph = createExamConstraintGraph settings (getExamData ())
    //constraintGraph |> printJson
    constraintGraph |> printGraph

    let greedyColoured = Greedy.colourGraph settings constraintGraph
    //greedyColoured |> printJson
    //greedyColoured |> printGraph
    greedyColoured |> printGraphColouring
    greedyColoured |> Greedy.printTimetable

    ()

[<EntryPoint>]
let main _ =
    _setJsonSettings
    let settings = loadSettings
    _test settings
    0