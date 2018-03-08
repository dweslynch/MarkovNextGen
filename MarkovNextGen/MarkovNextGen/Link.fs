namespace MarkovNextGen

open System
open System.Collections.Generic
open System.Linq

type public Link =
    struct
        val private randy : Random
        val mutable private after : string list

        new (lst : IEnumerable<string>) =
            { after = lst |> List.ofSeq; randy = new Random() }
        
        new (lst : string list) =
            { after = lst; randy = new Random() }
        
        member public this.After
            with get () = this.after
        
        // Way more efficient than adding each element individually
        member public this.AddAfter(lst : IEnumerable<string>) =
            let _lst = lst |> List.ofSeq
            this.after <- List.append this.after _lst
        
        member public this.AddAfter(lst : string list) =
            this.after <- List.append this.after lst
        
        member public this.AddAfter(s) =
            this.after <- s :: this.after
        
        member public this.RandomAfter
            with get () =
                if this.after.Length > 0 then
                    let i = this.randy.Next(0, this.after.Length)
                    this.after.[i]
                else String.Empty
    end

(*
type public Link =
    {
        After : List<string>;
    }
    member private this.randy = new Random()
    member public this.AddAfter(s) = this.After.Add(s)
    member public this.AddAfter(lst : IEnumerable<string>) =
        for x in lst do
            this.AddAfter(x)
    member public this.RandomAfter
        with get () =
            if this.After.Count > 0 then
                let i = this.randy.Next(0, this.After.Count)
                this.After.[i]
            else String.Empty
            *)