namespace fUI

/// Documentation for my library
///
/// ## Example
///
///     let h = Library.hello 1
///     printfn "%d" h
///
module braille = 
  
  let patternMap =  [
                    [0x1, 0x8],
                    [0x2, 0x10],
                    [0x4, 0x20],
                    [0x40,0x80]
                    ]

type Canvas = 
    
  