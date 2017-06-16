open System
open System.Diagnostics
open System.Runtime.InteropServices
open Microsoft.Win32.SafeHandles
open FSharp.NativeInterop

module NativeMethods =
    let STD_INPUT_HANDLE = -10

    let ENABLE_MOUSE_INPUT = 0x0010
    let ENABLE_QUICK_EDIT_MODE = 0x0040;
    let ENABLE_EXTENDED_FLAGS = 0x0080;
    let KEY_EVENT = 1;
    let MOUSE_EVENT = 2;
    let WH_MOUSE_LL = 14

    // type MouseMessages = 
    //     | WM_LBUTTONDOWN = 0x0201
    //     | WM_LBUTTONUP = 0x0202
    //     | WM_MOUSEMOVE = 0x0200
    //     | WM_MOUSEWHEEL = 0x020A
    //     | WM_RBUTTONDOWN = 0x0204
    //     | WM_RBUTTONUP = 0x0205


    [<Struct>]
    type COORD = { X:UInt16; Y:UInt16; }

    [<Struct>]
    type MOUSE_EVENT_RECORD = {
        dwMousePosition:COORD;
        dwButtonState:Int32;
        dwControlKeyState:Int32;
        dwEventFlags:Int32;
    } with
        // member defined with type declaration
        member this.FormatAsString = 
                let sb = Text.StringBuilder()
                Printf.bprintf sb  "Mouse event\n"
                Printf.bprintf sb   "    X ...............:   %0+4i  \n"  this.dwMousePosition.X
                Printf.bprintf sb   "    Y ...............:   %0+4i  \n" this.dwMousePosition.Y
                Printf.bprintf sb   "    dwButtonState ...: 0x%i.  \n" this.dwButtonState
                Printf.bprintf sb   "    dwControlKeyState: 0x%i.  \n" this.dwControlKeyState
                Printf.bprintf sb   "    dwEventFlags ....: 0x%i.  \n" this.dwEventFlags
                sb.ToString()

    [<StructLayout(LayoutKind.Explicit)>]
    [<Struct>]
    type KEY_EVENT_RECORD = {
        [<FieldOffset(0)>]
        [<MarshalAsAttribute(UnmanagedType.Bool)>]
        bKeyDown:bool;
        [<FieldOffset(4)>]
        wRepeatCount:UInt16;
        [<FieldOffset(6)>]
        wVirtualKeyCode:UInt16;
        [<FieldOffset(8)>]
        wVirtualScanCode:UInt16;
        [<FieldOffset(10)>]
        UnicodeChar:Char;
        [<FieldOffset(10)>]
        AsciiChar:Byte;
        [<FieldOffset(12)>]
        dwControlKeyState:Int32;
    } with
        member this.FormatAsString = 
                let sb = Text.StringBuilder()
                Printf.bprintf sb  "Key event  \n"
                Printf.bprintf sb  "    bKeyDown  .......:  %b  \n" this.bKeyDown
                Printf.bprintf sb  "    wRepeatCount ....:    %0+4i  \n" this.wRepeatCount
                Printf.bprintf sb  "    wVirtualKeyCode .:    %0+4i  \n" this.wVirtualKeyCode
                Printf.bprintf sb  "    uChar ...........:       %c  \n" this.UnicodeChar
                Printf.bprintf sb  "    dwControlKeyState: 0x0x%i  \n" this.dwControlKeyState
                sb.ToString()

    [<StructLayout(LayoutKind.Explicit)>]
    [<Struct>]
    type INPUT_RECORD = 
        {
            [<FieldOffset(0)>]
            EventType:Int16 ;
            [<FieldOffset(4)>]
            KeyEvent:KEY_EVENT_RECORD;
            [<FieldOffset(4)>]
            MouseEvent:MOUSE_EVENT_RECORD;
        }

    type ConsoleHandle() =
        inherit SafeHandleMinusOneIsInvalid(false)
        override x.ReleaseHandle() = true

    [<DllImportAttribute("kernel32.dll", SetLastError = true)>]
    extern [<MarshalAs(UnmanagedType.Bool)>] bool GetConsoleMode(ConsoleHandle hConsoleHandle, int& lpMode);

    [<DllImportAttribute("kernel32.dll", SetLastError = true)>]
    extern ConsoleHandle GetStdHandle(Int32 nStdHandle);

    [<DllImportAttribute("kernel32.dll", SetLastError = true)>]
    extern Boolean ReadConsoleInput(ConsoleHandle hConsoleInput, INPUT_RECORD& lpBuffer, UInt32 nLength, UInt32& lpNumberOfEventsRead);

    [<DllImportAttribute("kernel32.dll", SetLastError = true)>]
    extern [<MarshalAs(UnmanagedType.Bool)>] bool  SetConsoleMode(ConsoleHandle hConsoleHandle, Int32 dwMode);

    let (|MouseEvent|_|) inputRecord =
            if inputRecord.EventType = int16 MOUSE_EVENT then Some inputRecord.MouseEvent
            else None

    let (|KeyEvent|_|) inputRecord =
            if inputRecord.EventType = int16 KEY_EVENT then Some inputRecord.KeyEvent
            else None

[<EntryPoint>]
let main argv =
    Console.Clear()

    let handle = NativeMethods.GetStdHandle(NativeMethods.STD_INPUT_HANDLE)

    let mutable mode = 0
    let r = NativeMethods.GetConsoleMode (handle,  &mode)
    if not r  then failwith "Can't get console mode"

    let DISABLE_QUICK_EDIT_MODE = ~~~NativeMethods.ENABLE_QUICK_EDIT_MODE

    mode <- mode ||| NativeMethods.ENABLE_MOUSE_INPUT
    mode <- mode &&& DISABLE_QUICK_EDIT_MODE
    mode <- mode ||| NativeMethods.ENABLE_EXTENDED_FLAGS

    if not (NativeMethods.SetConsoleMode(handle, mode)) then failwith "Can't set console mode"
    printfn "mode %i" mode 
    let mutable record = Unchecked.defaultof<NativeMethods.INPUT_RECORD>

    let mutable recordLen = uint32 0;
    let mutable exit = false

    while not exit do
        
        let read = NativeMethods.ReadConsoleInput(handle, &record, uint32 1, &recordLen)
        if not read then failwith "Can't read console input"
        Console.SetCursorPosition(0, 0)
        match record with
        | NativeMethods.MouseEvent mouseEvent -> printf "%s" mouseEvent.FormatAsString
        | NativeMethods.KeyEvent keyEvent -> 
            printf "%s" keyEvent.FormatAsString
            if record.KeyEvent.wVirtualKeyCode = uint16 ConsoleKey.Escape then exit <- true
        | _ -> printf "BYE"

    0
    


