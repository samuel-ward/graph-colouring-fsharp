[<AutoOpen>]
module GraphColouring.CsvHelper

open FSharp.Data
open System
open System.Collections.Generic
open TypeProvider

(* Testing base CSV converter *)
let private _dataURI = "./Data/anonymised.csv"

let private _retrieveCsvData () =
    CsvFile.AsyncLoad _dataURI

let private _printCsvData () =
    let csv = _retrieveCsvData () |> Async.RunSynchronously
    Seq.iter
        (fun (r: CsvRow) ->
            Seq.iter
                (sprintf "%s" >> Console.WriteLine)
                r.Columns
        )
        csv.Rows
(*  *)

type StudentCsvTypeProvider = CsvProvider<"./Data/anonymised.csv", HasHeaders=false, Schema="string, string option, string option, string option, string option">

(* Not used anymore *)
let private _mapColumnsToList (c2: string option) c3 c4 c5 =
    [c2; c3; c4; c5]
    |> List.choose id

let getStudentData () =
    let temp = StudentCsvTypeProvider.GetSample()
    temp.Rows
    |> Seq.map
        (fun r ->
            let courseList = _mapColumnsToList r.Column2 r.Column3 r.Column4 r.Column5
            {
                Student.Id = r.Column1
                Courses = courseList
                CoursesQuantity = List.length courseList
            }
        )
    |> Seq.sortByDescending (fun s -> s.CoursesQuantity)
    |> Seq.toList
(*  *)

let private _getExamTotals (csv: StudentCsvTypeProvider) =
    (* Converts CSV file into map of exams and the total students sitting them *)
    csv.Rows
    |> Seq.map
        (fun r ->
            _mapColumnsToList r.Column2 r.Column3 r.Column4 r.Column5
        )
    |> Seq.collect id
    |> Seq.countBy id
    |> Map.ofSeq

let rec examStudentsBuilder (state: Map<string, string list>) (items: string list) (value: string) =
    (* Adds students to exam student lists *)
    match items with
    | [h] when state.ContainsKey h -> state.Add(h, (List.append state.[h] [value]))
    | [h] -> state.Add(h, [value])
    | h::t when state.ContainsKey h -> examStudentsBuilder (state.Add(h, (List.append state.[h] [value]))) t value
    | h::t -> examStudentsBuilder (state.Add(h, [value])) t value
    | _ -> state

let private _getExamStudents (csv: StudentCsvTypeProvider) =
    (* Converts CSV file into a map of exams and the students taking those exams *)
    csv.Rows
    |> Seq.mapFold
        (fun (state: Map<string, string list>) row -> 
            let exams = [row.Column2; row.Column3; row.Column4; row.Column5] |> List.choose id
            row, examStudentsBuilder state exams row.Column1
        )
        Map.empty
    |> function
    | _, map -> map

let private _examStudentsToExams (examStudents: Map<string, string list>) =
    (* Maps exam map to list of exams *)
    Seq.map
        (fun (KeyValue(k, v)) ->
            {
                Exam.Id = k
                Students = v
                StudentsQuantity = List.length v
            }
        )
        examStudents
    |> Seq.sortByDescending (fun e -> e.StudentsQuantity)
    |> Seq.toList

let getExamData () =
    (* Create exam map from CSV file *)
    let temp = StudentCsvTypeProvider.GetSample()
    _getExamStudents temp
    |> _examStudentsToExams