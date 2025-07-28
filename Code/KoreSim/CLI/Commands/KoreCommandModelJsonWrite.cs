// using System.Collections.Generic;

// // KoreCommandModelJsonWrite

// public class KoreCommandModelJsonWrite : KoreCommand
// {

//     public KoreCommandModelJsonWrite()
//     {
//         Signature.Add("model");
//         Signature.Add("jsonwrite");
//     }

//     public override string HelpString => $"{SignatureString} <filename>";

//     public override string Execute(List<string> parameters)
//     {
//         if (parameters.Count < 1)
//         {
//             return "KoreCommandModelJsonWrite.Execute -> insufficient parameters";
//         }

//         string filename = parameters[0];
//         string retString = "";

//         // Delete the Entity
//         KoreEventDriver.ModelToJsonFile(filename);

//         retString = $"Model written to {filename}.";

//         return retString;
//     }
// }
