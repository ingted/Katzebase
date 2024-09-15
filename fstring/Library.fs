namespace fs

open System
open System.Collections
open System.Collections.Generic

type fstring =
| S of string
| D of double
| A of fstring []
| T of string * fstring
    static member compareArrays (arr1: fstring array) (arr2: fstring array): int =
        Seq.zip arr1 arr2
        |> Seq.tryPick (fun (x, y) ->
            let res = compare x y
            if res = 0 then None else Some res)
        |> Option.defaultValue 0

    static member compareLength (arr1: fstring array) (arr2: fstring array): int =
        compare (arr1.Length) (arr2.Length)
    static member Compare (x: fstring, y: fstring): int =
        match (x, y) with
        | (D d1, D d2) -> Decimal.Compare(decimal d1, decimal d2) // 直接使用 decimal 的比较功能
        | (S s1, S s2) -> String.Compare(s1, s2, StringComparison.OrdinalIgnoreCase)
        | (A arr1, A arr2) ->
            let lenComp = fstring.compareLength arr1 arr2
            if lenComp <> 0 then lenComp
            else fstring.compareArrays arr1 arr2
        | (T (tag1, f1), T (tag2, f2)) ->
            let tagComp = String.Compare(tag1, tag2, StringComparison.OrdinalIgnoreCase)
            if tagComp <> 0 then tagComp
            else fstring.Compare (f1, f2)
        | (D _, _) -> -1 // 所有 D 与其他类型比较时的默认顺序
        | (_, D _) -> 1  // 所有其他类型与 D 比较时的默认顺序
        | (S _, (A _ | T _)) -> -1
        | ((A _ | T _), S _) -> 1
        | (A _, T _) -> -1
        | (T _, A _) -> 1

    interface IComparer<fstring> with
        override this.Compare(x: fstring, y: fstring): int =
            fstring.Compare(x, y)
    member this.s =
        match this with
        | S s -> s
        | _ -> failwith "Not fstring.S."
    member this.d =
        match this with
        | D d -> d
        | _ -> failwith "Not fstring.D."
    member this.db =
        match this with
        | D d -> d
        | _ -> failwith "Not fstring.D."
    member this.ToLowerInvariant () =
        match this with
        | S s -> s.ToLowerInvariant() |> S
        | _ -> failwith "Not fstring.S."



    static member fromStringArr (sArr) =
        sArr
        |> Array.map S
        |> A
    static member Empty = S ""
    static member IsNullOrEmpty (o:fstring) = if box o = null || o = fstring.Empty then true else false


open System.Runtime.CompilerServices

[<Extension>]
module ExtensionsString =
    [<Extension>]
    let toF(str : string) = S str
[<Extension>]
module ExtensionsDecimal =
    [<Extension>]
    let toF(d : decimal) = D (double d)
[<Extension>]
module ExtensionsInt =
    [<Extension>]
    let toF(d : int) = D (double d)

    
module PB =
    open ProtoBuf.Meta
    open ProtoBuf.FSharp
    let pbModel = 
        RuntimeTypeModel.Default
        |> Serialiser.registerUnionIntoModel<fstring> 

    let serializeF (ms, o) =
        Serialiser.serialise pbModel ms o

    let deserializeF<'T> ms =
        Serialiser.deserialise<'T> pbModel ms
