namespace MarkovNextGen

open System
open System.Collections.Generic
open Newtonsoft.Json
open System.Linq
open System.Drawing

type public Link(lst : string list) =
    class
        let randy = new Random()
        let mutable after = lst

        new (lst : IEnumerable<string>) =
            Link(lst |> List.ofSeq)
        
        new (s) =
            Link([s])
    
        new () =
            Link([])
    
        member public this.After
            with get() = after
            and set (value) = after <- value
        
        member public this.AddAfter(lst) =
            for x in lst do
                after <- x :: after
        
        member public this.AddAfter(lst : string list) =
            after <- List.append after lst
        
        member public this.AddAfter(s) =
            after <- s :: after
        
        [<JsonIgnore>]
        member public this.RandomAfter
            with get () =
                if after.Length > 0 then
                    let i = randy.Next(0, after.Length)
                    after.[i]
                else String.Empty
    end