namespace MarkovNextGen

open System
open System.Collections.Generic
open System.Linq

type public Markov(filename) =
    
    let randy = new Random()
    let _filename = filename

    new () =
        Markov("markov.pdo")
    
    member this.Chain
        with get () = MarkovUtilities.ReadChain _filename
        and private set (value) =
            MarkovUtilities.WriteChain _filename value
    
    member public this.AddToChain(word, link : IEnumerable<string>) =
        this.Chain <- MarkovUtilities.LinkToChain this.Chain word link
    
    member public this.AddToChain(s) =
        let res = MarkovUtilities.Train s
        this.Chain <- MarkovUtilities.Merge res this.Chain  // Merge new chain with existing one
    
    // Have to provide this since Chain property doesn't have a public set
    member public this.AddToChain(dict : Dictionary<string, Link>) =
        this.Chain <- MarkovUtilities.Merge dict this.Chain
    
    member public this.PrintChain () =
        MarkovUtilities.PrintChain this.Chain
    
    // Overloads for string generation
    member public this.Generate(length, word) =
        MarkovUtilities.Generate this.Chain length word
    member public this.Generate(length) =
        let keys = this.Chain.Keys.ToList()
        let word = keys.[randy.Next(0, keys.Count)]
        MarkovUtilities.Generate this.Chain length word
    member public this.Generate(word) =
        MarkovUtilities.Autogenerate this.Chain word
    member public this.Generate () =
        let keys = this.Chain.Keys.ToList()
        let word = keys.[randy.Next(0, keys.Count)]
        MarkovUtilities.Autogenerate this.Chain word
    
