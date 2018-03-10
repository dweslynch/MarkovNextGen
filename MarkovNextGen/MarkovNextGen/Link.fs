namespace MarkovNextGen

open System
open System.Collections.Generic
open Newtonsoft.Json

/// <summary>
/// A data structure representing a list of words that can come after a keyword
/// </summary>
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
        
        /// <summary>
        /// Public read-write property that corresponds to the internal list of strings
        /// </summary>
        member public this.After
            with get() = after
            and set (value) = after <- value
        
        /// <summary>
        /// Adds words to the current list
        /// </summary>
        /// <param name="lst">The strings to add</param>
        member public this.AddAfter(lst) =
            for x in lst do
                after <- x :: after
        
        /// <summary>
        /// Adds wrods to the current list
        /// </summary>
        /// <param name="lst">The strings to add</param>
        member public this.AddAfter(lst : string list) =
            after <- List.append after lst
        
        /// <summary>
        /// Adds a single string to the current list
        /// </summary>
        /// <param name="s">The string to add</param>
        member public this.AddAfter(s) =
            after <- s :: after
        
        /// <summary>
        /// Read-only accessor that gets a random string from the list
        /// </summary>
        [<JsonIgnore>]
        member public this.RandomAfter
            with get () =
                if after.Length > 0 then
                    let i = randy.Next(0, after.Length)
                    after.[i]
                else String.Empty
    end