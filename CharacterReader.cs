namespace CharacterConsole;

public class CharacterReader
{
    public string[] CharacterLines {get;set;}
    public List<Character> CharactersList {get;set;} = new List<Character>();
    public List<string> CharacterNamesList {get;set;} // For menu of characters

    public string GetName(string line)
    {
        string name;
        int commaIndex;

        // Check if the name is quoted
        if (line.StartsWith("\""))
        {
            // TODO: Find the closing quote and the comma right after it
            
            commaIndex = line.IndexOf(',');
            name = line.Substring(0, commaIndex);
            int pos = name.Length + 1;

            var line2 = line.Substring(pos);

            int commaIndex2 = line2.IndexOf(',');

            int nameEndsIndex = pos + commaIndex2;

            // TODO: Remove quotes from the name if present and parse the name
            name = line.Substring(0,nameEndsIndex);
            name = name.Replace("\"","");
        }
        else
        {
            // TODO: Name is not quoted, so store the name up to the first comma
            commaIndex = line.IndexOf(',');
            name = line.Substring(0,commaIndex);
        }
        return name;
    }
    
        public (string, int, int, string[]) GetCharacterTraits(string line)
    {   
        string[] fields = line.Split(",");

        string heroClass = fields[^4];
        int level = Convert.ToInt16(fields[^3]);
        int hitPoints = Convert.ToInt16(fields[^2]);
        string heroEquipmentString = fields[^1];
        
        // TODO: Parse equipment noting that it contains multiple items separated by '|'
        string[] heroEquipmentArray = heroEquipmentString.Split('|');

        return (heroClass, level, hitPoints, heroEquipmentArray);

    }

    public void LoadCharacters()
    {
        for (int i = 1; i < CharacterLines.Length; i++)
        {
            string line = CharacterLines[i];

            string characterName = GetName(line);

            var (characterClass, characterLevel, characterHitPoints, characterEquipment) = GetCharacterTraits(line);

            CharactersList.Add(new Character()
            {
                CharacterName = characterName,
                CharacterClass = characterClass,
                CharacterLevel = characterLevel,
                CharacterHitPoints = characterHitPoints,
                CharacterEquipment = characterEquipment
            });
        }

    }

    public void DisplayCharacterNamesMenu()
    {
        CharacterNamesList = new List<string>();

        foreach (var character in CharactersList)

        {
            CharacterNamesList.Add(character.CharacterName);
        }

        for (int i = 0; i < CharacterNamesList.Count; i++)
        {
            Console.WriteLine($"{i+1}: {CharacterNamesList[i]}");
        }
    }

    public Character FindCharacter(string choice)
    {
        int indexToFind = Convert.ToInt16(choice) - 1;
        string NameToFind = CharacterNamesList[indexToFind];
        var foundCharacter = CharactersList.Where(c => c.CharacterName == NameToFind).FirstOrDefault();
        return foundCharacter;
    }

    public void DisplayCharacters()
    {
        foreach (var character in CharactersList)
            {
                Console.WriteLine($"Name: {character.CharacterName}; Class: {character.CharacterClass}; Level: {character.CharacterLevel}; Hit Points: {character.CharacterHitPoints};  Equipment: {string.Join(", ", character.CharacterEquipment)}");
            }
    }

    public void AddCharacter(string newCharacter, string newClass, string[] choicesArray)
    {
        CharactersList.Add(new Character()
            {
                CharacterName = newCharacter,
                CharacterClass = newClass,
                CharacterLevel = 1,
                CharacterHitPoints = 10,
                CharacterEquipment = choicesArray
            });

    }
}