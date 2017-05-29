namespace fUI


// NEW


module fUI =
    type Parent = | Widget | Screen
    type Coord = { top:int; left: int}
    type Size = { width:int ; height: int  }

    type Form = { parent:Parent; keys:bool; size:Size; position:Coord; background: string; content: string }

    let form = {
        parent = Parent.Screen;
        keys= true;
        size = { width = 30; height = 4 };
        position = { left = 0 ; top = 0 };
        background = "green";
        content = "Submit or cancel?"
    }
    
 //






module Interface = 
    open System.Runtime.CompilerServices
    open System.IO
    open System

    [<Extension>]
    type Screen =
        [<Extension>]
        static member DrawBox (writer:TextWriter) = 
            writer.Write 
