namespace MarkovNextGen

open System
open System.Collections.Generic
open System.IO
open System.Linq
open System.Text.RegularExpressions
open Newtonsoft.Json

/// <summary>
/// A markov generator
/// </summary>
/// <remarks>
/// Takes an optional string parameter 'filename' for the chain file.  Default is 'markov.pdo'
/// </remarks>
type public Markov(filename) =
    let randy = new Random()
    let _filename = filename
    let mutable _chain = MarkovUtilities.ReadChain _filename

    new () =
        Markov("markov.pdo")
    
    /// <summary>
    /// Read-only accessor for the chain
    /// </summary>
    member this.Chain
        with get () = _chain
        and private set (value) = _chain <- value

    member private this.ReadChain() =
        let jsonchain = File.ReadAllText _filename
        JsonConvert.DeserializeObject<Dictionary<string, Link>> jsonchain

    member private this.WriteChain() =
        let jsonchain = JsonConvert.SerializeObject(_chain, Formatting.Indented)
        File.WriteAllText(_filename, jsonchain)
    
    /// <summary>
    /// Adds a single line of text to the chain without updating the chain file
    /// </summary>
    /// <param name="s">The line of text to be processed</param>
    member private this.LinkToChain(s : string) =
        // Consistent casing and treat linebreaks as spaces
        let lower = s.ToLower().Replace(Environment.NewLine, " ").Trim(' ')
        // Remove characters other than letters and spaces
        let cleancopy = Regex.Replace(lower, "[^a-z ]+", "", RegexOptions.Compiled)
        let words = cleancopy.Split ' '
        if words.Length > 1 then // Don't record single words
            for i in [0 .. words.Length - 2] do
                let key = words.[i]
                let value = words.[i + 1]
                if _chain.ContainsKey(key) then
                    _chain.[key].AddAfter(value)
                else
                    _chain.Add(key, Link value)
    
    /// <summary>
    /// Adds a link to the chain and writes to the chain file
    /// </summary>
    /// <param name="word">The key word to add</param>
    /// <param name="link">The list of words that could follow it</param>
    member public this.AddToChain(word, link : IEnumerable<string>) =
        if not (String.IsNullOrWhiteSpace(word)) then
            if _chain.ContainsKey(word) then
                _chain.[word].AddAfter(link)
            else
                _chain.Add(word, new Link(link))
            this.WriteChain()
    
    /// <summary>
    /// Adds a single line of text to the chain and writes to the chain file
    /// </summary>
    /// <param name="s">The line of text to be processed</param>
    member public this.AddToChain(s : string) =
        this.LinkToChain(s)
        this.WriteChain()
    
    /// <summary>
    /// Adds multiple lines of text to the chain and writes to the chain file
    /// </summary>
    /// <param name="lst">The lines of text to be processed</param>
    member public this.AddToChain(lst : IEnumerable<string>) =
        for s in lst do
            this.LinkToChain s
        this.WriteChain()
    
    /// <summary>
    /// Merges another chain into this one
    /// </summary>
    /// <param name="dict">The chain to be merged</param>
    member public this.AddToChain(dict : Dictionary<string, Link>) =
        for kvp in dict do
            if _chain.ContainsKey(kvp.Key) then
                _chain.[kvp.Key].AddAfter(kvp.Value.After)
            else
                // Might be better to copy kvp.Value, we'll see
                _chain.Add(kvp.Key, kvp.Value)
        this.WriteChain()
    
    /// <summary>
    /// Writes the chain to a different file
    /// </summary>
    /// <param name="_file">The name of the output file</param>
    member public this.Dump(_file) =
        let jsonchain = JsonConvert.SerializeObject(_chain, Formatting.Indented)
        File.WriteAllText(_file, jsonchain)
    
    /// <summary>
    /// Prints the chain to console
    /// </summary>
    member public this.PrintChain () =
        // Using static utility since there *shouldn't* be a performance loss
        MarkovUtilities.PrintChain _chain
    
    /// <summary>
    /// Generates a 'sentence' based on a specified length and starting word
    /// </summary>
    /// <param name="length">The number of words to generate</param>
    /// <param name="word">The starting word</param>
    member public this.Generate(length, word) =
        let keys = _chain.Keys.ToList();
        let mutable _word = word
        let mutable genChain = _word

        // Start at 1 since genChain already contains _word
        for i = 1 to length - 1 do
            if _chain.ContainsKey _word then
                if _chain.[_word].After.Length > 0 then
                    _word <- _chain.[_word].RandomAfter
                    genChain <- sprintf "%s %s" genChain _word
            else
                _word <- keys.[randy.Next(0, keys.Count)] // Pick a new word
                genChain <- sprintf "%s, %s" genChain _word // Add a comma since it's a new starting point
        genChain
    
    /// <summary>
    /// Generates a string with automatic length based on a specified starting word
    /// </summary>
    /// <param name="word">The starting word</param>
    member public this.Generate(word) =
        let mutable _word = word
        let mutable genChain = _word

        let mutable i = 0 // Break at 50 so we don't get an infinite loop
        while i < 50 && _chain.ContainsKey _word do
            if _chain.[_word].After.Length > 0 then
                _word <- _chain.[_word].RandomAfter
                genChain <- sprintf "%s %s" genChain _word
        genChain
    
    /// <summary>
    /// Generates a 'sentence' based on a specified length and random starting word
    /// </summary>
    /// <param name="length">The number of words to generate</param>
    member public this.Generate(length) =
        let keys = _chain.Keys.ToList()
        let word = keys.[randy.Next(0, keys.Count)]
        this.Generate(length, word)
    
    /// <summary>
    /// Generates a string with automatic length based on a random starting word
    /// </summary>
    member public this.Generate() =
        let keys = _chain.Keys.ToList()
        let word = keys.[randy.Next(0, keys.Count)]
        this.Generate(word)

