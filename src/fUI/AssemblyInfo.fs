namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("fUI")>]
[<assembly: AssemblyProductAttribute("fUI")>]
[<assembly: AssemblyDescriptionAttribute("Fsharp framework to build console rich application")>]
[<assembly: AssemblyVersionAttribute("1.0")>]
[<assembly: AssemblyFileVersionAttribute("1.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "1.0"
