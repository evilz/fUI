// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

[<EntryPoint>]
let main argv = 

    System.Console.OutputEncoding <- System.Text.Encoding.Unicode
    System.Console.BackgroundColor <- System.ConsoleColor.DarkYellow
    System.Console.ForegroundColor <- System.ConsoleColor.Black


    let c = 0x2800;

    //28FF

    [0..255]
    |> Seq.iter( fun(i) -> (printfn "%A" ((char)(c + i))))

    printfn "%A" c
    System.Console.ReadLine() |> ignore
    0 // return an integer exit code
