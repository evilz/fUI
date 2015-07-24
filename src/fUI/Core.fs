namespace fUI

module Core =

    type ConsoleCell =  {
        Char : char;
        Forground : uint16;
        Background : uint16
    }

    type ScreenSize = {
        Width : uint16;
        Height : uint16
    }

     type CellCoordinates = {
        X : uint16;
        Y : uint16
    }


    type ConsoleEvent =  
        | Key of char
        | Resize of ScreenSize
        | Mouse of CellCoordinates
    
