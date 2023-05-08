using System.Text.RegularExpressions;

internal class Program
{
    static readonly Dictionary<char, int> HebrewAlphabet = new Dictionary<char, int> {
        {'א', 1}, {'ב', 2}, {'ג', 3}, {'ד', 4}, {'ה', 5}, {'ו', 6}, {'ז', 7},
        {'ח', 8}, {'ט', 9}, {'י', 10}, {'כ', 20}, {'ל', 30}, {'מ', 40}, {'נ', 50},
        {'ס', 60}, {'ע', 70}, {'פ', 80}, {'צ', 90}, {'ק', 100}, {'ר', 200},
        {'ש', 300}, {'ת', 400}, {'ך', 20}, {'ם', 40}, {'ן', 50}, {'ף', 80}, {'ץ', 90}
    };
    
    public static void Main(string[] args)
    {
        // Declare dictionary file (word list)
        string dictionaryFile = "Dictionary.csv";
        
        // Declare word to calculate with
        string wordOfChoice = "יידיש";
        
        // Calculate gematria of wordOfChoice
        int gematria = CalculateGematria(wordOfChoice);
        
        // Check whether or not the file exists - if not, create it.
        if (!File.Exists(dictionaryFile)) {
            CreateDictionaryFile(dictionaryFile,
                new []
                {
                    "Bereishis.csv", "Shemos.csv", "Vayikro.csv", "Bemidbar.csv", "Dvorim.csv"
                });
        }

        // Make a dictionary with a string index returning an int
        Dictionary<string, int> dictionary = new();
        
        // Split each line on ';' and make the word the key, the gematria of the word the value
        foreach (string line in File.ReadLines(dictionaryFile))
        {
            string[] split = line.Split(";");
            dictionary[split[0]] = int.Parse(split[1]);
        }
        
        // Find words of similar numerical value and add them to a list
        List<string> wordsOfEqualGematria = new();
        foreach (string word in dictionary.Keys)
        {
            if (word == wordOfChoice) continue;
            if (dictionary[word] == gematria) wordsOfEqualGematria.Add(word);
        }

        // Print word and its gematria
        Console.WriteLine($"The gematria of {wordOfChoice} is {CalculateGematria(wordOfChoice)}.");

        // Print words with the same gematria
        Console.WriteLine("Words with a similar numerical value are:");
        
        foreach(string word in wordsOfEqualGematria) Console.WriteLine(word);
    }
    
    static int CalculateGematria(string word)
    {
        int counter = 0;
        try {
            foreach (char letter in word.Trim())
            {
                counter += HebrewAlphabet[letter];
            }
        } catch (Exception _) { }
        
        return counter;
    }

    static void CreateDictionaryFile(string fileToCreate, string[] fileNames)
    {
        List<string> dictionary = new();
        string regexRemoveTags = "(<)|([a-z])|(>)|(\\/)|(׃)|(־)|(=)|(\\[)|(\\])";

        foreach (string fileName in fileNames)
        {
            IEnumerable<string> words = string.Join(" ", File.ReadLines(fileName)
                .Select(x => Regex.Replace(x.Split(",")[1], regexRemoveTags, " "))
                .ToArray()).Split(" ");

            foreach (string word in words)
            {
                dictionary.Add(word);
            }
        }

        using StreamWriter writer = new StreamWriter(fileToCreate);
        {            
            string toWrite =
                string.Join("\n",
                from word in dictionary.Distinct()
                select $"{word};{CalculateGematria(word)}");

            writer.Write(toWrite);
            writer.Close();
        }
    }
}