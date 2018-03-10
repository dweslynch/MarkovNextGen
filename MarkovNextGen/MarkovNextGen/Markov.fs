namespace MarkovNextGen

open System
open System.Collections.Generic
open System.IO
open System.Linq
open System.Text.RegularExpressions
open Newtonsoft.Json
open System.Drawing

type public Markov(filename) =
    let randy = new Random()
    let _filename = filename
    let mutable _chain = MarkovUtilities.ReadChain _filename

    new () =
        Markov("markov.pdo")
    
    member this.Chain
        with get () = _chain
        and private set (value) = _chain <- value
    
    member private this.ReadChain() =
        let jsonchain = File.ReadAllText _filename
        JsonConvert.DeserializeObject<Dictionary<string, Link>> jsonchain

    member private this.WriteChain() =
        let jsonchain = JsonConvert.SerializeObject(_chain, Formatting.Indented)
        File.WriteAllText(_filename, jsonchain)
    
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
    
    member public this.AddToChain(word, link : IEnumerable<string>) =
        if not (String.IsNullOrWhiteSpace(word)) then
            if _chain.ContainsKey(word) then
                _chain.[word].AddAfter(link)
            else
                _chain.Add(word, new Link(link))
            this.WriteChain()
    
    member public this.AddToChain(s : string) =
        this.LinkToChain(s)
        this.WriteChain()
    
    member public this.AddToChain(lst : IEnumerable<string>) =
        for s in lst do
            this.LinkToChain s
        this.WriteChain()
    
    member public this.AddToChain(dict : Dictionary<string, Link>) =
        for kvp in dict do
            if _chain.ContainsKey(kvp.Key) then
                _chain.[kvp.Key].AddAfter(kvp.Value.After)
            else
                // Might be better to copy kvp.Value, we'll see
                _chain.Add(kvp.Key, kvp.Value)
        this.WriteChain()
    
    member public this.Dump(_file) =
        let jsonchain = JsonConvert.SerializeObject(_chain, Formatting.Indented)
        File.WriteAllText(_file, jsonchain)
    
    member public this.PrintChain () =
        // Using static utility since there *shouldn't* be a performance loss
        MarkovUtilities.PrintChain _chain
    
    // Standard generation
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
    
    // Standard autogenerate
    member public this.Generate(word) =
        let mutable _word = word
        let mutable genChain = _word

        let mutable i = 0 // Break at 50 so we don't get an infinite loop
        while i < 50 && _chain.ContainsKey _word do
            if _chain.[_word].After.Length > 0 then
                _word <- _chain.[_word].RandomAfter
                genChain <- sprintf "%s %s" genChain _word
        genChain
    
    member public this.Generate(length) =
        let keys = _chain.Keys.ToList()
        let word = keys.[randy.Next(0, keys.Count)]
        this.Generate(length, word)
    
    member public this.Generate() =
        let keys = _chain.Keys.ToList()
        let word = keys.[randy.Next(0, keys.Count)]
        this.Generate(word)