namespace MarkovNextGen

open System
open System.Collections.Generic
open Newtonsoft.Json

/// <summary>
/// A data structure representing a list of words that can come after a keyword
/// Default constructor initializes the internal list based on a provided F# string list
/// </summary>
type public Link(lst : string list) =
    class
        let randy = new Random()
        let mutable after = lst

        /// <summary>
        /// Constructor that initializes the internal list based the contents of on an IEnumerable<string>
        /// </summary>
        /// <param name="lst">The IEnumerable to initialize from</param>
        new (lst : IEnumerable<string>) =
            Link(lst |> List.ofSeq)
        
        /// <summary>
        /// Constructor that initializes the internal list with a single string
        /// </summary>
        /// <param name="s">The string to initialize with</param>
        new (s) =
            Link([s])
    
        /// <summary>
        /// Constructor that returns an empty Link
        /// </summary>
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
        /// <param name="items">The strings to add</param>
        member public this.AddAfter(items : IEnumerable<string>) =
            for x in items do
                after <- x :: after
        
        /// <summary>
        /// Adds words to the current list
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

/// <summary>
/// A generic data structure representing a list of T-items that can come after a key item
/// Default constructor initializes the internal list based on a provided F# list type
/// The provided type must have a default constructor
/// </summary>
type public Link<'T when 'T : (new : unit -> 'T) and 'T : equality> (lst : 'T list) =
    class
        let randy = new Random()
        let mutable after = lst

        /// <summary>
        /// Constructor that initializes the internal list based on an IEnumerable of type T
        /// </summary>
        /// <param name="lst">The IEnumerable to initialize from</param>
        new (lst : IEnumerable<'T>) =
            Link<'T>(lst |> List.ofSeq)
        
        /// <summary>
        /// Constructor that initializes the internal list with a single item
        /// </summary>
        /// <param name="item">The item to initialize from</param>
        new (item) =
            Link<'T>([item])
        
        /// <summary>
        /// Constructor that return an empty Link<'T>
        /// </summary>
        new () =
            Link<'T>([])
        
        /// <summary>
        /// Public read-write property that corresponds to the internal list of items
        /// </summary>
        member public this.After
            with get() = after
            and set (value) = after <- value
        
        /// <summary>
        /// Adds items to the current list.
        /// </summary>
        /// <param name="items">The items to add</param>
        member public this.AddAfter(items : IEnumerable<'T>) =
            for x in items do
                after <- x :: after
        
        /// <summary>
        /// Adds items to the current list
        /// </summary>
        /// <param name="lst">The items to add</param>
        member public this.AddAfter(lst : 'T list) =
            after <- List.append after lst
        
        /// <summary>
        /// Adds a single item to the current list
        /// </summary>
        /// <param name="item">The item to add</param>
        member public this.AddAfter(item) =
            after <- item :: after
        
        /// <summary>
        /// Read-only accessor that gets a random item from the list
        /// </summary>
        [<JsonIgnore>]
        member public this.RandomAfter
            with get () =
                if after.Length > 0 then
                    let i = randy.Next(0, after.Length)
                    after.[i]
                else new 'T()
    end