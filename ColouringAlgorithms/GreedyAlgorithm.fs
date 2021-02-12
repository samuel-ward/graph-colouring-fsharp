module GraphColouring.Algorithms.Greedy

open Hekate
open System

(* 
    Greedy Algorithm - Basic
        Psuedo Code:
        1. Colour first vertex/node with first colour
        2. For each remain vertex/node
            a. Consider the lowest numbered colour that has already been used,
                but not on an adjacent vertex/node
            b. If all previously assigned colours are adjacent, assign a new colour
        Assumptions:
        - '-1' means an vertex/node with has not been assigned a colour yet
*)

let colourGraph (graph: Graph<string, int, string>) =
    Console.WriteLine "---- Greedy Colouring ----"
    let edges = Hekate.Graph.Edges.toList graph
    List.fold
        (fun g node ->
            (* Split current node into lable and chromatic index *)
            let vertex, _ = node

            (* Colour current node *)
            let colouredNode = 
                List.where
                    (fun (vertex1, _, _) -> 
                        (* Find all adjacent nodes *)
                        String.Equals(vertex, vertex1)
                    )
                    edges
                |> function
                | [] -> 
                    (* No constraint edges -> set to 0 i.e. lowest index *)
                    Hekate.LNode (vertex, 0)
                | edglist -> 
                    List.where
                        (fun (name, _) ->
                            (* Get all nodes that appear in the list *)
                            List.where (fun (_, vertex2, _) -> vertex2 = name) edglist
                            |> function
                            | [] -> false
                            | _ -> true
                        )
                        (Hekate.Graph.Nodes.toList g)
                    |> function
                    (* If there are connected nodes return their chromatic index, else return nothing *)
                    | [] -> []
                    | nodelist -> List.map (fun (_, i) -> i) nodelist
                    |> function
                    (* If no nodes are connected, set chromatic index to 0, else find the lowest available index to set *)
                    | [] -> 0
                    | indexlist ->
                        (* Find the maximum chromatic index out of the receieved nodes *)
                        List.max indexlist
                        |> function
                        | i when i < 0 -> 0
                        | i ->
                            (* Populate a new list with all available chromatic indices between 0 and 1+max *)
                            List.init (i+1) (fun i -> i+1)
                            (* Negate those that are found on connected nodes *)
                            |> List.choose (fun ii -> List.contains ii indexlist |> function true -> None | false -> Some ii)
                            (* Choose the lowest possible index *)
                            |> List.min
                    |> fun i -> Hekate.LNode (vertex, i)

            (* This following will create a new graph with the current node being modified *)
            Hekate.Graph.Nodes.add colouredNode g
        )
        graph
        (Hekate.Graph.Nodes.toList graph)

let printTimetable (graph: Graph<string, int, string>) =
    Console.WriteLine "---- Timetable ----"
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
        Map.empty<int, string list>
        nodes
    |> fun (_, table) -> table
    |> GraphColouring.Helpers.printJson
    Console.WriteLine "---- - ----"