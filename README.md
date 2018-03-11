# MarkovNextGen
A lightning-fast .NET library for generating simple markov chains

Used in [Jahrbuch's "Monika" Discord bot](https://github.com/Jahrbuch/MONI)

# Installation
This package is available on NuGet.  Just search 'MarkovNextGen'

You can also download MarkovNextGen.dll from my website (Coming Soon!).

# Usage

To create a new Markov generator, you can specify a chain file or use the default markov.pdo

```cs
using MarkovNextGen;

var default = new Markov(); // uses markov.pdo
var special = new Markov("myfile.pdo");
```

You can train the generator a variety of ways - with lines of text, a key and list of words, a whole new chain, etc.

```cs
var generator = new Markov();

generator.AddToChain("a quick brown fox jumps over a lazy dog"); // Just a line of text

var newchain = MarkovUtilities.ReadChain("somefile.pdo");
generator.AddToChain(newchain);		// Passing in a new chain
```

You can generate strings based on length, starting word, or any combination thereof

```cs
var generator = new Markov();

generator.AddToChain("a quick brown fox jumps over the lazy dog");

var gen1 = generator.Generate(5, "fox");	// 5 word chain starting with "fox"
var gen2 = generator.Generate(5);			// 5 word chain with random starting word
var gen3 = generator.Generate("fox");		// Automatic-length chain starting with "fox"
var gen4 = generator.Generate();			// Automatic-length chain with random starting word
```

You can refer to the full documentation (Coming Soon!) for more details

# Performance
The lastest version of the `Markov` class adds a lot of improvements in efficiency.  It can train from a 400 line text file and serialize to a 4500 line chain file all in under a second

The static methods in the `MarkovUtilities` class aren't as efficient, but they allow direct manipulation of chains without needing an instance of the `Markov` class.