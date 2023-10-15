using System;
using System.Collections.Generic;
using System.Text;

namespace GodotLayersSourceGenerator;
public static class Utils {

    public static string MakeSafeIdentifier(string identifier) {
        // Trim whitespace
        identifier = identifier.Trim();
        
        // Add underscore if starts with a digit
        if (char.IsDigit(identifier[0])) {
            identifier = "_" + identifier;
        }
        
        // Convert words to pascal case
        var words = identifier.Split(' ');

        var sb = new StringBuilder();
        foreach (var word in words) {
            sb.Append(char.ToUpper(word[0]) + word.Substring(1));
        }

        return sb.ToString();
    }

}
