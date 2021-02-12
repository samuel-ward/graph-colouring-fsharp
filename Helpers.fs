module GraphColouring.Helpers

open Newtonsoft.Json
open System

let printJson (a: 'a) =
    (* Generic function to serialize into JSON and write to console *)
    a
    |> JsonConvert.SerializeObject
    |> Console.WriteLine