module GraphColouring.Algorithms.RandomColour

open GraphColouring.Graph
open GraphColouring.TypeProvider
open Hekate
open System

(*
    Random Colour Algorithm - Basic
        Psuedo Code:
        1. Randomly select vertex/node and colour it with the lowest chromatic index
        2. For each remaining vertex/node
            a. Choose a vertex/node at random
            b. Consider the lowest available chromatic index that is not present
                on an adjacent vertex/node
            c. If all adjacent vertices/nodes assign a new chromatic index
        Assumptions:
        - '-1' means a vertex/node has not been assigned a chomatic index yet
*)

let rec private _tryIndex cap index (indexList: Map<int, string list>) =
    (* Increment through the indices available *)
    match Map.tryFind index indexList with
    | None -> index
    | Some l when List.length l < cap && index >= 0 -> index
    | Some _ -> _tryIndex cap (index+1) indexList

let private _copyShuffle (rnd: Random) source =
    (* Use Fisher-Yates shuffle for randomizing *)
    let arr = Seq.toArray source
    let len = Array.length arr
    for i in 0 .. len - 2 do
        let tmp, j = arr.[i], rnd.Next(i, len)
        arr.[i] <- arr.[j]; arr.[j] <- tmp
    arr |> List.ofArray

let colourGraph (settings: Settings) (graph: Graph<string, int, string>) =
    let edges = Hekate.Graph.Edges.toList graph
    (* Randomise ordering of nodes *)
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
                    0, (g |> toIndexMap)
                | edglist -> 
                    List.where
                        (fun (name, _) ->
                            (* Get all nodes that appear in the list *)
                            List.where (fun (vertex1, vertex2, _) -> vertex2 = name || vertex1 = name) edglist
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
                    | [] -> 0, (g |> toIndexMap)
                    | indexlist ->
                        (* Bloat indices that conflict via edges *)
                        let indexMap =
                            List.fold
                                (fun acc ind ->
                                    Map.add
                                        ind
                                        (
                                            match settings.Room with
                                            | None -> []
                                            | Some i -> List.init i (fun _ -> "")
                                        )
                                        acc
                                )
                                (g |> toIndexMap)
                                indexlist
                        (* Find the maximum chromatic index out of the receieved nodes *)
                        let index =
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
                        index, indexMap
                |> fun (i, indexMap) -> 
                    (* If a room limit has been set then reassign index *)
                    match settings.Room with
                    | None -> i
                    | Some cap -> _tryIndex cap i indexMap
                |> fun i -> Hekate.LNode (vertex, i)

            (* This following will create a new graph with the current node being modified *)
            Hekate.Graph.Nodes.add colouredNode g
        )
        graph
        (graph |> Hekate.Graph.Nodes.toList |> _copyShuffle (Random()))