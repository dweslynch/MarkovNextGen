namespace MarkovNextGen

open System
open System.Collections.Generic

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

(*
type public Link(after : IEnumerable<string>) =
    
    let randy = new Random()
    let mutable _after = new List<string> (after)

    new() =
        Link(new List<string>())

    member public this.After
        with get () = _after
        and set (value) = _after <- value
    
    member public this.AddAfter(s) =
        _after.Add(s)
    
    member public this.AddAfter(lst : IEnumerable<string>) =
        for s in lst do
            _after.Add(s)
    
    member public this.RandomAfter
        with get () =
            if _after.Count > 0 then
                let i = randy.Next(0, _after.Count)
                _after.[i]
            else
                String.Empty
*)