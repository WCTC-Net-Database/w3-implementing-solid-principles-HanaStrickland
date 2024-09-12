namespace CharacterConsole;

public class CharacterWriter
{
    public List<Character> CharacterWriterList {get;set;}
    private List<string> OutputList = new List<string>();

    public void WriteOutCharacters()
    {
        foreach(var character in CharacterWriterList)
        {
            string nameString;

            if (character.CharacterName.Contains(","))
            {
                nameString = $"\"{character.CharacterName}\"";
            }
            else
            {
                nameString = character.CharacterName;
            }

            string pipeDelimitedChoicesString = string.Join("|", character.CharacterEquipment);

            string lineToAdd = $"{nameString},{character.CharacterClass},{character.CharacterLevel},{character.CharacterHitPoints},{pipeDelimitedChoicesString}";
            
            OutputList.Add(lineToAdd);
        }
        using (StreamWriter outputFile = new StreamWriter("WriteLines.txt"))
            {
                foreach (string line in OutputList)
                    outputFile.WriteLine(line);
            }
    }
}