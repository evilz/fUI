namespace fUI

module Interface = 
    open System.Runtime.CompilerServices
    open System.IO
    open System

    [<Extension>]
    type Screen =
        [<Extension>]
        static member DrawBox (writer:TextWriter) = 
            writer.Write 