/// <summary>
/// A generic markov generator
/// </summary>
/// <remarks>
/// Takes an optional string parameter 'filename' for the chain file.  Default is 'markov.pdo'
/// </remarks>
type public Markov<'T when 'T : (new : unit -> 'T) and 'T : equality>(filename) =
    let randy = new Random()
    let _filename = filename
    let mutable _chain = MarkovUtilities.Generics.ReadChain<'T> _filename

    new () =
        Markov<'T>("markov.pdo")
    
    /// <summary>
    /// Read-only accessor for the chain
    /// </summary>
    member this.Chain
        with get () = _chain
        and private set (value) = _chain <- value

    member private this.ReadChain() =
        let jsonchain = File.ReadAllText _filename
        JsonConvert.DeserializeObject<Dictionary<'T, Link<'T>>> jsonchain

    member private this.WriteChain() =
        let jsonchain = JsonConvert.SerializeObject(_chain, Formatting.Indented)
        File.WriteAllText(_filename, jsonchain)
    
    /// <summary>
    /// Adds a single list of items to the chain without updating the chain file
    /// </summary>
    /// <param name="items">An F# list of items to be processed</param>
    member private this.LinkToChain(items : 'T list) =
        if items.Length > 1 then // Don't record single items
            for i in [0 .. items.Length - 2] do
                let key = items.[i]
                let value = items.[i + 1]
                if _chain.ContainsKey(key) then
                    _chain.[key].AddAfter(value)
                else
                    _chain.Add(key, Link<'T> value)
    
    /// <summary>
    /// Adds a link to the chain and writes to the chain file
    /// </summary>
    /// <param name="item">The key item to add</param>
    /// <param name="link">The list of items that could follow it</param>
    member public this.AddToChain(item, link : IEnumerable<'T>) =
        if _chain.ContainsKey(item) then
            _chain.[item].AddAfter(link)
        else
            _chain.Add(item, new Link<'T>(link))
        this.WriteChain()
    
    /// <summary>
    /// Adds a list of items to the chain and writes to the chain file
    /// </summary>
    /// <param name="items">The list of items to be added</param>
    member public this.AddToChain(items : 'T list) =
        this.LinkToChain items
        this.WriteChain

    /// <summary>
    /// Adds a generic List<'T> of items to the chain and writes to the chain file
    /// </summary>
    /// <param name=itemss">The list of items to be processed</param>
    member public this.AddToChain(items : IEnumerable<'T>) =
        items |> List.ofSeq |> this.LinkToChain
        this.WriteChain()
    
    /// <summary>
    /// Merges another chain into this one
    /// </summary>
    /// <param name="dict">The chain to be merged</param>
    member public this.AddToChain(dict : Dictionary<'T, Link<'T>>) =
        for kvp in dict do
            if _chain.ContainsKey(kvp.Key) then
                _chain.[kvp.Key].AddAfter(kvp.Value.After)
            else
                // Might be better to copy kvp.Value, we'll see
                _chain.Add(kvp.Key, kvp.Value)
        this.WriteChain()
    
    /// <summary>
    /// Writes the chain to a different file
    /// </summary>
    /// <param name="_file">The name of the output file</param>
    member public this.Dump(_file) =
        let jsonchain = JsonConvert.SerializeObject(_chain, Formatting.Indented)
        File.WriteAllText(_file, jsonchain)
    
    /// <summary>
    /// Prints the chain to console
    /// </summary>
    member public this.PrintChain () =
        // Using static utility since there *shouldn't* be a performance loss
        MarkovUtilities.Generics.PrintChain<'T> _chain
    
    /// <summary>
    /// Generates markov output with automatic length based on a specified starting item
    /// </summary>
    /// <param name="item">The starting item</param>
    /// <param name="max">The maximum length of the chain before we assume an infinite loop</param
    member public this.Generate(item, max) =
        let mutable _item = item
        let genChain = new List<'T>()
        genChain.Add _item

        let mutable i = 0 // Break at max so we don't get an infinite loop
        while i < max && _chain.ContainsKey _item do
            if _chain.[_item].After.Length > 0 then
                _item <- _chain.[_item].RandomAfter
                genChain.Add _item
        genChain
    
    /// <summary>
    /// Generates markov output with automatic length and a specified starting item
    /// </summary>
    /// <remarks>
    /// Automatically breaks at 50 items to avoid infinite loops
    /// To avoid this, you can call Generate(T, int) to specify your own max length
    ///</remarks>
    /// <param name="item">The starting item</param>
    member public this.Generate(item) = this.Generate(item, 50)
    
    /// <summary>
    /// Generates markov output with automatic length and a random starting word
    /// </summary>
    member public this.Generate() =
        let keys = _chain.Keys.ToList()
        let item = keys.[randy.Next(0, keys.Count)]
        this.Generate(item)