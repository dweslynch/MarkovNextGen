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
    
    let Merge (from : Dictionary<string, Link>) (target : Dictionary<string, Link>) =
        let _target = new Dictionary<string, Link> (target)
        for kvp in from do
            if _target.ContainsKey kvp.Key then // Entry exists, merge
                _target.[kvp.Key].AddAfter(kvp.Value.After)
            else
                let link = new Link(kvp.Value.After)    // Create new entry
                _target.Add(kvp.Key, link)
        _target
    
    let PrintChain (chain : Dictionary<string, Link>) =
        for kvp in chain do
            printfn "%s" kvp.Key
            for x in kvp.Value.After do
                printfn "\t%s" x
            printfn ""
    
    let Generate (chain : Dictionary<string, Link>) length word =
        let keys = chain.Keys.ToList();
        let mutable _word = word
        let mutable genChain = _word

        // Starts at 1 since genChain already contains _word
        for i = 1 to length - 1 do
            if chain.ContainsKey _word then
                if chain.[_word].After.Length > 0 then
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
            if chain.[_word].After.Length > 0 then
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