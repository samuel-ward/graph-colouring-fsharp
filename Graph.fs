module GraphColouring.Graph

open Hekate
open Newtonsoft.Json
open System
open TypeProvider

let private _createExamVertices (exams: Exam list) =
    (* Create graph nodes of all exams *)
    List.init
        (List.length exams)
        (fun i ->
            (* Set label to -1 to denote chromatic index *)
            Hekate.LNode((List.item i exams).Id, -1)
        )

let private _createExamConflictEdges (nodes: LNode<string, int> list) (exams: Exam list) =
    (* Create edges that denote scheduling conflicts for students *)
    List.map
        (fun e ->
            List.fold
                (fun acc ee ->
                    (* Match all exams, excluding self, and matches students taking both exams *)
                    match ee with
                    (* Ignore current exam - avoid self-referencing edge *)
                    | s when String.Equals(s.Id, e.Id) -> acc
                    | s ->
                        List.map
                            (fun ss ->
                                (* Find all exams taken by the given student *)
                                match List.exists (fun x -> x = ss) e.Students with
                                | true -> 
                                    Some (
                                        Hekate.LEdge(
                                            (List.find (fun (l, _) -> String.Equals(l, e.Id)) nodes |> fun (i, _) -> i),
                                            (List.find (fun (l, _) -> String.Equals(l, s.Id)) nodes |> fun (i, _) -> i),
                                            "conflict"
                                        )
                                    )
                                | false -> None
                            )
                            s.Students
                        (* Remove all None responses - leaving only the created edges *)
                        |> List.choose id
                )
                []
                exams
        )
        exams
    |> List.collect id

let createExamConstraintGraph (settings: Settings) (exams: Exam list) =
    (* Create constraint graph *)
    let nodes = _createExamVertices exams
    let edges = _createExamConflictEdges nodes exams
    Hekate.Graph.create
        nodes
        edges

let toIndexMap (graph: Graph<'a, 'b, _>) =
    let nodes = Hekate.Graph.Nodes.toList graph
    List.mapFold
        (* Convert nodes into a map based on time slot - i.e. chromatic index *)
        (fun acc node ->
            let name, index = node
            true, Map.add
                index
                (
                    match Map.tryFind index acc with 
                    | None -> [name]
                    | Some values -> List.append values [name]
                )
                acc
        )
        Map.empty<'b, 'a list>
        nodes
    |> fun (_, g) -> g

let printGraph (graph : Graph<'a, 'b, _>) =
    Console.WriteLine "---- Base Graph Details ----"

    Console.WriteLine "NodeList::"
    Hekate.Graph.Nodes.toList graph
    |> JsonConvert.SerializeObject
    |> Console.WriteLine

    Console.WriteLine "Nodes::"
    Hekate.Graph.Nodes.count graph
    |> Console.WriteLine

    Console.WriteLine "EdgeList::"
    Hekate.Graph.Edges.toList graph
    |> JsonConvert.SerializeObject
    |> Console.WriteLine

    Console.WriteLine "Edges::"
    Hekate.Graph.Edges.count graph
    |> sprintf "Edges: %i"
    |> Console.WriteLine

    Console.WriteLine "---- - ----"

let printGraphColouring (graph: Graph<'a, 'b, _>) =
    (* Colours are timetable slots *)
    Console.WriteLine "---- Colouring of Graph ----"

    Hekate.Graph.Nodes.toList graph
    |> List.iter
        (fun node ->
            let vert, label = node
            sprintf "Vertex %A ---> Colour %A" vert label
            |> Console.WriteLine
        )

    Console.WriteLine "---- - ----"

let printTimetable (graph: Graph<string, int, _>) =
    Console.WriteLine "---- Timetable ----"
    graph
    |> toIndexMap
    |> fun table -> {Timetable.Timeslots = table}
    |> GraphColouring.Helpers.printTimetable
    Console.WriteLine "---- - ----"