namespace fUI

/// Documentation for my library
///
/// ## Example
///
///     let h = Library.hello 1
///     printfn "%d" h
///

module Wigets = 
    
    type TextBoxWidget = {Text:string}

module Terminal =

    open Wigets

    type CharSize = CharSize of int
    type PixelSise = PixelSize of int

    type WidgetTypes =
        | TextBox of TextBoxWidget
        | Empty

    type WidgetBorder =
        | Light
        | Fat
        | None

    type Widget = {Width:CharSize ; Height:CharSize; WidgetType: WidgetTypes; Border: WidgetBorder }
    
    type Screen = {Width:CharSize ; Height:CharSize; Widgets :Widget list} 
  


module Renderer = 
    
    open Terminal
    open Wigets


    let drawBorder (widget:Widget) =
        

    let renderWidget (widget:Widget) = 

        drawBorder widget
        
        
        match widget.WidgetType with
        |  TextBox t -> printf "%A" t.Text
        
    
        

    let renderWithDot (screen:Screen) =
        screen.Widgets 
        |> Seq.iter(renderWidget)


    let widget1 = { Width=CharSize(5);
                    Height=CharSize(1);
                    WidgetType=WidgetTypes.TextBox({Text="coucou"});
                    Border=Light
                   }

    let app = {Width=CharSize(15); Height=CharSize(15); Widgets=[widget1] }

    renderWithDot app 

    System.Console.ReadLine() |> ignore
  /// Returns 42
  ///
  /// ## Parameters
  ///  - `num` - whatever
 // let hello num = 42
