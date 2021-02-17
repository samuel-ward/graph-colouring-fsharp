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
    match settings.Algorithm with
    | All ->
        [
            GraphType.Random,
                System.DateTime.UtcNow,
                createExamConstraintGraph settings (getExamData ()) |> RandomColour.colourGraph settings,
                System.DateTime.UtcNow
            GraphType.Greedy,
                System.DateTime.UtcNow, 
                createExamConstraintGraph settings (getExamData ()) |> Greedy.colourGraph settings,
                System.DateTime.UtcNow
        ]
    | Random -> 
        [
            GraphType.Random,
                System.DateTime.UtcNow, 
                createExamConstraintGraph settings (getExamData ()) |> RandomColour.colourGraph settings,
                System.DateTime.UtcNow
        ]
    | Greedy -> 
        [
            GraphType.Greedy,
                System.DateTime.UtcNow,
                createExamConstraintGraph settings (getExamData ()) |> Greedy.colourGraph settings,
                System.DateTime.UtcNow
        ]
    |> List.iter
        (fun ((gt: GraphType), (st: DateTime), (g: Hekate.Graph<string, int, string>), (et: DateTime)) ->
            (* Print graph type header *)
            match gt with
            | GraphType.Constraint -> "Constraint Graph"
            | GraphType.Random -> "Random Colouring"
            | GraphType.Greedy -> "Greedy Colouring"
            |> sprintf "---- %s ----"
            |> Console.WriteLine

            (* Print graph timetable *)
            g |> Graph.printTimetable

            (* Print time elapsed - NOTE: the order may skew times because of initialisation *)
            Console.WriteLine "---- Ellapsed Time ----"
            sprintf "%fms" (et-st).TotalMilliseconds
            |> Console.WriteLine

            Console.WriteLine "---- - ----"

        )
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