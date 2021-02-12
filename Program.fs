module GraphColouring.Program


let rec run i =
    Console.WriteLine (sprintf "%A" i)
    run i


[<EntryPoint>]
let main argv =
    0