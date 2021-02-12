module GraphColouring.Program

open GraphColouring.CsvHelper
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

let private _test () =
    (* Test functions *)
    let students = getStudentData ()
    //students |> printJson

    let exams = getExamData ()
    //exams |> printJson

    ()

[<EntryPoint>]
let main argv =
    _setJsonSettings
    _test ()
    0