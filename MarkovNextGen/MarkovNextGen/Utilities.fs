namespace MarkovNextGen

open System
open System.IO
open System.Collections.Generic
open System.Linq
open System.Text.RegularExpressions
open Newtonsoft.Json

module public MarkovUtilities =

    let private randy = new Random()
    
    let ReadChain filename =
        if File.Exists filename then
            let jsonchain = File.ReadAllText filename
            JsonConvert.DeserializeObject<Dictionary<string, Link>> jsonchain
        else
            printfn "_Warn: No PDO detected"
            printfn "Resol: Returning blank Dictionary<string, Link>"
            new Dictionary<string, Link> ()
    
    let WriteChain filename (chain : Dictionary<string, Link>) =
        let jsonchain = JsonConvert.SerializeObject(chain, Formatting.Indented)
        File.WriteAllText (filename, jsonchain)
    
    let LinkToChain (chain : Dictionary<string, Link>) word (link : IEnumerable<string>) =
        let _chain = new Dictionary<string, Link> (chain)
        if String.IsNullOrWhiteSpace word then
            if _chain.ContainsKey word then
                _chain.[word].AddAfter(link)
            else
                let _lnk = { After = new List<string> (link) }
                _chain.Add(word, _lnk)
        _chain
    
    let Train (s : string) =
        // Consistent casing and treat linebreaks as spaces
        let lower = s.ToLower().Replace(Environment.NewLine, " ").Trim(' ')
        // Remove characters other than letters and spaces
        let cleancopy = Regex.Replace(lower, "[^a-z ]+", "", RegexOptions.Compiled)
        let words = cleancopy.Split(' ')
        let mutable chain = new Dictionary<string, Link> ()
        if words.Length >= 2 then // Don't record single words
            for i = 0 to words.Length - 2 do
                let key = words.[i]
                let value = words.[i + 1]
                let after = new List<string> ()
                after.Add value
                chain <- LinkToChain chain key after
        chain
    
    let Merge (from : Dictionary<string, Link>) (target : Dictionary<string, Link>) =
        let _target = new Dictionary<string, Link> (target)
        for kvp in from do
            if _target.ContainsKey kvp.Key then
                _target.[kvp.Key].AddAfter(kvp.Value.After)
            else
                let link = { After = new List<string> (kvp.Value.After) } // Create new Link with a copy of the lsit
                _target.Add(kvp.Key, link)
        _target
    
    let PrintChain (chain : Dictionary<string, Link>) =
        for kvp in chain do
            printfn "%s" kvp.Key
            for x in kvp.Value.After do
                printfn "\t%s" x
    
    let Generate (chain : Dictionary<string, Link>) length word =
        let keys = chain.Keys.ToList();
        let mutable _word = word
        let mutable genChain = _word

        // Starts at 1 since genChain already contains _word
        for i = 1 to length - 1 do
            if chain.ContainsKey _word then
                if chain.[_word].After.Count > 0 then
                    _word <- chain.[_word].RandomAfter
                    genChain <- sprintf "%s %s" genChain _word
            else 
                _word <- keys.[randy.Next(0, keys.Count)] // Pick a new word
                genChain <- sprintf "%s, %s" genChain _word // Add a comma since it's a new starting point
        genChain
    
    let Autogenerate (chain : Dictionary<string, Link>) word =
        let mutable _word = word
        let mutable genChain = _word

        let mutable i = 0 // Break at 50 so we don't get an infinite loop
        while i < 50 && chain.ContainsKey _word do
            if chain.[_word].After.Count > 0 then
                _word <- chain.[_word].RandomAfter
                genChain <- sprintf "%s %s" genChain _word
        genChain
    
    // PDO Manipulation

    let MergeFrom from target = // Merges two PDO files into one
        let _from = ReadChain from
        let _target = ReadChain target

        Merge _from _target
    
    let MergeTo from target =   // Merges one PDO file into another
        let merged = MergeFrom from target
        WriteChain target merged