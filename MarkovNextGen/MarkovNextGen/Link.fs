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