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
    }

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
    }

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



    // [<StructLayout(LayoutKind.Sequential)>]
    // [<Struct>]
    // type POINT = { x:int; y:int; }

    // [<StructLayout(LayoutKind.Sequential)>]
    // [<Struct>]
    // type MSLLHOOKSTRUCT = { pt:POINT; mouseData:UInt32; flags:UInt32; time:UInt32; dwExtraInfo:IntPtr; }

    // type LowLevelMouseProc = delegate of int * nativeint * nativeint -> nativeint

    // [<DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)>]
    // extern nativeint SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, nativeint hMod, uint32 dwThreadId);

    // [<DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)>]
    // extern nativeint GetModuleHandle(string lpModuleName);

    // [<DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)>]
    // extern nativeint CallNextHookEx(nativeint hhk, int nCode, IntPtr wParam, IntPtr lParam);

    // [<DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)>]
    // extern [<MarshalAs(UnmanagedType.Bool)>] bool  UnhookWindowsHookEx(IntPtr hhk);



type System.Int32 with                        // x=this
    member x.Flip i = (x ^^^ (1 <<< i))       // change bit  


[<EntryPoint>]
let main argv =

        
    //Console.Clear()

    let handle = NativeMethods.GetStdHandle(NativeMethods.STD_INPUT_HANDLE)

    let mutable mode = 0
    let r = NativeMethods.GetConsoleMode (handle,  &mode)
    printfn "mode %i" mode 
    if not r  then failwith "paf"

    let DISABLE_QUICK_EDIT_MODE = NativeMethods.ENABLE_QUICK_EDIT_MODE.Flip(1)

    mode <- mode ||| NativeMethods.ENABLE_MOUSE_INPUT
    printfn "mode %i" mode 
    mode <- mode &&& DISABLE_QUICK_EDIT_MODE
    printfn "mode %i" mode 
    mode <- mode ||| NativeMethods.ENABLE_EXTENDED_FLAGS
    printfn "mode %i" mode 

    if not (NativeMethods.SetConsoleMode(handle, mode)) then failwith "pouf"

    let mutable record:NativeMethods.INPUT_RECORD =  { 
        EventType=int16 0 ; 
        KeyEvent= {
                    bKeyDown = false;
                    wRepeatCount= uint16 0;
                    wVirtualKeyCode= uint16 0;
                    wVirtualScanCode= uint16 0;
                    UnicodeChar= char 0;
                    AsciiChar= byte 0;
                    dwControlKeyState= 0;
        }; 
        MouseEvent= {
                        dwMousePosition = { X=uint16 0; Y=uint16 0 };
                        dwButtonState=0;
                        dwControlKeyState=0;
                        dwEventFlags=0;}
        }

    let mutable recordLen = uint32 0;
    let mutable exit = false

    while not exit do
        
        let read = NativeMethods.ReadConsoleInput(handle, &record, uint32 1, &recordLen)
        if not read then failwith "ouille"
        Console.SetCursorPosition(0, 0)
        match record.EventType with
        | _ when record.EventType = int16 NativeMethods.MOUSE_EVENT -> 
            printfn "Mouse event"
            printfn "    X ...............:   %0+4i  "  record.MouseEvent.dwMousePosition.X
            printfn "    Y ...............:   %0+4i  " record.MouseEvent.dwMousePosition.Y
            printfn "    dwButtonState ...: 0x%i.  " record.MouseEvent.dwButtonState
            printfn "    dwControlKeyState: 0x%i.  " record.MouseEvent.dwControlKeyState
            printfn "    dwEventFlags ....: 0x%i.  " record.MouseEvent.dwEventFlags
        | _ when record.EventType = int16 NativeMethods.KEY_EVENT ->
            printfn "Key event  "
            printfn "    bKeyDown  .......:  %b  " record.KeyEvent.bKeyDown
            printfn "    wRepeatCount ....:    %0+4i  " record.KeyEvent.wRepeatCount
            printfn "    wVirtualKeyCode .:    %0+4i  " record.KeyEvent.wVirtualKeyCode
            printfn "    uChar ...........:       %c  " record.KeyEvent.UnicodeChar
            printfn "    dwControlKeyState: 0x0x%i  " record.KeyEvent.dwControlKeyState
            if record.KeyEvent.wVirtualKeyCode = uint16 ConsoleKey.Escape then exit <- true
        | _ -> printf "BYE"

    0
    


